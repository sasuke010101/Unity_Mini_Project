using UnityEngine;
using System.Collections;

//这是由无数长方形组成的线,最后合成一个mesh,显示为一条线 (隔一个留一个长方形就是虚线 隔X个也行,自己改代码)
public class MeshDashedLine : MonoBehaviour {

	//w线的宽,points是一条理论上的线,由连续的点组成(比如一个berzier串) ,uv是否计算uv ,dashed是实线还是虚线
	public static Mesh CreateMesh(float w,Vector3[] points ,bool uv,bool dashed)
	{
		Mesh ms = new Mesh (); 
		ms.name = "MDLine";
		ms.vertices = getPoints(points,w);
		ms.triangles = getTri(ms.vertices.Length,dashed);
		ms.normals = getNor (ms.vertices.Length);
		if(uv)
		{
			ms.uv = getUV (ms.vertices);
		}
		else
		{
			ms.uv = new Vector2[ms.vertices.Length];
		}
		return ms;
	}
	
	static Vector3[] getNor(int len)
	{
		Vector3[] ret = new Vector3[len];
		for(int i=0;i<len;i++)
		{
			ret[i].x = 0;
			ret[i].y = 0;
			ret[i].z = -1;
		}
		return ret;
	}
	
	//len是顶点的数目
	static int[] getTri(int len,bool dashed)
	{
		if(dashed) //实线
		{
			int num = len*3;
			int[] ret = new int[num]; //三角个数正好是点数减2    *3是三角   *2是双面
			
			int j = 0;
			for(int i=0;i<len;i+=4,j+=6)
			{
				ret[j+0]=i+0;
				ret[j+1]=i+1;
				ret[j+2]=i+2;
				
				ret[j+3]=i+0;
				ret[j+4]=i+2;
				ret[j+5]=i+3;
			}
			
			//反面  //TMD 点序不对,法线自动被翻了, 所以用双面
			for(int i=0;i<len;i+=4,j+=6)
			{
				ret[j+0]=i+2;
				ret[j+1]=i+1;
				ret[j+2]=i+0;
				
				ret[j+3]=i+3;
				ret[j+4]=i+2;
				ret[j+5]=i+0;
			}
			return ret;
		}
		else
		{
			int num = len/2*3;
			int[] ret = new int[num]; //三角个数正好是点数减2    *3是三角   *2是双面
			
			int j = 0;
			for(int i=0;i<len;i+=8,j+=6)
			{
				ret[j+0]=i+0;
				ret[j+1]=i+1;
				ret[j+2]=i+2;
				
				ret[j+3]=i+0;
				ret[j+4]=i+2;
				ret[j+5]=i+3;
			}
			
			//反面  //TMD 点序不对,法线自动被翻了, 所以用双面
			for(int i=0;i<len;i+=8,j+=6)
			{
				ret[j+0]=i+2;
				ret[j+1]=i+1;
				ret[j+2]=i+0;
				
				ret[j+3]=i+3;
				ret[j+4]=i+2;
				ret[j+5]=i+0;
			}
			return ret;
		}
	}
	
	//计算UV,参数传入顶点
	//这个UV是全图的, 也可以改成一个小方格0-1,这样每个方格一个圆点图,校果也不错
	static Vector2[] getUV(Vector3[] vertices)
	{
		float minx = 100000000;
		float maxx = -100000000;
		float miny = 100000000;
		float maxy = -100000000;
		for(int i=0;i<vertices.Length;i++)
		{
			if(vertices[i].x < minx)
			{
				minx = vertices[i].x;
			}
			if(vertices[i].x > maxx)
			{
				maxx = vertices[i].x;
			}
			if(vertices[i].y < miny)
			{
				miny = vertices[i].y;
			}
			if(vertices[i].y > maxy)
			{
				maxy = vertices[i].y;
			}
		}
		
		float dx = maxx - minx;
		float dy = maxy - miny;
		Vector2[] ret = new Vector2[vertices.Length];
		for(int i=0;i<vertices.Length;i++)
		{
			ret[i].x = (vertices[i].x - minx)/dx;
			ret[i].y = (vertices[i].y - miny)/dy;
		}
		return ret;
	}
	
