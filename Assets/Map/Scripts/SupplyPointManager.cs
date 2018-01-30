using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace DimensionCollapse
{
    public class SupplyPointManager
    {
        public static readonly SupplyPointManager INSTANCE;
        static SupplyPointManager()
        {
            INSTANCE = new SupplyPointManager();
        }
        public float[,] SupplyPoints;
        private SupplyPointManager()
        {
            string rootPath = Application.streamingAssetsPath + "/Map/MapFragments";
            string supplyPointsPath = rootPath + "/supplyPoints.txt";
            if (!File.Exists(supplyPointsPath))
            {
                Debug.Log("Can't find supplyPoints.txt, SupplyPoints will be an empty array.");
                SupplyPoints = new float[0, 3];
            }
            else
            {
                using (StreamReader sr = new StreamReader(supplyPointsPath))
                {
                    string supplyPointsData = sr.ReadToEnd();
                    string[] supplyPointStrs = supplyPointsData.Split('\n');
                    SupplyPoints = new float[supplyPointStrs.Length - 1, 3];
                    for (int i = 0; i < supplyPointStrs.Length - 1; i++)
                    {
                        string[] xyz = supplyPointStrs[i].Split(' ');
                        SupplyPoints[i, 0] = float.Parse(xyz[0]);
                        SupplyPoints[i, 1] = float.Parse(xyz[1]);
                        SupplyPoints[i, 2] = float.Parse(xyz[2]);
                    }
                }
            }
        }
    }
}

