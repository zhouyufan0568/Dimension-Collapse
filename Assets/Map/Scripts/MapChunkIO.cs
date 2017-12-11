using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapChunkIO {

    public int Chunk_X_Max { get; private set; }
    public int Chunk_Z_Max { get; private set; }
    public int ChunkSize { get; private set; }
    public int ChunkCount { get; private set; }

    private ChunkData[,] chunks;

    public MapChunkIO()
    {
        //step 1: get the single instance of the MaterialManager. 
        //materialManager = MaterialManager.INSTANCE;

        //step 2: read the mapInfo.bytes.
        string rootPath = Application.streamingAssetsPath + "/Map/MapFragments";
        //Debug.Log("rootPath: " + rootPath);
        if (!ValidateDirExists(rootPath))
        {
            Debug.LogError("Map is broken.");
            return;
        }
        string mapInfoPath = rootPath + "/mapInfo.bytes";
        if (!ValidateFileExists(mapInfoPath))
        {
            Debug.LogError("Map is broken.");
            return;
        }
        using (BinaryReader br = new BinaryReader(new FileStream(mapInfoPath, FileMode.Open)))
        {
            Chunk_X_Max = br.ReadInt32();
            Chunk_Z_Max = br.ReadInt32();
            ChunkSize = br.ReadInt32();
            //Debug.Log("Chunk_X_Max: " + Chunk_X_Max);
            //Debug.Log("Chunk_Z_Max: " + Chunk_Z_Max);
            //Debug.Log("ChunkSize: " + ChunkSize);
        }

        //step 3: read all chunks.
        ChunkCount = 0;
        chunks = new ChunkData[Chunk_X_Max, Chunk_Z_Max];
        for (int chunkX = 0; chunkX < Chunk_X_Max; chunkX++)
        {
            string dirPath = rootPath + '/' + chunkX;
            if (!ValidateDirExists(dirPath))
            {
                continue;
            }

            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            FileInfo[] fileInfos = dirInfo.GetFiles();
            foreach (FileInfo fileInfo in fileInfos)
            {
                if (!fileInfo.FullName.EndsWith(".bytes"))
                {
                    continue;
                }

                string strChunkZ = fileInfo.Name.Split('.')[0];
                int chunkZ;
                if (int.TryParse(strChunkZ, out chunkZ))
                {
                    using(BinaryReader br = new BinaryReader(new FileStream(fileInfo.FullName, FileMode.Open)))
                    {
                        int cubeCount = br.ReadInt32();
                        int materialCount = br.ReadInt32();
                        BytePoint[] positions = new BytePoint[cubeCount];
                        byte[] materialIds = new byte[cubeCount];
                        bool[][] isVisible = new bool[cubeCount][];
                        int index = 0;
                        for (int i = 0; i < materialCount; i++)
                        {
                            byte materialId = br.ReadByte();
                            int count = br.ReadInt32();
                            while (count-- > 0)
                            {
                                ushort encodedXYZ = br.ReadUInt16();
                                byte encodedVisible = br.ReadByte();
                                byte x, y, z;
                                DecodeXYZ(encodedXYZ, out x, out y, out z);
                                positions[index] = new BytePoint(x, y, z);
                                materialIds[index] = materialId;
                                isVisible[index] = DecodeVisible(encodedVisible);
                                index++;
                            }
                        }
                        int visibleCount;
                        visibleCount = br.ReadInt32();

                        chunks[chunkX, chunkZ] = new ChunkData(positions, materialIds, isVisible, visibleCount);
                        ChunkCount++;
                    }
                }
            }
        }
    }

    private bool ValidateFileExists(string path)
    {
        return new FileInfo(path).Exists;
    }

    private bool ValidateDirExists(string path)
    {
        return new DirectoryInfo(path).Exists;
    }

    public static ushort EncodeXYZ(ushort x, ushort y, ushort z)
    {
        ushort res = 0;
        res += x;
        res += (ushort)(z * 16);
        res += (ushort)(y * 256);
        return res;
    }

    public static void DecodeXYZ(ushort data, out byte x, out byte y, out byte z)
    {
        x = (byte)(data % 16);
        data /= 16;
        z = (byte)(data % 16);
        data /= 16;
        y = (byte)(data % 256);
    }

    public static byte EncodeVisible(bool[] isVisible)
    {
        //for (int i = 0; i < isVisible.Length; i++)
        //{
        //    Debug.Log(isVisible[i]);
        //}
        int res = 0;
        for (int i = 0; i < 6; i++)
        {
            if (isVisible[i])
            {
                res |= 1 << i;
            }
        }
        //Debug.Log(res);
        return (byte)res;
    }

    public static bool[] DecodeVisible(byte isVisibleInByte)
    {
        //Debug.Log(isVisibleInByte);
        bool[] res = new bool[6];
        for (int i = 0; i < 6; i++)
        {
            int isVisible = isVisibleInByte % 2;
            if (isVisible == 1)
            {
                res[i] = true;
            }
            isVisibleInByte >>= 1;
        }
        //for (int i = 0; i < res.Length; i++)
        //{
        //    Debug.Log(res[i]);
        //}
        return res;
    }

    public bool GetChunkDataByXZ(int x, int z, out BytePoint[] positions, out byte[] materialIds, out bool[][] isVisible, out int visiblePlaneCount)
    {
        positions = null;
        materialIds = null;
        isVisible = null;
        visiblePlaneCount = 0;
        //Debug.Log(x);
        //Debug.Log(z);
        if (!(x >= 0 && x < Chunk_X_Max) || !(z >= 0 && z < Chunk_Z_Max))
        {
            return false;
        }

        ChunkData chunk = chunks[x, z];
        if (chunk == null)
        {
            return false;
        }
        //Debug.Log("here");

        positions = chunk.positions;
        materialIds = chunk.materialIds;
        isVisible = chunk.isVisible;
        visiblePlaneCount = chunk.visiblePlaneCount;
        return true;
    }

    private class ChunkData
    {
        public BytePoint[] positions;
        public byte[] materialIds;
        public bool[][] isVisible;
        public int visiblePlaneCount;

        public ChunkData(BytePoint[] positions, byte[] materialIds, bool[][] isVisible, int visiblePlaneCount)
        {
            this.positions = positions;
            this.materialIds = materialIds;
            this.isVisible = isVisible;
            this.visiblePlaneCount = visiblePlaneCount;
        }
    }

}
