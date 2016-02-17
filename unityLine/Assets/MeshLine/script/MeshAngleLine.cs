using UnityEngine;
using System.Collections;

//高明,2015,QQ28djfsdkhfsldfha:)

//这是由无数长方形组成的线,最后合成一个mesh,显示为一条线 (隔一个留一个长方形就是虚线 隔X个也行,自己改代码)
public class MeshAngleLine : MonoBehaviour {

	//w线的宽,points是一条理论上的线,由连续的点组成(比如一个berzier串) ,uv是否计算uv ,
	public static Mesh CreateMesh(float w,Vector3[] points ,bool uv)
	{
		Mesh ms = new Mesh (); 
		ms.name = "MDLine";
		ms.vertices = getPoints(points,w);
		ms.triangles = getTri(ms.vertices.Length);
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
	static int[] getTri(int len)
	{
			int num = (len-2)*2*3; //三角形有 点数减2 *2个,再乘以3是三角顶点数
			int[] ret = new int[num]; //三角个数正好是点数减2    *3是三角   *2是双面
			
			int j = 0;
			for(int i=0;i<len-2;i+=2,j+=6)
			{
				ret[j+0]=i+0;
				ret[j+1]=i+1;
				ret[j+2]=i+2;
				
				ret[j+3]=i+0;
				ret[j+4]=i+2;
				ret[j+5]=i+3;
			}
			
			//反面  //TMD 点序不对,法线自动被翻了, 所以用双面
			for(int i=0;i<len-2;i+=2,j+=6)
			{
				ret[j+0]=i+0;
				ret[j+1]=i+2;
				ret[j+2]=i+1;
				
				ret[j+3]=i+0;
				ret[j+4]=i+3;
				ret[j+5]=i+2;
			}
			return ret;
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
		Vector3[] lefts = new Vector3[inpoints.Length*2];//inpoints点左侧的所有点

		if(true)
		{
			int j=0;
			Vector3 r1 = Vector3.zero;
			Vector3 r2 = Vector3.zero;

			//第一个点 //为什么第一个点getB ,,,,,,,,,试出来的
			lefts[j++] = MeshDashedLine.getB(inpoints[0],inpoints[1],w);
			lefts[j++] = MeshDashedLine.getA(inpoints[0],inpoints[1],w);
			for(int i=1;i<(inpoints.Length-1);i++)
			{
				getAB(inpoints[i-1],inpoints[i],inpoints[i+1],w,out r1,out r2,i);
				lefts[j++] = r1;
				lefts[j++] = r2;
			}
			//最后一个点
			if(inpoints.Length%2==1)//最后一点是偶数点
			{
				lefts[j++] = MeshDashedLine.getC(inpoints[inpoints.Length-2],inpoints[inpoints.Length-1],w);
				lefts[j++] = MeshDashedLine.getD(inpoints[inpoints.Length-2],inpoints[inpoints.Length-1],w);
			}
			else
			{
				lefts[j++] = MeshDashedLine.getD(inpoints[inpoints.Length-2],inpoints[inpoints.Length-1],w);
				lefts[j++] = MeshDashedLine.getC(inpoints[inpoints.Length-2],inpoints[inpoints.Length-1],w);
			}
		}

		return lefts;
	}

	//角ABC 内外两个折点             w线宽            r1,r2返回的点    num是这排点的奇偶号
	static void getAB(Vector3 A,Vector3 B,Vector3 C,float w,out Vector3 r1,out Vector3 r2,int num)
	{
		Vector3 BA = A - B;  //把 A 平移到 B为原点处
		Vector3 BC = C - B;

		if (num % 2 == 0) 
		{
			num = -1;	
		}
		else
		{
			num = 1;
		}

		BA.Normalize (); //只是使两个向量同长,方便求角平分线向量
		BC.Normalize ();

		Vector3 BB = (BA + BC) / 2; //角平分线向量

		BB.Normalize (); //长度变1

		int q = getQuadrant (A, B, C); //通过三个点的分布,知道r1 r2哪一个在内角,哪一个是外折角

		if(q==3||q==4)
		{
			r2 = BB*(w / 2); //折角处的一个点
			r1 = -r2;  //原点对面的对称折点

			r1*=num;
			r2*=num;

			//平移回原坐标系
			r1 += B;
			r2 += B;
		}
		else
		{
			r1 = BB*(w / 2); //折角处的一个点
			r2 = -r1;  //原点对面的对称折点

			r1*=num;
			r2*=num;

			//平移回原坐标系
			r1 += B;
			r2 += B;
		}
	}

	//ABC是全局空间中的坐标, 如果以B为新原点 ,A为X负方向,那 返回C反在的象限数  
	static int getQuadrant(Vector3 A,Vector3 B,Vector3 C)
	{
		float ang;
		//先算出BA的旋转角度 以B为原点 BA 与负轴的夹角
		//1像限
		if(A.x >= B.x && A.y >= B.y)
		{
			//AB直线相对世界转了 ang 角度
			ang = Vector3.Angle(A-B,Vector3.left);
			ang = ang/180*Mathf.PI;
		}
		//2像限
		else if(A.x <= B.x && A.y >= B.y)
		{
			ang = Vector3.Angle(A-B,Vector3.left);
			ang = ang/180*Mathf.PI;
		}
		else if(A.x <= B.x && A.y <= B.y) 
		{			
			ang = Vector3.Angle(A-B,Vector3.left);
			ang = (360-ang)/180*Mathf.PI;
		}
		//4像限
		else
		{
			ang = Vector3.Angle(A-B,Vector3.left);
			ang = (360-ang)/180*Mathf.PI;
		}

			//x1 y1新系坐标, x y 是在旧系中的坐标
			// x = x1 cosa - y1 sina + a
			// y = y1 cosa + x1 sina + b

			//已知旧系,求新系
			//任意点(Cx,Cy)，绕一个坐标点(Bx,By)逆时针旋转a角度后的新的坐标设为(x0, y0)，公式：
			//x0= (Cx - Bx)*cos(a) - (Cy - By)*sin(a) + Bx ;
			//y0= (Cx - Bx)*sin(a) + (Cy - By)*cos(a) + By ;
			
		//新坐标 xy
		float x = (C.x - B.x) * Mathf.Cos (ang) - (C.y - B.y) * Mathf.Sin (ang);// + B.x; //不加就是以B为原点时的坐标,加就是以原始原点的坐标
		float y = (C.x - B.x) * Mathf.Sin (ang) + (C.y - B.y) * Mathf.Cos (ang);// + B.y;

		if (x >= 0 && y >= 0)
			return 1;
		if (x <= 0 && y >= 0)
			return 2;
		if (x <= 0 && y <= 0)
			return 3;

		return 4;
	}
	
}







