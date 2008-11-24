#include "stdafx.h"
#include "Gabor.h"
#include "matlab.h"
#include "math.h"
#include "gaborkerneldll.h"
#include "gaborfilterdll.h"

Gabor::Gabor(int freq, int ori) : frequency(freq), oriention(ori)
{
	gaborkerneldllInitialize();
	gaborfilterdllInitialize();
	
	// 初始化并计算核函数
	Init();
}

int Gabor::Init()
{
	int row = 0, col = 0;
	int i = 0;
	// 生成一组核函数
	for (int j = 0; j < frequency * 2; j+=2)
	{
		for (int k = 0; k < oriention; k+=1)
		{
			CalculateKernel(2, 4, j, k, i, &row, &col);	
			//printf("%d kernel generated\n", i);
			++i;			
		}
	}
/*
	CalculateKernel(2, 4, 16, 1.03, i, &row, &col);	
	printf("%d kernel generated\n", i);
	++i;
*/
	
	return 1;
}

Gabor::~Gabor()
{
	// free kernel function array
	for (int i = 0; i < KERNEL_NUM; ++i)
	{
		mxDestroyArray(kernel[i]);
	}
	gaborkerneldllTerminate();
	gaborfilterdllTerminate();
}

/************************************************************************/
/* 生成核函数 
/* Sx & Sy : Variances along x and y-axes respectively                    
/* f : The frequency of the sinusoidal function
/* theta : The orientation of Gabor filter
/************************************************************************/
int Gabor::CalculateKernel(double Sx, double Sy, double f, double theta, int index, int *row, int *col) 
{
	mxArray *ax, *bx, *cx, *dx, *g;
	
	mlfAssign(&ax,mlfScalar(Sx)); 
	mlfAssign(&bx,mlfScalar(Sy));
	mlfAssign(&cx,mlfScalar(f));
	mlfAssign(&dx,mlfScalar(theta));
	g=mlfGaborkernel(ax,bx,cx,dx);
	
	double *doubleG = (double *)mxGetData(g);
	*row = mxGetM(g);
	*col = mxGetN(g);

	mxArray *k = mxCreateDoubleMatrix(*row, *col, mxREAL);
	double *data = mxGetPr(k);
	int total = (*row) * (*col);
	for (int i = 0; i < total; ++i)
	{
		data[i] = doubleG[i];
		
	}

	kernel[index] = k;
	
	mxDestroyArray(ax);
	mxDestroyArray(bx);
	mxDestroyArray(cx);
	mxDestroyArray(dx);
	mxDestroyArray(g);

	return(1);
}

/************************************************************************/
/* 提取图片纹理
/* [in]fname: 图片文件名，包括路径
/* [out]pic_gaborWL: 提取出的特征向量                                                                     */
/************************************************************************/
int Gabor::OnWenLi(char *fname, Pic_GaborWL *pic_gaborWL)
{	
	int index = 0;
	for (int i = 0; i < KERNEL_NUM; ++i)
	{
		int row = mxGetM(kernel[i]);
		int col = mxGetN(kernel[i]);
		mxArray *k = mxCreateDoubleMatrix(row, col, mxREAL);
		double *data = mxGetPr(k);
		double * kernelData = (double *)mxGetData(kernel[i]);
		int total = (row) * (col);
		int j = 0;
		for (j = 0; j < total; ++j)
		{
			data[j] = kernelData[j];			
		}
		mxArray *pic = mxCreateString(fname);
		// 将图片进行Gabor滤波
		mxArray *gabout=mlfGaborfilter(pic, k);
		mxDestroyArray(k);
		mxDestroyArray(pic);

		double * result = (double *)mxGetData(gabout);

		row = mxGetM(gabout);
		col = mxGetN(gabout);
		total = row * col; // shit,此处发现一个低级错误，留念

		// 计算均值和方差
		double sum = 0;
		for (j = 0; j < total; ++j)
		{
			sum += result[j];
		}
		double avg = sum / total;

		double deta = 0;
		for (j = 0; j < total; ++j)
		{
			deta += (result[j] - avg) * (result[j] - avg);
		}
		deta = deta / total;

		pic_gaborWL->wenli[index++] = avg;
		pic_gaborWL->wenli[index++] = deta;

		mxDestroyArray(gabout);
	}
	
	return 0;
}

/************************************************************************/
/* 检索图片纹理
/* [in]model_WL: 关键图的纹理特征向量
/* [in]Pic_WL: 待检图片集的特征向量数组
/* [in]fnum: 待检图片数
/* [out]sort: 根据相似度得到的待检图片集相似度排序                                                                     */
/************************************************************************/
int Gabor::OnRetrieveIn(Pic_GaborWL *model_WL, Pic_GaborWL *Pic_WL, int fnum, int *sort) 
{
    double *Dis_YS = (double *)malloc(fnum * sizeof(double));//基于纹理特征的距离
	double *Dis_YS0 = (double *)malloc(fnum * sizeof(double));//归一化后的纹理特征的距离
	
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
int Gabor::GetDistance(Pic_GaborWL model_WL, Pic_GaborWL Pic_WL, double *Dis_WL)
{  
	
	double Temp = 0;
	int r;
	
	double total=0;
	for(r=0;r<GABOR_TEXTURE_SIZE;r++)
		total+=(model_WL.wenli[r]-Pic_WL.wenli[r])*(model_WL.wenli[r]-Pic_WL.wenli[r]);
	
	Temp=(double)sqrt(total);

	*Dis_WL = Temp;
	return(1);
}

/************************************************************************/
/* 求归一化数值                                                                     */
/************************************************************************/
int Gabor::GuiYiHua(double *Dis_old,double *Dis_new, int fnum)
{
	double total=0;
	int i;
	double T_ave,T_bzc;//均值和标准差
	
	for(i=0;i<fnum;i++)   
		total+= Dis_old[i];
	T_ave=(double)total/fnum;//均值
	
	total=0;
	for(i=0;i<fnum;i++)
		total+=(Dis_old[i]-T_ave)*(Dis_old[i]-T_ave);
    T_bzc=(double)sqrt(total/fnum);//标准差
	
	for(i=0;i<fnum;i++)
	{
		Dis_new[i]=(double)(Dis_old[i]-T_ave)/(3*T_bzc);
		Dis_new[i]=(Dis_new[i]+1)/2;
        if(Dis_new[i]<0) Dis_new[i]=0;
		if(Dis_new[i]>1) Dis_new[i]=1;//归一化到(0,1)
	}//高斯归一化
	
	return 1;
}


/************************************************************************/
/* 对序列作快速排序                                                                     */
/************************************************************************/
int Gabor::QSort(double *List, int low, int high, int *sort)
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
int Gabor::Partition(double *List,int low,int high,int *sort)
{
	double pivotkey=List[low];
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