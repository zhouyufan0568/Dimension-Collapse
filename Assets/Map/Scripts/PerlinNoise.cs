using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PerlinNoise : MonoBehaviour {

	private NoiseFuction noise;

	public int MapChunkNumX=30;
	public int MapChunkNumZ=30;
	public int ChunkSize=100;
	public int ChunkNumX;
	public int ChunkNumZ;
	public int terrainHeight = 10;

	private int worldX;
	private int worldZ;
	private float terrainSizeX;
	private float terrainSizeZ;

	// Use this for initialization
	void Start () {

		noise = new NoiseFuction ();

		worldX = ChunkNumX * ChunkSize;
		worldZ = ChunkNumZ * ChunkSize;
		terrainSizeX = MapChunkNumX * ChunkSize;
		terrainSizeZ = MapChunkNumZ * ChunkSize;
		CreateMap ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void CreateMap(){
		int worldY;
		for (int x = 0; x < ChunkSize; x++) {
			for (int z = 0; z < ChunkSize; z++) {
				//worldY = Mathf.FloorToInt(terrainHeight * noise.PerlinNoise ((worldX + x)/(float)ChunkSize, (worldZ + z)/(float)ChunkSize));
				worldY = Mathf.FloorToInt(terrainHeight * noise.PerlinNoise ((worldX + x)/(float)terrainSizeX, (worldZ + z)/(float)terrainSizeZ));
				GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
				if (worldY <= 5) {
					cube.GetComponent<MeshRenderer> ().material = (Material) Resources.Load ("Materials/(26)sea");
				} else {
					cube.GetComponent<MeshRenderer> ().material = (Material) Resources.Load ("Materials/(25)grass");
				}
				cube.transform.position = new Vector3 (worldX + x, worldY, worldZ + z);
				cube.transform.SetParent (transform);
			}
		}
	}
}
