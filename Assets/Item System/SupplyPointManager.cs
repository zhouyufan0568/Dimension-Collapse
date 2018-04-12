using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

namespace DimensionCollapse
{
    public class SupplyPointManager : MonoBehaviour
    {
        public static SupplyPointManager INSTANCE
        {
            get
            {
                return instance;
            }
        }
        private static SupplyPointManager instance;

        private static string KEY = "SPITI";

        [HideInInspector]
        public Vector3[] SupplyPoints;

        private Dictionary<int, int> supplyPointIndexToItemId;
        private Item[] items;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;

            string rootPath = Application.streamingAssetsPath + "/Map/MapFragments";
            string supplyPointsPath = rootPath + "/supplyPoints.txt";
            if (!File.Exists(supplyPointsPath))
            {
                Debug.Log("Can't find supplyPoints.txt, SupplyPoints will be an empty array.");
                SupplyPoints = new Vector3[0];
            }
            else
            {
                using (StreamReader sr = new StreamReader(supplyPointsPath))
                {
                    string supplyPointsData = sr.ReadToEnd();
                    supplyPointsData.Replace("\r", "");
                    string[] supplyPointStrs = supplyPointsData.Split('\n');
                    List<Vector3> SupplyPointsTemp = new List<Vector3>(supplyPointStrs.Length);
                    for (int i = 0; i < supplyPointStrs.Length - 1; i++)
                    {
                        try
                        {
                            //Debug.Log(supplyPointStrs[i]);
                            string[] xyz = supplyPointStrs[i].Split(' ');
                            float x = float.Parse(xyz[0]);
                            float y = float.Parse(xyz[1]);
                            float z = float.Parse(xyz[2]);
                            SupplyPointsTemp.Add(new Vector3(x, y, z));
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e);
                        }
                    }
                    SupplyPoints = SupplyPointsTemp.ToArray();
                    Debug.Log("There will be " + SupplyPoints.Length + " supply points in the map:");
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < SupplyPoints.Length; i++)
                    {
                        builder.AppendLine(SupplyPoints[i].ToString());
                    }
                    Debug.Log(builder.ToString());
                }
            }
        }

        private void Start()
        {
            ItemManager.INSTANCE.ResetItemCollection();
            if (PhotonNetwork.isMasterClient)
            {
                for (int i = 0; i < SupplyPoints.Length; i++)
                {
                    Vector3 position = SupplyPoints[i];
                    Quaternion rotation = Quaternion.identity;
                    string itemName = ItemManager.INSTANCE.GetRandomItemName();
                    ItemManager.INSTANCE.AddIntoItemCollection(PhotonNetwork.InstantiateSceneObject(itemName, position, rotation, 0, null));
                }
            }
            //if (PhotonNetwork.isMasterClient)
            //{
            //    supplyPointIndexToItemId = new Dictionary<int, int>(SupplyPoints.Length);
            //    for (int i = 0; i < SupplyPoints.Length; i++)
            //    {
            //        supplyPointIndexToItemId[i] = ItemManager.INSTANCE.GetRandomItemPrefab().ID;
            //    }

            //    ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable
            //    {
            //        { KEY, supplyPointIndexToItemId }
            //    };
            //    PhotonNetwork.room.SetCustomProperties(hashtable);
            //}
            //else
            //{
            //    try
            //    {
            //        supplyPointIndexToItemId = (Dictionary<int, int>)Convert.ChangeType(PhotonNetwork.room.CustomProperties[KEY], typeof(Dictionary<int, int>));
            //    }
            //    catch (Exception e)
            //    {
            //        Debug.Log("Can't obtain items to be spawn. There will be no item in the map of current client.");
            //        supplyPointIndexToItemId = new Dictionary<int, int>(SupplyPoints.Length);
            //        for (int i = 0; i < SupplyPoints.Length; i++)
            //        {
            //            supplyPointIndexToItemId[i] = -1;
            //        }
            //    }
            //}

            //ItemManager.INSTANCE.ResetItemCollection();
            //items = new Item[SupplyPoints.Length];
            //for (int i = 0; i < SupplyPoints.Length; i++)
            //{
            //    Item item = ItemManager.INSTANCE.InstantiateItemById(supplyPointIndexToItemId[i]);
            //    item.transform.SetPositionAndRotation(
            //        SupplyPoints[i],
            //        Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0)
            //        );
            //    items[i] = item;
            //}
        }
    }
}

