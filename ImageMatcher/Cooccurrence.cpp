#include "stdafx.h"
#include "Cooccurrence.h"
#include "string.h"
#include "math.h"
#include "malloc.h"
#include "stdio.h"
#include <highgui.h>

/************************************************************************/
/* 读BMP文件，获取灰度向量                                                                     */
/************************************************************************/
/*int Cooccurrence::ReadColorImage(char *FileName, int *Row, int *Col, unsigned char *BufR, unsigned char *BufG,
				   unsigned char *BufB,unsigned char *BufY, unsigned char *Fhead)
{ 
	unsigned long Index=0; 
	int i,j,k;
	FILE *ImageDataFile;  
  
	if((ImageDataFile=fopen(FileName,"rb"))==NULL) 
		return(0);
 
	for(i=0; i<54; i++) 
		Fhead[i]=fgetc(ImageDataFile);

	*Col=Fhead[19]*256+Fhead[18];//得到图像长度
	*Row=Fhead[23]*256+Fhead[22];//得到图像宽度

	k=4-(*Col)*3%4; 
	if(k==4) k=0;//固定格式

	for(i=0;i<(*Row);i++)
	{ 
		for(j=0;j<(*Col);j++,Index++)
		{ 
			BufB[Index]=fgetc(ImageDataFile);//得到每个像素的B值
			BufG[Index]=fgetc(ImageDataFile);//得到每个像素的G值
			BufR[Index]=fgetc(ImageDataFile);//得到每个像素的R值
			BufY[Index]=(unsigned char)(0.299*BufR[Index]+0.587*BufG[Index]+0.114*BufB[Index]);//得到灰度分量
		}
		for(j=1;j<=k;j++) fgetc(ImageDataFile);//固定格式，去除多余象素
	}  

	fclose(ImageDataFile);       
	return(1);
}*/

/************************************************************************/
/* 提取图片纹理
/* [in]fname: 文件名称，包括路径
/* [out]Pic_WenLi:提取出的纹理特征向量                                                                     */
/************************************************************************/
int Cooccurrence::OnWenLi(char *fname, Pic_WLType *Pic_WenLi) 
{
	IplImage* imgRgb = cvLoadImage(fname, CV_LOAD_IMAGE_COLOR);
	if (imgRgb == NULL)
	{
		// log error, cannot open file
		return 1;
	}

	unsigned char *imageY = new unsigned char[imgRgb->width * imgRgb->height];
	
	unsigned char b, g, r;
	int max = 0;
	for( int h = 0; h < imgRgb->height; ++h ) {
		for ( int w = 0; w < imgRgb->width * 3; w += 3 ) {
			b  = ((unsigned char*)(imgRgb->imageData + imgRgb->widthStep * h))[w+0];
			g = ((unsigned char*)(imgRgb->imageData + imgRgb->widthStep * h))[w+1];
			r = ((unsigned char*)(imgRgb->imageData + imgRgb->widthStep * h))[w+2];
			imageY[imgRgb->width * h + w / 3] = (unsigned char)(0.299*r + 0.587*g + 0.114*b); //得到灰度分量
			if (max < imgRgb->width * h + w / 3)
			{
				max = imgRgb->width * h + w / 3;
			}
		}
	}

	GetDivision(imgRgb->height, imgRgb->width, imageY, Pic_WenLi); //得到25个分块0,45,90,135度共生矩阵的8个特征值，生成纹理特征
	
	cvReleaseImage( &imgRgb );
	delete[] imageY;
	
	return 0;
}

