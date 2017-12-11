using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector3Pool {
    private static int LENGTH_X = 16;
    private static int LENGTH_Y = 256;
    private static int LENGTH_Z = 16;
    public static Vector3Pool INSTANCE = new Vector3Pool();

    private Vector3[,,] vector3Space;

    private Vector3Pool()
    {
        vector3Space = new Vector3[LENGTH_X, LENGTH_Y, LENGTH_Z];
        for (int x = 0; x < LENGTH_X; x++)
        {
            for (int y = 0; y < LENGTH_Y; y++)
            {
                for (int z = 0; z < LENGTH_Z; z++)
                {
                    vector3Space[x, y, z] = new Vector3(x, y, z);
                }
            }
        }
    }

    public Vector3 GetVector3ByXYZ(int x, int y, int z)
    {
        if (x >= 0 && x < LENGTH_X
            && y >= 0 && y < LENGTH_Y
            && z >= 0 && z < LENGTH_Z)
        {
            return vector3Space[x, y, z];
        }
        else
        {
            return Vector3.zero;
        }
    }
}
