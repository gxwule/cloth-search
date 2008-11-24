/*
 * MATLAB Compiler: 3.0
 * Date: Sun Nov 23 22:22:32 2008
 * Arguments: "-B" "macro_default" "-O" "all" "-O" "fold_scalar_mxarrays:on"
 * "-O" "fold_non_scalar_mxarrays:on" "-O" "optimize_integer_for_loops:on" "-O"
 * "array_indexing:on" "-O" "optimize_conditionals:on" "-v" "-B" "sgl" "-m"
 * "-W" "main" "-L" "C" "-t" "-T" "link:exe" "-h" "libmmfile.mlib" "-W"
 * "mainhg" "libmwsglm.mlib" "-t" "-W" "lib:gaborfilterdll" "-T" "link:lib"
 * "gaborfilter.m" 
 */

#ifndef MLF_V2
#define MLF_V2 1
#endif

#ifndef __gaborfilterdll_h
#define __gaborfilterdll_h 1

#ifdef __cplusplus
extern "C" {
#endif

#include "libmatlb.h"

extern void InitializeModule_gaborfilterdll(void);
extern void TerminateModule_gaborfilterdll(void);
extern mxArray * mlfGaborfilter(mxArray * pic, mxArray * G);
extern void mlxGaborfilter(int nlhs,
                           mxArray * plhs[],
                           int nrhs,
                           mxArray * prhs[]);
extern mxArray * mlfNImread(int nargout,
                            mxArray * * map,
                            mxArray * * alpha,
                            ...);
extern mxArray * mlfImread(mxArray * * map, mxArray * * alpha, ...);
extern void mlfVImread(mxArray * synthetic_varargin_argument, ...);
extern void mlxImread(int nlhs, mxArray * plhs[], int nrhs, mxArray * prhs[]);
extern mxArray * mlfRgb2gray(mxArray * r, mxArray * g, mxArray * b);
extern void mlxRgb2gray(int nlhs, mxArray * plhs[], int nrhs, mxArray * prhs[]);
extern mxArray * mlfIm2double(mxArray * img, mxArray * typestr);
extern void mlxIm2double(int nlhs,
                         mxArray * plhs[],
                         int nrhs,
                         mxArray * prhs[]);
extern void mlfImages_private_checkinput(mxArray * A,
                                         mxArray * classes,
                                         mxArray * attributes,
                                         mxArray * function_name,
                                         mxArray * variable_name,
                                         mxArray * argument_position);
extern void mlxImages_private_checkinput(int nlhs,
                                         mxArray * plhs[],
                                         int nrhs,
                                         mxArray * prhs[]);
extern void mlfImages_private_checknargin(mxArray * low,
                                          mxArray * high,
                                          mxArray * numInputs,
                                          mxArray * function_name);
extern void mlxImages_private_checknargin(int nlhs,
                                          mxArray * plhs[],
                                          int nrhs,
                                          mxArray * prhs[]);
extern mxArray * mlfImages_private_checkstrs(mxArray * in,
                                             mxArray * valid_strings,
                                             mxArray * function_name,
                                             mxArray * variable_name,
                                             mxArray * argument_position);
extern void mlxImages_private_checkstrs(int nlhs,
                                        mxArray * plhs[],
                                        int nrhs,
                                        mxArray * prhs[]);
extern mxArray * mlfImages_private_num2ordinal(mxArray * number);
extern void mlxImages_private_num2ordinal(int nlhs,
                                          mxArray * plhs[],
                                          int nrhs,
                                          mxArray * prhs[]);
extern mxArray * mlfNStrread(int nargout, mlfVarargoutList * varargout, ...);
extern mxArray * mlfStrread(mlfVarargoutList * varargout, ...);
extern void mlfVStrread(mxArray * synthetic_varargin_argument, ...);
extern void mlxStrread(int nlhs, mxArray * plhs[], int nrhs, mxArray * prhs[]);
extern mxArray * mlfNDataread(int nargout, mlfVarargoutList * varargout, ...);
extern mxArray * mlfDataread(mlfVarargoutList * varargout, ...);
extern void mlfVDataread(mxArray * synthetic_varargin_argument, ...);
extern void mlxDataread(int nlhs, mxArray * plhs[], int nrhs, mxArray * prhs[]);
extern void gaborfilterdllInitialize(void);
extern void gaborfilterdllTerminate(void);

#ifdef __cplusplus
}
#endif

#endif
