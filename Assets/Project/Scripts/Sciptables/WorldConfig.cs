using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Config", menuName = "Configs/World Config", order = 1)]
public class WorldConfig : ScriptableObject
{
    public Material material;
    public Vector2Int areaSize;
    public int scale;
    public Material[] blockMaterials;
}
