#include "stdafx.h"

#include <cstdio>
#include <highgui.h>
#include "GetFeature.h"

#define C0 0.4829629131445341
#define C1 0.8365163037378079
#define C2 0.2241438680420134
#define C3 -0.1294095225512604

#define LUVRGB_YRATIO_BOUND 0.008856

float matRGBtoXYZ[3][3];
double Wx,Wy,Wz,u0,v0;
int nLindex = 10;
float luv[159][3];
float LLindex[10];
int Lindex[10][2];


void wtou(unsigned int *step,unsigned char *bytebuf,int pos);
void daub4(float a[], unsigned long n, int isign);
void wtn(float *a, unsigned long nn[], int isign, int iter, void (*wtstep)(float [], unsigned long, int));


void wtou(unsigned int *step,unsigned char *bytebuf,int pos)
{
	unsigned char *temp=(unsigned char *)step;
	int i;
	for(i=0;i<4;i++)
		temp[i]=bytebuf[pos+3-i];
	return;
}

void daub4(float a[], unsigned long n, int isign)
{
	float* wksp = new float[n+1];
	unsigned long dstart,dstart1,i,j;
	
	if (n < 4) return;
	
	dstart1=(dstart=n>>1)+1;
	if (isign >= 0)
    {
		for (i=1,j=1;j<=n-3;j+=2,i++)
		{
			wksp[i]=C0*a[j]+C1*a[j+1]+C2*a[j+2]+C3*a[j+3];
			wksp[i+dstart] = C3*a[j]-C2*a[j+1]+C1*a[j+2]-C0*a[j+3];
		}
		wksp[i]=C0*a[n-1]+C1*a[n]+C2*a[1]+C3*a[2];
		wksp[i+dstart] = C3*a[n-1]-C2*a[n]+C1*a[1]-C0*a[2];
    } 
	else
    {
		wksp[1]=C2*a[dstart]+C1*a[n]+C0*a[1]+C3*a[dstart1];
		wksp[2] = C3*a[dstart]-C0*a[n]+C1*a[1]-C2*a[dstart1];
		for (i=1,j=3;i<dstart;i++)
		{
			wksp[j++]=C2*a[i]+C1*a[i+dstart]+C0*a[i+1]+C3*a[i+dstart1];
			wksp[j++] = C3*a[i]-C0*a[i+dstart]+C1*a[i+1]-C2*a[i+dstart1];
		}
    }
	for (i=1;i<=n;i++) a[i]=wksp[i];

	delete[] wksp;
}


void wtn(float *a, unsigned long nn[], int isign, int iter,
		 void (*wtstep)(float [], unsigned long, int))
{
	unsigned long ntot;
	int count,counter,n1,n2,n1min,n2min,n1max,n2max,j;
	ntot = nn[1] * nn[2];
	float* wksp = new float[ntot];
	
	if(isign>=0)
    {
		n1min=nn[1]>>iter;
		if(n1min<4)
			n1min=4;
		n2min=nn[2]>>iter;
		if(n2min<4)
			n2min=4;
		n1=nn[1]<<1;
		n2=nn[2]<<1;
		//printf("n1=%d,n1min=%d\n",n1,n1min);
		//  printf("n2=%d,n2min=%d\n",n2,n2min);
    }
	else
    {
		n1max=nn[1];
		n2max=nn[2];
		n1=nn[1]>>iter;
		n2=nn[2]>>iter;
    }
	for(j=1;j<=iter;j++)
    {
		if(isign>=0)
		{
			n1=n1>>1;
			n2=n2>>1;
			if((n1<n1min)||(n2<n2min))
			{
				//printf("n1=%d,n1min=%d\n",n1,n1min);
				//printf("n2=%d,n2min=%d\n",n2,n2min);
				//printf("1Too many iterations: exiting.\n");
				// TODO log
				return;
			};
		}
		else
		{
			n1=n1<<1;
			n2=n2<<1;
			if((n1>n1max)||(n2>n2max))
			{
				//printf("Too many iterations: exiting.\n");
				// TODO log
				return;
			};
		};
		for(count=0;count<n1;count++)
		{
			for(counter=0;counter<n2;counter++)
				wksp[counter+1]=a[count*nn[2]+counter];
			(*wtstep)(wksp,n2,isign);
			for(counter=0;counter<n2;counter++)
				a[count*nn[2]+counter]=wksp[counter+1];
		};
		for(count=0;count<n2;count++)
		{
			for(counter=0;counter<n1;counter++)
				wksp[counter+1]=a[count+counter*nn[2]];
			(*wtstep)(wksp,n1,isign);
			for(counter=0;counter<n1;counter++)
				a[count+counter*nn[2]]=wksp[counter+1];
		}
    }

	delete[] wksp;
}


/*
 *	得到8*8维小波特征向量
 */