	//通过一条线 ,得到包围这条线的mesh点 (小线断直角,小长方) 
	static Vector3[] getPoints(Vector3[] inpoints,float w)
	{
		if(inpoints.Length<2)
		{
			return null;
		}
		Vector3[] lefts = new Vector3[inpoints.Length*4];//inpoints点左侧的所有点
		
		//(除最后一个)
		if(true)
		{
			int j=0;
			for(int i=0;i<(inpoints.Length-1);i++)
			{
				lefts[j++] = getA(inpoints[i],inpoints[i+1],w);
				lefts[j++] = getB(inpoints[i],inpoints[i+1],w);
				lefts[j++] = getC(inpoints[i],inpoints[i+1],w);
				lefts[j++] = getD(inpoints[i],inpoints[i+1],w);
			}
		}
		
		return lefts;
	}

	//                   |
	//                   |
	//                   |
	//                   .A          .D
	//                   |
	//   ________________|Center_____._forwardPoint______>
	//                   |
	//                   |
	//                   .B          .C
	//                   |
	//                   |

	
	//以Center为原点,forwordport为正方向,的坐标系中, 画一个方形,反回左上角坐标
	public static Vector3 getA (Vector3 Center,Vector3 forwardPoint,float w)
	{
		//Vector3.Angle 一二像限正常  Vector3.Angle 三四像限 

		//vector.angle是角度不是弧度,坑了我一天
		
		//1像限
		if(Center.x <= forwardPoint.x && Center.y <= forwardPoint.y)
		{
			float ang = Vector3.Angle(forwardPoint-Center,Vector3.right);
			ang = ang/180*Mathf.PI;
			// x = x1 cosa - y1 sina + a
			// y = y1 cosa + x1 sina + b
			
			//新坐标 (0,-w)
			float x = (0) * Mathf.Cos (ang) - (w/2) * Mathf.Sin (ang) + Center.x;
			float y = (w/2)*Mathf.Cos(ang) + (0)*Mathf.Sin(ang) + Center.y;
			
			return new Vector3 (x,y,0);
		}
		//2像限
		else if(Center.x >= forwardPoint.x && Center.y <= forwardPoint.y)
		{
			float ang = Vector3.Angle(forwardPoint-Center,Vector3.right);
			ang = ang/180*Mathf.PI;
			// x = x1 cosa - y1 sina + a
			// y = y1 cosa + x1 sina + b
			
			//新坐标 (0,-w)
			float x = (0) * Mathf.Cos (ang) - (w/2) * Mathf.Sin (ang) + Center.x;
			float y = (w/2)*Mathf.Cos(ang) + (0)*Mathf.Sin(ang) + Center.y;
			
			return new Vector3 (x,y,0);
		}
		//3像限
		else if(Center.x >= forwardPoint.x && Center.y >= forwardPoint.y)
		{			
			float ang = Vector3.Angle(forwardPoint-Center,Vector3.right);
			//Debug.Log(ang);
			ang = (360-ang)/180*Mathf.PI;
			// x = x1 cosa - y1 sina + a
			// y = y1 cosa + x1 sina + b
			
			//新坐标 (0,-w)
			float x = (0) * Mathf.Cos (ang) - (w/2) * Mathf.Sin (ang) + Center.x;
			float y = (w/2)*Mathf.Cos(ang) + (0)*Mathf.Sin(ang) + Center.y;
			
			return new Vector3 (x,y,0);
			
		}
		//4像限
		else
		{
			float ang = Vector3.Angle(forwardPoint-Center,Vector3.right);
			//Debug.Log(ang);
			ang = (360-ang)/180*Mathf.PI;
			// x = x1 cosa - y1 sina + a
			// y = y1 cosa + x1 sina + b
			
			//新坐标 (0,-w)
			float x = (0) * Mathf.Cos (ang) - (w/2) * Mathf.Sin (ang) + Center.x;
			float y = (w/2)*Mathf.Cos(ang) + (0)*Mathf.Sin(ang) + Center.y;
			
			return new Vector3 (x,y,0);
		}
	}
	//以Center为原点,forwordport为方向,的坐标系中, 画一个方形,反回左下角坐标
	public static Vector3 getB (Vector3 Center,Vector3 forwardPoint,float w)
	{
		//Vector3.Angle 一二像限正常  Vector3.Angle 三四像限 
		
		//1像限
		if(Center.x <= forwardPoint.x && Center.y <= forwardPoint.y)
		{
			float ang = Vector3.Angle(forwardPoint-Center,Vector3.right);
			ang = ang/180*Mathf.PI;
			// x = x1 cosa - y1 sina + a
			// y = y1 cosa + x1 sina + b
			
			//新坐标 (0,-w)
			float x = (0) * Mathf.Cos (ang) - (-w/2) * Mathf.Sin (ang) + Center.x;
			float y = (-w/2)*Mathf.Cos(ang) + (0)*Mathf.Sin(ang) + Center.y;
			
			return new Vector3 (x,y,0);
		}
		//2像限
		else if(Center.x >= forwardPoint.x && Center.y <= forwardPoint.y)
		{
			float ang = Vector3.Angle(forwardPoint-Center,Vector3.right);
			ang = ang/180*Mathf.PI;
			// x = x1 cosa - y1 sina + a
			// y = y1 cosa + x1 sina + b
			
			//新坐标 (0,-w)
			float x = (0) * Mathf.Cos (ang) - (-w/2) * Mathf.Sin (ang) + Center.x;
			float y = (-w/2)*Mathf.Cos(ang) + (0)*Mathf.Sin(ang) + Center.y;
			
			return new Vector3 (x,y,0);
		}
		//3像限
		else if(Center.x >= forwardPoint.x && Center.y >= forwardPoint.y)
		{			
			float ang = Vector3.Angle(forwardPoint-Center,Vector3.right);
			//Debug.Log(ang);
			ang = (360-ang)/180*Mathf.PI;
			// x = x1 cosa - y1 sina + a
			// y = y1 cosa + x1 sina + b
			
			//新坐标 (0,-w)
			float x = (0) * Mathf.Cos (ang) - (-w/2) * Mathf.Sin (ang) + Center.x;
			float y = (-w/2)*Mathf.Cos(ang) + (0)*Mathf.Sin(ang) + Center.y;
			
			return new Vector3 (x,y,0);
			
		}
		//4像限
		else
		{
			float ang = Vector3.Angle(forwardPoint-Center,Vector3.right);
			//Debug.Log(ang);
			ang = (360-ang)/180*Mathf.PI;
			// x = x1 cosa - y1 sina + a
			// y = y1 cosa + x1 sina + b
			
			//新坐标 (0,-w)
			float x = (0) * Mathf.Cos (ang) - (-w/2) * Mathf.Sin (ang) + Center.x;
			float y = (-w/2)*Mathf.Cos(ang) + (0)*Mathf.Sin(ang) + Center.y;
			
			return new Vector3 (x,y,0);
		}
	}
	//以Center为原点,forwordport为方向,的坐标系中, 画一个方形,反回右下角坐标
	public static Vector3 getC (Vector3 Center,Vector3 forwardPoint,float w)
	{
		//点距
		float d = Mathf.Sqrt ((Center.x-forwardPoint.x)*(Center.x-forwardPoint.x)+(Center.y-forwardPoint.y)*(Center.y-forwardPoint.y));
		
		//Vector3.Angle 一二像限正常  Vector3.Angle 三四像限 
		
		//1像限
		if(Center.x <= forwardPoint.x && Center.y <= forwardPoint.y)
		{
			float ang = Vector3.Angle(forwardPoint-Center,Vector3.right);
			ang = ang/180*Mathf.PI;
			// x = x1 cosa - y1 sina + a
			// y = y1 cosa + x1 sina + b
			
			//新坐标 (d,-w)
			float x = (d) * Mathf.Cos (ang) - (-w/2) * Mathf.Sin (ang) + Center.x;
			float y = (-w/2)*Mathf.Cos(ang) + (d)*Mathf.Sin(ang) + Center.y;
			
			return new Vector3 (x,y,0);
		}
		//2像限
		else if(Center.x >= forwardPoint.x && Center.y <= forwardPoint.y)
		{
			float ang = Vector3.Angle(forwardPoint-Center,Vector3.right);
			ang = ang/180*Mathf.PI;
			// x = x1 cosa - y1 sina + a
			// y = y1 cosa + x1 sina + b
			
			//新坐标 (d,-w)
			float x = (d) * Mathf.Cos (ang) - (-w/2) * Mathf.Sin (ang) + Center.x;
			float y = (-w/2)*Mathf.Cos(ang) + (d)*Mathf.Sin(ang) + Center.y;
			
			return new Vector3 (x,y,0);
		}
		//3像限
		else if(Center.x >= forwardPoint.x && Center.y >= forwardPoint.y)
		{			
			float ang = Vector3.Angle(forwardPoint-Center,Vector3.right);
			//Debug.Log(ang);
			ang = (360-ang)/180*Mathf.PI;
			// x = x1 cosa - y1 sina + a
			// y = y1 cosa + x1 sina + b
			
			//新坐标 (d,-w)
			float x = (d) * Mathf.Cos (ang) - (-w/2) * Mathf.Sin (ang) + Center.x;
			float y = (-w/2)*Mathf.Cos(ang) + (d)*Mathf.Sin(ang) + Center.y;
			
			return new Vector3 (x,y,0);
			
		}
		//4像限
		else
		{
			float ang = Vector3.Angle(forwardPoint-Center,Vector3.right);
			//Debug.Log(ang);
			ang = (360-ang)/180*Mathf.PI;
			// x = x1 cosa - y1 sina + a
			// y = y1 cosa + x1 sina + b
			
			//新坐标 (d,-w)
			float x = (d) * Mathf.Cos (ang) - (-w/2) * Mathf.Sin (ang) + Center.x;
			float y = (-w/2)*Mathf.Cos(ang) + (d)*Mathf.Sin(ang) + Center.y;
			
			return new Vector3 (x,y,0);
		}
	}
	//以Center为原点,forwordport为方向,的坐标系中, 画一个方形,反回右上角坐标
	public static Vector3 getD (Vector3 Center,Vector3 forwardPoint,float w)
	{
		//点距
		float d = Mathf.Sqrt ((Center.x-forwardPoint.x)*(Center.x-forwardPoint.x)+(Center.y-forwardPoint.y)*(Center.y-forwardPoint.y));
		
		//Vector3.Angle 一二像限正常  Vector3.Angle 三四像限 
		
		//1像限
		if(Center.x <= forwardPoint.x && Center.y <= forwardPoint.y)
		{
			float ang = Vector3.Angle(forwardPoint-Center,Vector3.right);
			ang = ang/180*Mathf.PI;
			// x = x1 cosa - y1 sina + a
			// y = y1 cosa + x1 sina + b
			
			//新坐标 (d,-w)
			float x = (d) * Mathf.Cos (ang) - (w/2) * Mathf.Sin (ang) + Center.x;
			float y = (w/2)*Mathf.Cos(ang) + (d)*Mathf.Sin(ang) + Center.y;
			
			return new Vector3 (x,y,0);
		}
		//2像限
		else if(Center.x >= forwardPoint.x && Center.y <= forwardPoint.y)
		{
			float ang = Vector3.Angle(forwardPoint-Center,Vector3.right);
			ang = ang/180*Mathf.PI;
			// x = x1 cosa - y1 sina + a
			// y = y1 cosa + x1 sina + b
			
			//新坐标 (d,-w)
			float x = (d) * Mathf.Cos (ang) - (w/2) * Mathf.Sin (ang) + Center.x;
			float y = (w/2)*Mathf.Cos(ang) + (d)*Mathf.Sin(ang) + Center.y;
			
			return new Vector3 (x,y,0);
		}
		//3像限
		else if(Center.x >= forwardPoint.x && Center.y >= forwardPoint.y)
		{			
			float ang = Vector3.Angle(forwardPoint-Center,Vector3.right);
			//Debug.Log(ang);
			ang = (360-ang)/180*Mathf.PI;
			// x = x1 cosa - y1 sina + a
			// y = y1 cosa + x1 sina + b
			
			//新坐标 (d,-w)
			float x = (d) * Mathf.Cos (ang) - (w/2) * Mathf.Sin (ang) + Center.x;
			float y = (w/2)*Mathf.Cos(ang) + (d)*Mathf.Sin(ang) + Center.y;
			
			return new Vector3 (x,y,0);
			
		}
		//4像限
		else
		{
			float ang = Vector3.Angle(forwardPoint-Center,Vector3.right);
			//Debug.Log(ang);
			ang = (360-ang)/180*Mathf.PI;
			// x = x1 cosa - y1 sina + a
			// y = y1 cosa + x1 sina + b
			
			//新坐标 (d,-w)
			float x = (d) * Mathf.Cos (ang) - (w/2) * Mathf.Sin (ang) + Center.x;
			float y = (w/2)*Mathf.Cos(ang) + (d)*Mathf.Sin(ang) + Center.y;
			
			return new Vector3 (x,y,0);
		}
	}
	
}







