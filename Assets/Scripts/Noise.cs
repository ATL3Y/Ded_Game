using UnityEngine;
using System.Collections;

public class Noise : MonoBehaviour {

	ImprovedPerlinNoise perlin;

	public int seed = 0;
	public float frequency = 10.0f;
	public float lacunarity = 2.0f;
	public float gain = 0.5f;

	public void StartATL () 
	{
		perlin = new ImprovedPerlinNoise(seed);

		perlin.LoadResourcesFor4DNoise();

		GetComponent<Renderer>().material.SetTexture("_PermTable1D", perlin.GetPermutationTable1D());
		GetComponent<Renderer>().material.SetTexture("_PermTable2D", perlin.GetPermutationTable2D());
		GetComponent<Renderer>().material.SetTexture("_Gradient4D", perlin.GetGradient4D());
	}

	public void UpdateATL ()
	{
		GetComponent<Renderer>().material.SetFloat("_Frequency", frequency);
		GetComponent<Renderer>().material.SetFloat("_Lacunarity", lacunarity);
		GetComponent<Renderer>().material.SetFloat("_Gain", gain);
	}
}
