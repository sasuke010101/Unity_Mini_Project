using UnityEngine;
using System.Collections;

//一个贝赛尔曲线的取点方法,
public class myBezier
{
	public Vector3 p0;
	public Vector3 p1;
	public Vector3 p2;
	public Vector3 p3;
		
	private float Ax;
	private float Ay;		
	private float Az;		
	private float Bx;		
	private float By;		
	private float Bz;		
	private float Cx;		
	private float Cy;
	private float Cz;
		
	// Init function v0 = 1st point, v1 = handle of the 1st point , v2 = handle of the 2nd point, v3 = 2nd point		
	// handle1 = v0 + v1		
	// handle2 = v3 + v2
		
	public myBezier( Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3 )
	{			
		this.p0 = v0;
		this.p1 = v1;
		this.p2 = v2;
		this.p3 = v3;

		SetConstant ();
	}

	//用pointnum个点描述这条曲线,返回这若干个点
	public Vector3[] GetPoints(int pointnum=100)
	{
		if (pointnum < 10) 
		{
			pointnum = 10;
		}
		if(pointnum > 500)
		{
			pointnum = 500;
		}
		Vector3[] tmp = new Vector3[pointnum];

		for(int i=0;i<pointnum;i++)
		{
			tmp[i] = GetPointAtTime((i+0.0f)/(pointnum-1));
		}

		return tmp;
	}
		
	// 0.0 >= t <= 1.0
	private Vector3 GetPointAtTime( float t )
	{			
		float t2 = t * t;			
		float t3 = t * t * t;			
		float x = this.Ax * t3 + this.Bx * t2 + this.Cx * t + p0.x;			
		float y = this.Ay * t3 + this.By * t2 + this.Cy * t + p0.y;			
		float z = this.Az * t3 + this.Bz * t2 + this.Cz * t + p0.z;			
		return new Vector3( x, y, z);		

		//x(t) = ax * t ^ 3 + bx * t ^ 2 + cx * t + x0
		//y(t) = ay * t ^ 3 + by * t ^ 2 + cy * t + y0
	}
		
	private void SetConstant()			
	{
		this.Cx = 3f * ( this.p1.x - this.p0.x );			
		this.Bx = 3f * ( this.p2.x - this.p1.x ) - this.Cx;			
		this.Ax = this.p3.x - this.p0.x - this.Cx - this.Bx;	

		this.Cy = 3f * ( this.p1.y - this.p0.y );			
		this.By = 3f * ( this.p2.y - this.p1.y ) - this.Cy;			
		this.Ay = this.p3.y - this.p0.y - this.Cy - this.By;	

		this.Cz = 3f * ( this.p1.z - this.p0.z );
		this.Bz = 3f * ( this.p2.z - this.p1.z ) - this.Cz;
		this.Az = this.p3.z - this.p0.z - this.Cz - this.Bz;	

		//cx = 3 * ( x1 - x0 )
		//bx = 3 * ( x2 - x1 ) - cx
		//ax = x3 - x0 - cx - bx
				
		//cy = 3 * ( y1 - y0 )
		//by = 3 * ( y2 - y1 ) - cy
		//ay = y3 - y0 - cy - by
	}

}