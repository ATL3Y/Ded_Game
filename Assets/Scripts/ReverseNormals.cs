using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(MeshFilter))]
public class ReverseNormals : MonoBehaviour 
{
	
	public void StartATL () 
	{
		MeshFilter filter = GetComponent(typeof (MeshFilter)) as MeshFilter;
		if (filter != null)
		{
			Mesh mesh = filter.mesh;
			
			Vector3[] normals = mesh.normals;
			for (int i = 0; i < normals.Length; i++) 
			{
				normals [i] = -normals [i];
			}
			mesh.normals = normals;
			
			for (int j = 0; j < mesh.subMeshCount; j++)
			{
				int[] triangles = mesh.GetTriangles(j);
				for (int i = 0; i < triangles.Length; i += 3)
				{
					int temp = triangles[i + 0];
					triangles[i + 0] = triangles[i + 1];
					triangles[i + 1] = temp;
				}
				mesh.SetTriangles(triangles, j);
			}
		}        
		
		this.GetComponent<MeshCollider>().sharedMesh = filter.mesh;
	}
}
