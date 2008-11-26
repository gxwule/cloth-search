#pragma once

#define DIM 8

// Initialize luv[][], LLindex[], Lindex[][], matRGBtoXYZ[3][3],Wx,Wy,Wz,u0,v0
// It should be called before get_waveletfeature.
// It can be called just once before several "get_waveletfeature"
// Return 
bool luv_init(const char* luvFile);

// the feature should be allocated and freed out of the method.
bool get_waveletfeature(const char* filename, float* feature);

