using UnityEngine;
using System.Collections;

//GL.xxx     
//LineRender 
//Vectrosity 		
//都用过,种种原因,还是想自己写一个

//高明,2015,QQ28djfsdkhfsldfha:)


public class MeshLine : MonoBehaviour {
	static Material mat = null;

	//w线的宽,points是一条理论上的线,由连续的点组成(比如一个berzier串) ,parentobj是线最终建到哪个物体下 ,uv是否计算uv
	public static void Draw(Mesh ms,GameObject parentObj)
	{
		if(mat==null)
		{
			mat = Resources.Load("MeshLineMat") as Material;
		}

		GameObject obj = new GameObject ();
		obj.name = "Test";
		if(parentObj!=null)
		{
			obj.transform.parent = parentObj.transform;
		}
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localScale = Vector3.one;
		MeshFilter mf = obj.AddComponent<MeshFilter>();
		MeshRenderer mr = obj.AddComponent<MeshRenderer>();
	
		mf.mesh = ms;

		mr.material = mat;
	}
}








