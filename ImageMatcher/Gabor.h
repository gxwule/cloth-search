#pragma once

#include "stdafx.h"
#include "matlab.h"

#define FREQ_NUM 4 // 可调的频率数
#define ORIE_NUM 2 // 可调的方向数
#define KERNEL_NUM (FREQ_NUM * ORIE_NUM) // 核函数总数
#define GABOR_TEXTURE_SIZE (KERNEL_NUM * 2)

/************************************************************************/
/* Gabor滤波纹理提取和检索                                                                     */
/************************************************************************/
class Gabor
{
public:
	typedef struct Pic_GaborWL
	{
		double wenli[GABOR_TEXTURE_SIZE];
	} Pic_GaborWL; // 每个图片的纹理特征向量存储单位，一个核函数产生2个向量
private:
	mxArray *kernel[KERNEL_NUM]; // 核函数
	int frequency;
	int oriention;
public:
	int OnWenLi(const char *fname, Pic_GaborWL *pic_gaborWL);
	int OnRetrieveIn(Pic_GaborWL *model_WL, Pic_GaborWL *Pic_WL, int fnum, int *sort);
	// Current not support customize these two parameters, actually.
	Gabor(int frequency=FREQ_NUM, int oriention=ORIE_NUM);
	~Gabor();

private:
	int Init();
	int CalculateKernel(double Sx, double Sy, double f, double theta, int index, int *row, int *col);
	int GetDistance(Pic_GaborWL model_WL, Pic_GaborWL Pic_WL, double *Dis_WL);
	int GuiYiHua(double *Dis_old,double *Dis_new, int fnum);
	int QSort(double *List, int low, int high, int *sort);
	int Partition(double *List,int low,int high,int *sort);
};