/************************************************************************/
/* 按分区提取纹理                                                                     */
/************************************************************************/
int Cooccurrence::GetDivision(int Row,int Col,unsigned char *ImageY,struct Pic_WLType *Pic_WenLi)
{
	
	int a=0;
	int Index=0;
	int Col0=Col/5;
    int Row0=Row/5;
	
	int Co_Matrix_0[64][64],Co_Matrix_45[64][64],Co_Matrix_90[64][64],Co_Matrix_135[64][64];
	//0,45,90,135度方向的灰度共生矩阵
    float CCON[4][Block_Total],AASM[4][Block_Total],EENT[4][Block_Total],CCOR[4][Block_Total];
	//25个分块中0,45,90,135度方向的灰度共生矩阵的4个特征量
    float  A_CON[Block_Total],A_ASM[Block_Total],A_ENT[Block_Total],A_COR[Block_Total],T_CON[Block_Total],T_ASM[Block_Total],T_ENT[Block_Total],T_COR[Block_Total];
    //25个分块中从4个特征量提取的均值和方差
	
	int i, j, p, q;
    for(p=0,i=0;i<Block_Num;p=p+Row0,i++)//以0为起点，以Row0为单位纵向扫描全图，Row0为1/5的图高度
	{
		for(q=0,j=0;j<Block_Num;q=q+Col0,j++)//以0为起点，以Col0为单位横向扫描全图，Col0为1/5的图宽度
		{
			Index=(p/Row0)*(Row0*Col)+q;//每个分块的像素起点
			
			GetCoMatrix(Col,Row0,Col0,p,q,Index,ImageY,Co_Matrix_0,Co_Matrix_45,
				Co_Matrix_90,Co_Matrix_135);//求0,45,90,135度方向的共生矩阵
			
			CCON[0][a]=GetCON(Co_Matrix_0);
			AASM[0][a]=GetASM(Co_Matrix_0);
            EENT[0][a]=GetENT(Co_Matrix_0);
			CCOR[0][a]=GetCOR(Co_Matrix_0);
			
			CCON[1][a]=GetCON(Co_Matrix_45);
			AASM[1][a]=GetASM(Co_Matrix_45);
            EENT[1][a]=GetENT(Co_Matrix_45);
			CCOR[1][a]=GetCOR(Co_Matrix_45);
			
			CCON[2][a]=GetCON(Co_Matrix_90);
			AASM[2][a]=GetASM(Co_Matrix_90);
            EENT[2][a]=GetENT(Co_Matrix_90);
			CCOR[2][a]=GetCOR(Co_Matrix_90);
			
			CCON[3][a]=GetCON(Co_Matrix_135);
			AASM[3][a]=GetASM(Co_Matrix_135);
            EENT[3][a]=GetENT(Co_Matrix_135);
			CCOR[3][a]=GetCOR(Co_Matrix_135);
			
			a++;//从0到24
		}
	}//分块特征		    
	
	float total=0;
	
	for(i=0;i<Block_Total;i++)
	{
		for(int j=0;j<4;j++) total=total+CCON[j][i];//求反差的均值和标准差
		A_CON[i]=total/4;
		T_CON[i]=(float)(sqrt(((CCON[0][i]-A_CON[i])*(CCON[0][i]-A_CON[i])
			+(CCON[1][i]-A_CON[i])*(CCON[1][i]-A_CON[i])
			+(CCON[2][i]-A_CON[i])*(CCON[2][i]-A_CON[i])
			+(CCON[3][i]-A_CON[i])*(CCON[3][i]-A_CON[i]))/4));
		
		total=0;
        for(j=0;j<4;j++) total=total+AASM[j][i];//求能量的均值和标准差
		A_ASM[i]=total/4;
		T_ASM[i]=(float)(sqrt(((AASM[0][i]-A_ASM[i])*(AASM[0][i]-A_ASM[i])
			+(AASM[1][i]-A_ASM[i])*(AASM[1][i]-A_ASM[i])
			+(AASM[2][i]-A_ASM[i])*(AASM[2][i]-A_ASM[i])
			+(AASM[3][i]-A_ASM[i])*(AASM[3][i]-A_ASM[i]))/4));
		
		total=0;
		for(j=0;j<4;j++) total=total+EENT[j][i];//求熵的均值和标准差
		A_ENT[i]=total/4;
		T_ENT[i]=(float)(sqrt(((EENT[0][i]-A_ENT[i])*(EENT[0][i]-A_ENT[i])
			+(EENT[1][i]-A_ENT[i])*(EENT[1][i]-A_ENT[i])
			+(EENT[2][i]-A_ENT[i])*(EENT[2][i]-A_ENT[i])
			+(EENT[3][i]-A_ENT[i])*(EENT[3][i]-A_ENT[i]))/4));
		
		total=0;
		for(j=0;j<4;j++) total=total+CCOR[j][i];//求相关的均值和标准差
		A_COR[i]=total/4;
		T_COR[i]=(float)(sqrt(((CCOR[0][i]-A_COR[i])*(CCOR[0][i]-A_COR[i])
			+(CCOR[1][i]-A_COR[i])*(CCOR[1][i]-A_COR[i])
			+(CCOR[2][i]-A_COR[i])*(CCOR[2][i]-A_COR[i])
			+(CCOR[3][i]-A_COR[i])*(CCOR[3][i]-A_COR[i]))/4));
		
	}//分别对25个分块求特征值	
	
	for(i=0;i<Block_Total;i++) // Block_Total + 1 => Block_Total??, tony
	{
		Pic_WenLi->wenli[i][0]=A_CON[i]; Pic_WenLi->wenli[i][1]=T_CON[i];
		Pic_WenLi->wenli[i][2]=A_ASM[i]; Pic_WenLi->wenli[i][3]=T_ASM[i];
		Pic_WenLi->wenli[i][4]=A_ENT[i]; Pic_WenLi->wenli[i][5]=T_ENT[i];
		Pic_WenLi->wenli[i][6]=A_COR[i]; Pic_WenLi->wenli[i][7]=T_COR[i];
	}
	
	return 0;
}

