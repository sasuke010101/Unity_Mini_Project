using UnityEngine;
using System.Collections;

//一个例子,在obj1 和 obj2 间画一条线

public class Test : MonoBehaviour {
	public GameObject obj1;
	public GameObject obj2;
	public GameObject obj3;
	public GameObject obj4;

	GameObject sign;

	// Use this for initialization
	void Start () 
	{
		//贝赛尔线
		myBezier br = createBezier (obj1.transform.localPosition,obj2.transform.localPosition);
		Vector3[] points = br.GetPoints (30); //30是始点和终点间 画点的密集成度,数越大越密 不要问一亿行不行 - -!
		Mesh ms = MeshDashedLine.CreateMesh(0.2f, points, false, true);
		MeshLine.Draw (ms,null);

		//贝赛尔线(虚线)
		myBezier br2 = createBezier (obj2.transform.localPosition,obj3.transform.localPosition);
		Vector3[] points2 = br2.GetPoints (30); //30是始点和终点间 画点的密集成度,数越大越密 不要问一亿行不行 - -!
		Mesh ms2 = MeshDashedLine.CreateMesh (0.2f, points2, false, false);
		MeshLine.Draw (ms2,null);

		//直线(虚)  有uv图的 可以把线加宽,变实 会看到图 :)
		int i = 0;
		Vector3[] points3 = new Vector3[30];  //30我随便写的, 分段要自己写好算法 让每个小方块的长和宽差不多,才均好看, 这里不讨论了
		Vector3 start = obj3.transform.localPosition;
		Vector3 end = obj4.transform.localPosition;
		float dx = (end.x - start.x) / points3.Length; //把两点间分成 30段,每段x y 增多少
		float dy = (end.y - start.y) / points3.Length;
		for(i=0;i<points3.Length;i++)
		{
			points3[i].x = start.x + dx*i;
			points3[i].y = start.y + dy*i;
		}
		Mesh ms3 = MeshDashedLine.CreateMesh (0.2f, points3, true, false);
		MeshLine.Draw (ms3,null);


		//r=a(1-sinx)
		i = 0;
		Vector3[] ps = new Vector3[250];
		for(int x=0;x<500;x+=2,i++)
		{
			ps[i].x = x/10.0f;
			ps[i].y = 2*(1-Mathf.Sin(x/10.0f));
		}
		Mesh ms4 = MeshDashedLine.CreateMesh (0.2f, ps, false, true);
		MeshLine.Draw (ms4,null);

		//用有折角的线
		//r=a(1-sinx)
		i = 0;
		Vector3[] ps2 = new Vector3[250];
		for(int x=0;x<500;x+=2,i++)
		{
			ps2[i].x = x/10.0f;
			ps2[i].y = 2*(1-Mathf.Sin(x/10.0f))+1;
		}
		Mesh ms5 = MeshAngleLine.CreateMesh (0.2f, ps2, false);
		MeshLine.Draw (ms5,null);
	}

	myBezier createBezier (Vector3 vecport1,Vector3 vecport2)
	{
		Vector3 v0 = Vector3.zero;
		Vector3 v1 = Vector3.zero;
		Vector3 v2 = Vector3.zero;
		Vector3 v3 = Vector3.zero;
		
		//算出起点,终点,控制点
		float oldx1 = vecport1.x;
		float oldx2 = vecport2.x;
		
		//这里的控制点要么靠想像力,要么找个现成的画线程序观察控制点和端点的位置关系
		bool dist = Vector3.Distance (vecport1, vecport2) < 80;
		
		if(oldx1>oldx2)
		{
			v0 = vecport1;
			v1 = new Vector3(vecport1.x-Mathf.Abs(vecport1.x-vecport2.x),vecport1.y,vecport1.z);
			v2 = new Vector3(vecport2.x+Mathf.Abs(vecport1.x-vecport2.x),vecport2.y,vecport1.z);
			if(dist) //两点太近的话,控制点要调远点,好看
			{
				v1 = new Vector3(vecport1.x-Mathf.Abs(vecport1.x-vecport2.x)/2f,vecport1.y,vecport1.z);
				v2 = new Vector3(vecport2.x+Mathf.Abs(vecport1.x-vecport2.x)/2f,vecport2.y,vecport1.z);
			}
			v3 = vecport2;
		}
		else
		{
			v0 = vecport1;
			v1 = new Vector3(vecport1.x+Mathf.Abs(vecport1.x-vecport2.x),vecport1.y,vecport1.z);
			v2 = new Vector3(vecport2.x-Mathf.Abs(vecport1.x-vecport2.x),vecport2.y,vecport1.z);
			if(dist) //两点太近的话,控制点要调远点,好看
			{
				v1 = new Vector3(vecport1.x+Mathf.Abs(vecport1.x-vecport2.x)/2f,vecport1.y,vecport1.z);
				v2 = new Vector3(vecport2.x-Mathf.Abs(vecport1.x-vecport2.x)/2f,vecport2.y,vecport1.z);
			}
			v3 = vecport2;
		}
		myBezier br = new myBezier (v0,v1,v2,v3);
		
		return br;
	}
}
