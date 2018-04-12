using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineMesh : MonoBehaviour {
	public Material materialPrefab;

	public int Chunk_X_Max { get; private set; }
	public int Chunk_Z_Max { get; private set; }
	public int ChunkSize { get; private set; }
	public int ChunkCount { get; private set; }

	// All possible block textures
	public Texture2D[] World_Textures;
	// Our texture atlas
	private Texture2D WorldTextureAtlas;

	public Rect[] WorldTextureAtlasUvs { get; set; }

	public readonly Dictionary<int, BlockUVCoordinates> m_BlockUVCoordinates =
		new Dictionary<int, BlockUVCoordinates>();

	//private MaterialManager materialManager;
	private MapChunkIO mapChunkIO;
	private Vector3Pool vector3Pool;

	private Vector3[] tempVector3s = new Vector3[8];
	private int[][] visibleToVerts = new int[6][]{
		new int[]{ 2, 6, 4, 0 },
		new int[]{ 1, 5, 7, 3 },
		new int[]{ 0, 1, 3, 2 },
		new int[]{ 5, 4, 6, 7 },
		new int[]{ 0, 4, 5, 1 },
		new int[]{ 3, 7, 6, 2 }
	};
	private int[] triSorts = new int[6]{0, 1, 3, 1, 2, 3};

	private void Awake()
	{
		InitializeTextures();
		//materialManager = MaterialManager.INSTANCE;
		mapChunkIO = new MapChunkIO();
		vector3Pool = Vector3Pool.INSTANCE;
		Chunk_X_Max = mapChunkIO.Chunk_X_Max;
		Chunk_Z_Max = mapChunkIO.Chunk_Z_Max;
		ChunkSize = mapChunkIO.ChunkSize;
		ChunkCount = mapChunkIO.ChunkCount;
		//Debug.Log("Chunk_X_Max: " + Chunk_X_Max);
		//Debug.Log("Chunk_Z_Max: " + Chunk_Z_Max);
		//Debug.Log("ChunkSize: " + ChunkSize);
		//Debug.Log("ChunkCount: " + ChunkCount);
	}

	public bool IsChunkExists(int chunkX, int chunkZ)
	{
		if (chunkX >= Chunk_X_Max || chunkZ >= Chunk_Z_Max)
		{
			return false;
		}

		BytePoint[] positions;
		byte[] materialIds;
		bool[][] isVisible;
		int visiblePlaneCount;
		if (mapChunkIO.GetChunkDataByXZ(chunkX, chunkZ, out positions, out materialIds, out isVisible, out visiblePlaneCount))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public bool GetChunkByXZ(int chunkX, int chunkZ, out GameObject chunk)
	{
		chunk = null;

		if (chunkX >= Chunk_X_Max || chunkZ >= Chunk_Z_Max)
		{
			return false;
		}

		BytePoint[] positions;
		byte[] materialIds;
		bool[][] isVisible;
		int visiblePlaneCount;
		if (mapChunkIO.GetChunkDataByXZ(chunkX, chunkZ, out positions, out materialIds, out isVisible, out visiblePlaneCount))
		{
			chunk = new GameObject();
            chunk.layer = 9;

			Mesh mesh = new Mesh
			{
				name = "Combined"
			};
			int cubeCount = positions.Length;
			Vector3[] verts = new Vector3[visiblePlaneCount * 4];
			int[] tris = new int[visiblePlaneCount * 6];
			Vector2[] uv = new Vector2[visiblePlaneCount * 4];
			Vector3[] normals = new Vector3[visiblePlaneCount * 4];
			int planeWritten = 0;
			for (int i = 0; i < cubeCount; i++)
			{
				float x = positions[i].X;
				float y = positions[i].Y;
				float z = positions[i].Z;

				//(0, 0, 0)
				tempVector3s[0].x = x;
				tempVector3s[0].y = y;
				tempVector3s[0].z = z;

				//(1, 0, 0)
				tempVector3s[1].x = x + 1;
				tempVector3s[1].y = y;
				tempVector3s[1].z = z;

				//(0, 0, 1)
				tempVector3s[2].x = x;
				tempVector3s[2].y = y;
				tempVector3s[2].z = z + 1;

				//(1, 0, 1)
				tempVector3s[3].x = x + 1;
				tempVector3s[3].y = y;
				tempVector3s[3].z = z + 1;

				//(0, 1, 0)
				tempVector3s[4].x = x;
				tempVector3s[4].y = y + 1;
				tempVector3s[4].z = z;

				//(1, 1, 0)
				tempVector3s[5].x = x + 1;
				tempVector3s[5].y = y + 1;
				tempVector3s[5].z = z;

				//(0, 1, 1)
				tempVector3s[6].x = x;
				tempVector3s[6].y = y + 1;
				tempVector3s[6].z = z + 1;

				//(1, 1, 1)
				tempVector3s[7].x = x + 1;
				tempVector3s[7].y = y + 1;
				tempVector3s[7].z = z + 1;

				//mat
				int curMaterialId = materialIds[i];
                //Debug.Log(curMaterialId);
				BlockUVCoordinates uvCoordinates = m_BlockUVCoordinates[curMaterialId];

				for (int j = 0; j < 6; j++)
				{
					if (isVisible[i][j])
					{
						for (int k = planeWritten * 4; k < planeWritten * 4 + 4; k++)
						{
							verts[k] = CopyVector3(tempVector3s[visibleToVerts[j][k - planeWritten * 4]]);
						}
						for (int k = planeWritten * 6; k < planeWritten * 6 + 6; k++)
						{
							tris[k] = triSorts[k - planeWritten * 6] + planeWritten * 4;
						}

						Rect materialRect;
						switch (j)
						{
						case 0:
						case 1:
						case 4:
						case 5:
							materialRect = uvCoordinates.BlockFaceUvCoordinates[(int)BlockFace.Side];
							break;
						case 2:
							materialRect = uvCoordinates.BlockFaceUvCoordinates[(int)BlockFace.Bottom];
							break;
						case 3:
							materialRect = uvCoordinates.BlockFaceUvCoordinates[(int)BlockFace.Top];
							break;
						default:
							materialRect = uvCoordinates.BlockFaceUvCoordinates[(int)BlockFace.Side];
							break;
						}
						uv[planeWritten * 4 + 0] = new Vector2(materialRect.xMin, materialRect.yMin);
						uv[planeWritten * 4 + 1] = new Vector2(materialRect.xMin, materialRect.yMax);
						uv[planeWritten * 4 + 2] = new Vector2(materialRect.xMax, materialRect.yMax);
						uv[planeWritten * 4 + 3] = new Vector2(materialRect.xMax, materialRect.yMin);

						//for (int k = planeWritten * 4; k < planeWritten * 4 + 4; k++)
						//{
						//    switch (j)
						//    {
						//        case 0:
						//            normals[k] = Vector3.left;
						//            break;
						//        case 1:
						//            normals[k] = Vector3.right;
						//            break;
						//        case 2:
						//            normals[k] = Vector3.down;
						//            break;
						//        case 3:
						//            normals[k] = Vector3.up;
						//            break;
						//        case 4:
						//            normals[k] = Vector3.back;
						//            break;
						//        case 5:
						//            normals[k] = Vector3.forward;
						//            break;
						//    }
						//}
						planeWritten++;
					}
				}
			}
			//Debug.Log("vertices length: " + verts.Length);
			//Debug.Log("triangles length: " + tris.Length);
			//Debug.Log("uv length: " + uv.Length);
			//Debug.Log("planeWritten: " + planeWritten);
			mesh.vertices = verts;
			mesh.triangles = tris;
			mesh.uv = uv;
			mesh.RecalculateNormals();
			//mesh.RecalculateTangents();
			//mesh.RecalculateBounds();
			//CalculateMeshTangents(mesh);

			MeshFilter thisMeshFilter = chunk.AddComponent<MeshFilter>();
			thisMeshFilter.mesh = mesh;

			MeshRenderer thisMeshRenderer = chunk.AddComponent<MeshRenderer>();
			thisMeshRenderer.sharedMaterial = materialPrefab;

			MeshCollider thisMeshCollider = chunk.AddComponent<MeshCollider>();
			thisMeshCollider.sharedMesh = mesh;
		}
		else
		{
			return false;
		}

		chunk.name = "Chunk(" + chunkX + ',' + ' ' + chunkZ + ')';
		chunk.transform.position = new Vector3(chunkX * ChunkSize, 0, chunkZ * ChunkSize);
		chunk.layer=9;
		return true;
	}

	public void RestoreChunkByXZ(int chunkX, int chunkZ, GameObject chunk)
	{
		Destroy(chunk);
	}

	private Vector3 CopyVector3(Vector3 template)
	{
		return vector3Pool.GetVector3ByXYZ((int)template.x, (int)template.y, (int)template.z);
	}

	private void InitializeTextures()
	{
		WorldTextureAtlas = new Texture2D (2048, 2048, TextureFormat.ARGB32, false);
		WorldTextureAtlasUvs = WorldTextureAtlas.PackTextures(World_Textures, 0);
		WorldTextureAtlas.filterMode = FilterMode.Bilinear;
		WorldTextureAtlas.anisoLevel = 9;
		WorldTextureAtlas.Apply();
		materialPrefab.mainTexture = WorldTextureAtlas;
		GenerateUVCoordinatesForAllBlocks();
	}

    public void GenerateUVCoordinatesForAllBlocks()
    {
        // Topsoil
		SetBlockUVCoordinates(BlockType.Default, 0, 0, 0);
		SetBlockUVCoordinates(BlockType.Stone_andesite_smooth, 1, 1, 1);
		SetBlockUVCoordinates(BlockType.BookShelf, 2, 2, 2);
		SetBlockUVCoordinates(BlockType.Glass_magenta, 3, 3, 3);
		SetBlockUVCoordinates(BlockType.Glass_cyan, 4, 4, 4);
		SetBlockUVCoordinates(BlockType.Concrete_green, 5, 5, 5);
		SetBlockUVCoordinates(BlockType.Concrete_blue, 6, 6, 6);
		SetBlockUVCoordinates(BlockType.Wool_Colored_orange, 7, 7, 7);
		SetBlockUVCoordinates(BlockType.Concrete_red, 8, 8, 8);
		SetBlockUVCoordinates(BlockType.Concrete_silver, 9, 9, 9);
		SetBlockUVCoordinates(BlockType.Shulker_top_blue, 10, 10, 10);
		SetBlockUVCoordinates(BlockType.Shulker_top_line, 11, 11, 11);
		SetBlockUVCoordinates(BlockType.Shulker_top_yellow, 12, 12, 12);
		SetBlockUVCoordinates(BlockType.Wool_colored_yellow, 13, 13, 13);
		SetBlockUVCoordinates(BlockType.Cobblestone, 14, 14, 14);
		SetBlockUVCoordinates(BlockType.Hardened_clay_staind_brown, 15, 15, 15);
		SetBlockUVCoordinates(BlockType.Hardened_clay_staind_white, 16, 16, 16);
		SetBlockUVCoordinates(BlockType.Pumpkin_face_on, 17, 17, 17);
		SetBlockUVCoordinates(BlockType.Shulker_top_brown, 18, 18, 18);
		SetBlockUVCoordinates(BlockType.Stone_granite_smooth, 19, 19, 19);
		SetBlockUVCoordinates(BlockType.Trandoor, 20, 20, 20);
		SetBlockUVCoordinates(BlockType.Wool_colored_lime, 21, 21, 21);
		SetBlockUVCoordinates(BlockType.Wool_colored_red, 22, 22, 22);
		SetBlockUVCoordinates(BlockType.black, 23, 23, 23);
		SetBlockUVCoordinates(BlockType.Wool_colored_pink, 24, 24, 24);
		SetBlockUVCoordinates(BlockType.grass, 25, 27, 27);
		SetBlockUVCoordinates(BlockType.sea, 26, 26, 26);

        SetBlockUVCoordinates(BlockType.grass_path_top, 28, 29, 28);
        SetBlockUVCoordinates(BlockType.grass_path_top_deep, 30, 31, 30);
        SetBlockUVCoordinates(BlockType.grass_path_top_s, 32, 33, 32);
        SetBlockUVCoordinates(BlockType.stonebrick_mossy, 34, 34, 34);
        SetBlockUVCoordinates(BlockType.door_iron_upper_s, 35, 35, 35);
        SetBlockUVCoordinates(BlockType.Tnt, 37, 36, 38);
        SetBlockUVCoordinates(BlockType.wheat_stage_3, 39, 39, 39);
        SetBlockUVCoordinates(BlockType.sandstone_carved, 40, 40, 40);
        SetBlockUVCoordinates(BlockType.brick1, 41, 41, 41);
        SetBlockUVCoordinates(BlockType.brick2, 42, 42, 42);
        SetBlockUVCoordinates(BlockType.brickside, 43, 43, 43);
        SetBlockUVCoordinates(BlockType.cobblestone, 44, 44, 44);
        SetBlockUVCoordinates(BlockType.Concrete_cyan, 45, 45, 45);
        SetBlockUVCoordinates(BlockType.Concrete_powder_light_blue, 46, 46, 46);
        SetBlockUVCoordinates(BlockType.Concrete_lime , 47, 47, 47);
        SetBlockUVCoordinates(BlockType.Concrete_powder_orange, 48, 48, 48);
        SetBlockUVCoordinates(BlockType.Concrete_powder_yellow, 49, 49, 49);
        SetBlockUVCoordinates(BlockType.Crafting_table_top, 50, 50, 50);
        SetBlockUVCoordinates(BlockType.Daylight_detector_top, 51, 51, 51);
        SetBlockUVCoordinates(BlockType.Dispenser_front_horizontal, 52, 52, 52);
        SetBlockUVCoordinates(BlockType.Door_wood_upper, 53, 53, 53);
        SetBlockUVCoordinates(BlockType.end_stone, 54, 54, 54);
        SetBlockUVCoordinates(BlockType.grass_top, 55, 55, 55);
        SetBlockUVCoordinates(BlockType.hardened_clay_stained_brown, 56, 56, 56);
        SetBlockUVCoordinates(BlockType.hopper_top, 57, 57, 57);
        SetBlockUVCoordinates(BlockType.leaves_acacia, 58, 58, 58);
        SetBlockUVCoordinates(BlockType.leaves_jungle_opaque, 59, 59, 59);
        SetBlockUVCoordinates(BlockType.dropper_front_horizontal, 59, 59, 59);
        SetBlockUVCoordinates(BlockType.liie_oak_gate_side, 60, 60, 60);
        SetBlockUVCoordinates(BlockType.log_oak, 61, 61, 61);
        SetBlockUVCoordinates(BlockType.log_oak1, 62, 62, 62);
        SetBlockUVCoordinates(BlockType.observer_front, 63, 63, 63);
        SetBlockUVCoordinates(BlockType.obsidian, 64, 64, 64);
        SetBlockUVCoordinates(BlockType.planks_oak, 65, 65, 65);
        SetBlockUVCoordinates(BlockType.purpur_block, 66, 66, 66);
        SetBlockUVCoordinates(BlockType.stone, 67, 67, 67);
        SetBlockUVCoordinates(BlockType.stonebrick_carved, 68, 68, 68);
        SetBlockUVCoordinates(BlockType.stonebrick_cracked, 69, 69, 69);
        SetBlockUVCoordinates(BlockType.tnt_top1, 70, 71, 72);
        SetBlockUVCoordinates(BlockType.wheat_stage_7, 73, 73, 73);
        SetBlockUVCoordinates(BlockType.wool_colored_brown, 74, 74, 74);
        SetBlockUVCoordinates(BlockType.wool_colored_cyan, 75, 75, 75);
        SetBlockUVCoordinates(BlockType.wool_colored_gray, 76, 76, 76);
        SetBlockUVCoordinates(BlockType.wool_colored_orange, 77, 77, 77);
        SetBlockUVCoordinates(BlockType.wool_colored_red, 78, 78, 78);
        SetBlockUVCoordinates(BlockType.wool_colored_yellow, 79, 79, 79);
        SetBlockUVCoordinates(BlockType.Concrete_green1, 5, 5, 5);
        SetBlockUVCoordinates(BlockType.Concrete_red1, 8, 8, 8);
        SetBlockUVCoordinates(BlockType.Tnt_side, 5, 5, 5);
        SetBlockUVCoordinates(BlockType.grass_deep, 80, 27, 27);
        SetBlockUVCoordinates(BlockType.grass_s, 81, 27, 27);

    }

    private void SetBlockUVCoordinates(BlockType blockType, int topIndex, int sideIndex, int bottomIndex)
    {
        m_BlockUVCoordinates.Add((int)(blockType),
            new BlockUVCoordinates(WorldTextureAtlasUvs[topIndex], WorldTextureAtlasUvs[sideIndex],
                WorldTextureAtlasUvs[bottomIndex]));
    }

    private void Start() {
        //Debug.Log("start");
//        for (int chunkX = 0; chunkX < Chunk_X_Max; chunkX++)
//        {
//            for (int chunkZ = 0; chunkZ < Chunk_Z_Max; chunkZ++)
//            {
//                //Debug.Log(chunkX);
//                //Debug.Log(chunkZ);
//                GameObject chunk;
//                GetChunkByXZ(chunkX, chunkZ, out chunk);
//            }
//        }
    }
}
