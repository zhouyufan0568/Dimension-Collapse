using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager
{
    public static readonly MaterialManager INSTANCE = new MaterialManager();
    private Dictionary<byte, Material> idToMaterial;
    private Dictionary<string, byte> materialNameToId;

    private MaterialManager()
    {
        idToMaterial = new Dictionary<byte, Material>();

        Material[] materials = Resources.LoadAll<Material>("Materials/");
        foreach (Material m in materials)
        {
            int from = m.name.IndexOf('(') + 1;
            int to = m.name.IndexOf(')') - 1;
            if (from < 0 || to < 0)
            {
                continue;
            }
            string strId = m.name.Substring(from, to);
            byte id;
            if (byte.TryParse(strId, out id))
            {
                if (!idToMaterial.ContainsKey(id))
                {
                    idToMaterial.Add(id, m);
                }
            }
        }
    }

    public Material IdToMaterial(byte id)
    {
        if (idToMaterial.ContainsKey(id))
        {
            return idToMaterial[id];
        }
        else
        {
            return idToMaterial[0];
        }
    }

    public byte MaterialNameToId(string materialName)
    {
        if (materialNameToId == null)
        {
            materialNameToId = new Dictionary<string, byte>(idToMaterial.Count);
            byte[] Ids = GetAllIds();
            foreach (byte Id in Ids)
            {
                materialNameToId.Add(idToMaterial[Id].name, Id);
            }
        }

        if (materialNameToId.ContainsKey(materialName))
        {
            return materialNameToId[materialName];
        }
        else
        {
            return 0;
        }
    }

    public byte[] GetAllIds()
    {
        Dictionary<byte, Material>.KeyCollection keys = idToMaterial.Keys;
        byte[] Ids = new byte[keys.Count];
        int index = 0;
        foreach (byte Id in keys)
        {
            Ids[index++] = Id;
        }
        return Ids;
    }
}
