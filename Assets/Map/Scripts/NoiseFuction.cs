using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NoiseFuction {

	float persistence = 1f;  
	int Number_Of_Octaves =9;  

	double Noise(int x,int y)    // 根据(x,y)获取一个初步噪声值  
	{  
		int n = x + y * 57;    
		n = (n<<13) ^ n;  
		return ( 1.0 - ( (n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);  
	}  

	double SmoothedNoise(int x, int y)   //光滑噪声  
	{  
		double corners = ( Noise(x-1, y-1)+Noise(x+1, y-1)+Noise(x-1, y+1)+Noise(x+1, y+1) ) / 16;  
		double sides = ( Noise(x-1, y) +Noise(x+1, y) +Noise(x, y-1) +Noise(x, y+1) ) / 8;  
		double center = Noise(x, y) / 4;  
		return corners + sides + center;  
	}  
	double Cosine_Interpolate(double a,double b, double x)  // 余弦插值  
	{  
		double ft = x * 3.1415927;  
		double f = (1 - Math.Cos(ft)) * 0.5;  
		return a*(1-f) + b*f;  
	}  

	double InterpolatedNoise(float x,float y)   // 获取插值噪声  
	{  
		int integer_X = (int)x;  
		float  fractional_X = x - integer_X;  
		int integer_Y = (int)y;  
		float fractional_Y = y - integer_Y;  
		double v1 = SmoothedNoise(integer_X, integer_Y);  
		double v2 = SmoothedNoise(integer_X + 1, integer_Y);  
		double v3 = SmoothedNoise(integer_X, integer_Y + 1);  
		double v4 = SmoothedNoise(integer_X + 1, integer_Y + 1);  
		double i1 = Cosine_Interpolate(v1, v2, fractional_X);  
		double i2 = Cosine_Interpolate(v3, v4, fractional_X);
		return Cosine_Interpolate(i1, i2, fractional_Y);  
	}  

	public float PerlinNoise(float x,float y)    // 最终调用：根据(x,y)获得其对应的PerlinNoise值  
	{  
		double total = 0;  
		double p = persistence;  
		int n = Number_Of_Octaves;  

		for(int i=0; i<n; i++)  
		{  
			double frequency = Math.Pow(2,i);  
			double amplitude = Math.Pow(p,i);
			total = total + InterpolatedNoise((float)(x * frequency), (float)(y * frequency)) * amplitude;  
		}
		return (float)total+2.1f;  
	}  
}
