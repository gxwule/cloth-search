/*
 * MATLAB Compiler: 3.0
 * Date: Sun Nov 23 14:36:05 2008
 * Arguments: "-B" "macro_default" "-O" "all" "-O" "fold_scalar_mxarrays:on"
 * "-O" "fold_non_scalar_mxarrays:on" "-O" "optimize_integer_for_loops:on" "-O"
 * "array_indexing:on" "-O" "optimize_conditionals:on" "-v" "-t" "-W"
 * "lib:gaborkerneldll" "-T" "link:lib" "gaborkernel.m" 
 */

#ifndef MLF_V2
#define MLF_V2 1
#endif

#ifndef __gaborkerneldll_h
#define __gaborkerneldll_h 1

#ifdef __cplusplus
extern "C" {
#endif

#include "libmatlb.h"

extern void InitializeModule_gaborkerneldll(void);
extern void TerminateModule_gaborkerneldll(void);
extern mxArray * mlfGaborkernel(mxArray * Sx,
                                mxArray * Sy,
                                mxArray * f,
                                mxArray * theta);
extern void mlxGaborkernel(int nlhs,
                           mxArray * plhs[],
                           int nrhs,
                           mxArray * prhs[]);
extern void gaborkerneldllInitialize(void);
extern void gaborkerneldllTerminate(void);

#ifdef __cplusplus
}
#endif

#endif
