using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveBuildData
{
    public Vector3 definition;
    public BlockType material;
    public int hardeness;

    public SaveBuildData(Vector3 blockDefiniton, BlockType blockMaterial, int blockHardeness)
    {
        definition = blockDefiniton;
        material = blockMaterial;
        hardeness = blockHardeness;
    }
}