/************************************************************************/
/* 生成共生矩阵                                                                     */
/************************************************************************/
int Cooccurrence::GetCoMatrix(int Col,int RowX,int ColY,int p,int q,unsigned long Index,
				  unsigned char *BufY,int CMatrix_0[64][64],int CMatrix_45[64][64],
				  int CMatrix_90[64][64],int CMatrix_135[64][64])
{   
	int i,j,k,Newlevel,m,n;
	char GrayMatrix[600][500];
	
	for(i=p;i<p+RowX;i++)
	{
		for(j=q;j<q+ColY;j++)
		{   
			k=BufY[Index]%4;
			Newlevel=(BufY[Index]-k)/4;
			GrayMatrix[i][j]=Newlevel;
			Index++;
		}
		Index=Index+(Col-ColY);
	}//生成灰度像素矩阵
	
	for(m=0;m<64;m++)
	{
		for(n=0;n<64;n++)
		{
			CMatrix_0[m][n]=0;
		}
	}//共生矩阵置0

	for(i=p;i<RowX+p;i++)
	{
		for(j=q;j<q+ColY-1;j++)
		{   
			m=GrayMatrix[i][j];
			n=GrayMatrix[i][j+1];
			CMatrix_0[m][n]++;
		}
		for(j=q+ColY-1;j>q;j--)
		{
			m=GrayMatrix[i][j];
			n=GrayMatrix[i][j-1];
			CMatrix_0[m][n]++;
		}
	}//生成0度共生矩阵
	
	for(m=0;m<64;m++)
	{
		for(n=0;n<64;n++)
		{
			CMatrix_45[m][n]=0;
		}
	}//共生矩阵置0

	for(i=p;i<p+RowX-1;i++)
	{
		for(j=q+1;j<q+ColY;j++)
		{   
			m=GrayMatrix[i][j];
			n=GrayMatrix[i+1][j-1];
			CMatrix_45[m][n]++;
		}
	}
	for(i=p+RowX-1;i>p;i--)
	{
		for(j=q;j<q+ColY-1;j++)
		{
			m=GrayMatrix[i][j];
			n=GrayMatrix[i-1][j+1];
			CMatrix_45[m][n]++;
		}
	}//生成45度共生矩阵
	

	for(m=0;m<64;m++)
	{
		for(n=0;n<64;n++)
		{
			CMatrix_90[m][n]=0;
		}
	}//共生矩阵置0

	for(j=q;j<q+ColY;j++)
	{
		for(i=p;i<p+RowX-1;i++)
		{   
			m=GrayMatrix[i][j];
			n=GrayMatrix[i+1][j];
			CMatrix_90[m][n]++;
		}
		for(i=p+RowX-1;i>p;i--)
		{
			m=GrayMatrix[i][j];
			n=GrayMatrix[i-1][j];
			CMatrix_90[m][n]++;
		}
	}//生成90度共生矩阵
	
	for(m=0;m<64;m++)
	{
		for(n=0;n<64;n++)
		{
			CMatrix_135[m][n]=0;
		}
	}//共生矩阵置0

	for(i=p;i<p+RowX-1;i++)
	{
		for(j=q;j<q+ColY-1;j++)
		{   
			m=GrayMatrix[i][j];
			n=GrayMatrix[i+1][j+1];
			CMatrix_135[m][n]++;
		}
	}
	for(i=p+RowX-1;i>p;i--)
	{
		for(j=q+ColY-1;j>q;j--)
		{
			m=GrayMatrix[i][j];
			n=GrayMatrix[i-1][j-1];
			CMatrix_135[m][n]++;
		}
	}//生成135度共生矩阵

	return 0;	
}