bool get_waveletfeature(const char* filename, float* feature)
{
	int i,j,nL,t;
	double r,g,b;
	double l,u,v;
	double x,y,z;
	double deno,up,vp;
	double yRatio;

	IplImage* imgRgb = cvLoadImage(filename, CV_LOAD_IMAGE_COLOR);
	if (imgRgb == NULL)
	{
		// log error
		return false;
	}
	CvSize imgSize;
	imgSize.width = imgRgb->width;
	imgSize.height = imgRgb->height;

	unsigned long nn[3] = {0, imgSize.height, imgSize.width};

	float* picluv = new float[imgSize.width * imgSize.height];
	memset(picluv, 0, sizeof(float) * imgSize.width * imgSize.height);

	for( int h = 0; h < imgSize.height; ++h ) 
	{
		for ( int w = 0; w < imgSize.width * 3; w += 3 ) 
		{
			l=u=v=0;
			b  = ((PUCHAR)(imgRgb->imageData + imgRgb->widthStep * h))[w+0] / 255.0;
			g = ((PUCHAR)(imgRgb->imageData + imgRgb->widthStep * h))[w+1] / 255.0;
			r = ((PUCHAR)(imgRgb->imageData + imgRgb->widthStep * h))[w+2] / 255.0;

			if(r!=0.0 || g!=0.0 || b!=0.0){
				x=matRGBtoXYZ[0][0]*r + matRGBtoXYZ[0][1]*g + matRGBtoXYZ[0][2]*b;
				y=matRGBtoXYZ[1][0]*r + matRGBtoXYZ[1][1]*g + matRGBtoXYZ[1][2]*b;
				z=matRGBtoXYZ[2][0]*r + matRGBtoXYZ[2][1]*g + matRGBtoXYZ[2][2]*b;
				//compute u' and v'
				deno = x + 15.0*y + 3.0*z;
				up = 4.0*x / deno;
				vp = 9.0*y / deno;
	
				yRatio = y / Wy;
			
				if(yRatio >= LUVRGB_YRATIO_BOUND)
					l = 116.0 * (double)pow(yRatio, 0.33333333) - 16.0;
				else
					l = 903.3 * yRatio;
				u = 13.0 * l * (up - u0);
				v = 13.0 * l * (vp - v0);
			}
			
			nL = 0; // holds the current Lindex
			while (nL < nLindex && l > LLindex[nL]) nL++;

			for (t=Lindex[nL][0]; t<Lindex[nL][1]; t++) {
				if (u <= luv[t][1]) {
					if (v <= luv[t][2]) {
						picluv[h*imgSize.width+w/3] = t;
						break;
					}
				}
			}				
		}
	}

	cvReleaseImage( &imgRgb );

	wtn(picluv,nn,1,6,daub4);
		
	for(i=0;i<DIM;i++) 
	{
		for(j=0;j<DIM;j++)
		{
			feature[i*DIM+j]=picluv[i*imgSize.width+j];
		}
	}
	
	delete[] picluv;

	return true;
}

/*
 * 初始化 luv[][], LLindex[], Lindex[][]以及matRGBtoXYZ[3][3],Wx,Wy,Wz,u0,v0
 */
bool luv_init(const char* luvFile)
{
  FILE *indexFile;
  int i;
  float L, u, v; 
  int index;
  double deno;

  if (fopen_s(&indexFile, luvFile, "r") != 0) {
     //printf ("Error in opening luv file <%s> in class LuvIndexing\n", "luv.dat");
     // TODO log
	  return false;
  }

  if (feof(indexFile)) return false;

  // read in sizes of luv and Lindex arrays and allocate storages
  for (i=0; i<159; i++) {
     fscanf_s(indexFile, "%f %f %f %d", &L, &u, &v, &index);
     luv[i][0] = L;
     luv[i][1] = u;
     luv[i][2] = v;
  }

  fclose (indexFile);
 
  LLindex[0] = 0.5 * (luv[0][0] + luv[1][0]);
  LLindex[1] = 0.5 * (luv[2][0] + luv[3][0]);
  LLindex[2] = 0.5 * (luv[8][0] + luv[9][0]);
  LLindex[3] = 0.5 * (luv[20][0] + luv[21][0]);
  LLindex[4] = 0.5 * (luv[42][0] + luv[43][0]);
  LLindex[5] = 0.5 * (luv[74][0] + luv[75][0]);
  LLindex[6] = 0.5 * (luv[112][0] + luv[113][0]);
  LLindex[7] = 0.5 * (luv[143][0] + luv[144][0]);
  LLindex[8] = 0.5 * (luv[157][0] + luv[158][0]);
  LLindex[9] = 1000000;

  Lindex[0][0] = 0;
  Lindex[0][1] = 1;
  Lindex[1][0] = 1;
  Lindex[1][1] = 3;
  Lindex[2][0] = 3;
  Lindex[2][1] = 9;
  Lindex[3][0] = 9;
  Lindex[3][1] = 21;
  Lindex[4][0] = 21;
  Lindex[4][1] = 43;
  Lindex[5][0] = 43;
  Lindex[5][1] = 75;
  Lindex[6][0] = 75;
  Lindex[6][1] = 113;
  Lindex[7][0] = 113;
  Lindex[7][1] = 144;
  Lindex[8][0] = 144;
  Lindex[8][1] = 158;
  Lindex[9][0] = 158;
  Lindex[9][1] = 159;
  
 
  
  matRGBtoXYZ[0][0] = 0.607;
  matRGBtoXYZ[0][1] = 0.174;
  matRGBtoXYZ[0][2] = 0.200;
  matRGBtoXYZ[1][0] = 0.299;
  matRGBtoXYZ[1][1] = 0.587;
  matRGBtoXYZ[1][2] = 0.114;
  matRGBtoXYZ[2][0] = 0.000;
  matRGBtoXYZ[2][1] = 0.066;
  matRGBtoXYZ[2][2] = 1.116;
  
  Wx=matRGBtoXYZ[0][0] + matRGBtoXYZ[0][1] + matRGBtoXYZ[0][2];
  Wy=matRGBtoXYZ[1][0] + matRGBtoXYZ[1][1] + matRGBtoXYZ[1][2];
  Wz=matRGBtoXYZ[2][0] + matRGBtoXYZ[2][1] + matRGBtoXYZ[2][2];
  deno = Wx + 15.0*Wy + 3.0*Wz;
  u0 = 4.0*Wx / deno;
  v0 = 9.0*Wy / deno;
  
  return true;
}
