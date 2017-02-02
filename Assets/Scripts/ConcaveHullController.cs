using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FluffyUnderware.Curvy;

public class ConcaveHullController : MonoBehaviour 
{
	private int maxHullVerts = 32;
	private int maxHulls = 128;
	private float precision = 1.0f;

	bool init = false;
	private float t;

	private List<GameObject> _childMeshes = new List<GameObject> ();

	// Use this for initialization
	public void StartATL ( ThirdPersonUserControl player, CurvySpline spline ) 
	{
		/*
		 * calling hull functions doesn't work
		print ("start in ConcaveHullController called");
		foreach (Transform child in transform) 
		{
			//can set vetices here 
			ConcaveCollider newCollider = child.gameObject.AddComponent<ConcaveCollider>();
			newCollider.MaxHullVertices = maxHullVerts;
			newCollider.MaxHulls = maxHulls;
			newCollider.Precision = precision;
			newCollider.ForceNoMultithreading = true;
			newCollider.ComputeHulls (null, null);
			//child.GetComponent<ConcaveCollider> ().ComputeHulls (null, null);
			Destroy(child.transform.GetChild(0).gameObject);
			child.gameObject.AddComponent<MakeMesh> ();
			//print ("largest hull verts = " + child.GetComponent<ConcaveCollider> ().GetLargestHullVertices ());
		}
		*/

		foreach (Transform child in transform) 
		{
			_childMeshes.Add (child.gameObject);
		}

	}
	
	// Update is called once per frame
	public void UpdateATL ( ThirdPersonUserControl player, CurvySpline spline ) 
	{

		if (!init && spline.IsInitialized)
		{
			init = true;
			t = spline.GetNearestPointTF (player.transform.position);
		}

		if (!init)
			return;

		t = spline.GetNearestPointTF (player.transform.position);
		float indexf = t / (1f / _childMeshes.Count); //assuming this rounds down to int
		indexf = indexf - t % (1f / _childMeshes.Count);
		int index = (int) indexf;

		// t goes from 0 to 1. count of meshes goes from 0 to count.
		// 1 / count is the approximate "t" length of each segment 
		// t / ( 1/count) is the number of segments that fit into t
		// enable a range of ints around t/(1/count)

		for (int i = 0; i < _childMeshes.Count; i++) 
		{
			if (i > index - 3 && i < index + 3) 
			{
				_childMeshes [i].GetComponent<ConcaveCollider> ().enabled = true;
				//_childMeshes [i].GetComponent<MeshCollider> ().enabled = false;
			} 
			else 
			{
				_childMeshes [i].GetComponent<ConcaveCollider> ().enabled = false;
				//_childMeshes [i].GetComponent<MeshCollider> ().enabled = true;
			}
		}
	
	}
}