/************************************************************************/
/* 求能量                                                                     */
/************************************************************************/
float Cooccurrence::GetASM(int CMatrix[64][64])
{
	int i,j;
	float a=0,p=0;
	for(i=0;i<64;i++)
	{
		for(j=0;j<64;j++)
		{
			p=CMatrix[i][j]+p;
		}
	}
	for(i=0;i<64;i++)
	{
		for(j=0;j<64;j++)
		{
			a=(CMatrix[i][j]/p)*(CMatrix[i][j]/p)+a;
		}
	}
	return(a);
}

/************************************************************************/
/* 求熵                                                                     */
/************************************************************************/
float Cooccurrence::GetENT(int CMatrix[64][64])
{
	int i,j;
	float e=0,p=0;
	for(i=0;i<64;i++)
	{
		for(j=0;j<64;j++)
		{
			p=CMatrix[i][j]+p;
		}
	}
	for(i=0;i<64;i++)
	{
		for(j=0;j<64;j++)
		{   
			if(CMatrix[i][j]==0)
				CMatrix[i][j]=1;
			e=(float)((CMatrix[i][j]/p)*(log10(CMatrix[i][j]/p)/0.30103)+e);
		}
	}

	e=e*(-1);
	return(e);
}

/************************************************************************/
/* 求相关                                                                     */
/************************************************************************/
float Cooccurrence::GetCOR(int CMatrix[64][64])
{
	int i,j;
	float R,r=0,p=0,u1=0,u2=0,v1=0,v2=0,m=0,n=0;
	for(i=0;i<64;i++)
	{
		for(j=0;j<64;j++)
		{
			p=CMatrix[i][j]+p;
		}
	}
	
	for(i=0;i<64;i++) 
	{
		m=(float)(i+1);
		for(j=0;j<64;j++)
		{
			n=(CMatrix[i][j]/p)+n;
		}
		u1=m*n+u1;
		n=0;
	}
	m=0;	
	
	for(j=0;j<64;j++) 
	{
		m=(float)(j+1);
		for(i=0;i<64;i++)
		{
			n=(CMatrix[i][j]/p)+n;
		}
		u2=m*n+u2;
		n=0;
	}
	m=0;	

	for(i=0;i<64;i++) 
	{
		m=i+1-u1;
		for(j=0;j<64;j++)
		{
			n=(CMatrix[i][j]/p)+n;
		}
		v1=m*m*n+v1;
		n=0;
	}
	m=0;
	
	for(j=0;j<64;j++) 
	{
		m=j+1-u2;
		for(i=0;i<64;i++)
		{
			n=(CMatrix[i][j]/p)+n;
		}
		v2=m*m*n+v2;
		n=0;
	}
	m=0;
	
	for(i=0;i<64;i++)
	{
		for(j=0;j<64;j++)
		{
			r=(i+1)*(j+1)*(CMatrix[i][j]/p)+r;
		}
	}
	R=(r-u1*u2)/(v1*v2);
	return(R);
}

/************************************************************************/
/* 求反差                                                                     */
/************************************************************************/
float Cooccurrence::GetCON(int CMatrix[64][64])
{
	int i,j;
	float c=0,p=0;
	for(i=0;i<64;i++)
	{
		for(j=0;j<64;j++)
		{
			p=CMatrix[i][j]+p;
		}
	}
	for(i=0;i<64;i++)
	{
		for(j=0;j<64;j++)
		{
			c=(i-j)*(i-j)*(CMatrix[i][j]/p)+c;
		}
	}
	return(c);
}

