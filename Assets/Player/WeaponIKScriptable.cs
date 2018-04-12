using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponIKData", menuName = "WeaponIKData")]
public class WeaponIKScriptable : ScriptableObject
{
    public List<int> weaponIds;
    public List<Vector3> weaponPositions;
    public List<Vector3> weaponRotations;
    public List<Vector3> weaponScales;
}