/************************************************************************/
/* 检索图片纹理
/* [in]model_WL: 关键图的纹理特征向量
/* [in]Pic_WL: 待检图片集的特征向量数组
/* [in]fnum: 待检图片数
/* [out]sort: 根据相似度得到的待检图片集相似度排序                                                                          */
/************************************************************************/
int Cooccurrence::OnRetrieveIn(Pic_WLType *model_WL, Pic_WLType *Pic_WL, int fnum, int *sort) 
{
    float *Dis_YS = (float *)malloc(fnum * sizeof(float));//基于纹理特征的距离
	float *Dis_YS0 = (float *)malloc(fnum * sizeof(float));//归一化后的纹理特征的距离
	
	int i = 0;
	for (i  = 0; i < fnum; ++i)
	{
		GetDistance(*model_WL, Pic_WL[i], &(Dis_YS[i]));//基于纹理特征的距离	
		//printf("%d 计算完距离\n", i);
	}
	GuiYiHua(Dis_YS, Dis_YS0, fnum);//纹理距离的归一化
	
	for(i=0;i<fnum;i++)   
		sort[i]=i;
    QSort(Dis_YS0,0,fnum-1,sort);//求得的相似距离快速排序

	free(Dis_YS);
	free(Dis_YS0);
	return(1);
}

/************************************************************************/
/* 计算两张图片之间的相似距离                                                                     */
/************************************************************************/
int Cooccurrence::GetDistance(Pic_WLType model_WL, Pic_WLType Pic_WL, float *Dis_WL)
{  
	float TempBlock[Block_Total];
	float Temp;
	int j,r;
	
	for(Temp=0,j=0;j<Block_Total;j++)
	{
		float total=0;
		for(r=0;r<BLOCK_TEXTURE_SIZE;r++) // 8个特征向量
			total+=(model_WL.wenli[j][r]-Pic_WL.wenli[j][r])*(model_WL.wenli[j][r]-Pic_WL.wenli[j][r]);
		
		TempBlock[j]=(float)sqrt(total);	
		Temp += TempBlock[j];//可以设置每个分块的权值，计算总的相似距离
	}
	*Dis_WL = Temp;//求所有分块的纹理加权和距离
	return(1);
}

/************************************************************************/
/* 求归一化数值                                                                     */
/************************************************************************/
int Cooccurrence::GuiYiHua(float *Dis_old,float *Dis_new, int fnum)
{
	double total=0;
	int i;
	float T_ave,T_bzc;//均值和标准差
	
	for(i=0;i<fnum;i++)   
		total+= Dis_old[i];
	T_ave=(float)total/fnum;//均值
	
	total=0;
	for(i=0;i<fnum;i++)
		total+=(Dis_old[i]-T_ave)*(Dis_old[i]-T_ave);
    T_bzc=(float)sqrt(total/fnum);//标准差
	
	for(i=0;i<fnum;i++)
	{
		Dis_new[i]=(float)(Dis_old[i]-T_ave)/(3*T_bzc); // 如果所有值都相等, 那么T_bzc=0, 出错??? --tony
		Dis_new[i]=(Dis_new[i]+1)/2;
        if(Dis_new[i]<0) Dis_new[i]=0;
		if(Dis_new[i]>1) Dis_new[i]=1;//归一化到(0,1)
	}//高斯归一化
	
	return 1;
}


/************************************************************************/
/* 对序列作快速排序                                                                     */
/************************************************************************/
int Cooccurrence::QSort(float *List, int low, int high, int *sort)
{
	if(low<high)//长度大于1
	{
		int pivotloc=Partition(List,low,high,sort);//将序列一分为二
		QSort(List,low,pivotloc-1,sort);//对低子表递归排序，pivotloc是枢轴位置
		QSort(List,pivotloc+1,high,sort);//对高子表递归排序
	}
	return(1);
}

/************************************************************************/
/* 将序列分成两段，前一段均小于枢轴记录，后一段大于枢轴记录                                                                     */
/************************************************************************/
int Cooccurrence::Partition(float *List,int low,int high,int *sort)
{
	float pivotkey=List[low];
	int z;
	z=sort[low];//用子表的第一个记录做枢轴记录
	
	while(low<high)
	{   
		while((low<high)&&(List[high]>=pivotkey)) 
		{ 
			--high;
		}
		if(low!=high)
		{ 
			List[low]=List[high];
			sort[low]=sort[high];
		}//将比枢轴记录小的记录移到低端
		
		while((low<high)&&(List[low]<=pivotkey)) 
		{   
			++low;
		}
		if(low!=high)
		{
			List[high]=List[low];
			sort[high]=sort[low];
		}//将比枢轴记录大的记录移到高端
	}
	List[low]=pivotkey;
	sort[low]=z;
	return low;//返回枢轴记录
	
}
