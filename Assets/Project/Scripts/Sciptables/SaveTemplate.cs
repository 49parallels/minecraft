using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SaveTemplate", menuName = "SaveTemplate", order = 51)]
public class SaveTemplate : ScriptableObject
{
    public Vector3 playerPosition = Vector3.zero;
    public List<SaveBuildData> serializedBuildedBlocks;

    public List<GameObject> buildedBlocks = new List<GameObject>();
    
    public void SerializeData() 
    {
        serializedBuildedBlocks = new List<SaveBuildData>();
        foreach (var data in buildedBlocks)
        {
            serializedBuildedBlocks.Add(new SaveBuildData(data.transform.position, data.GetComponent<BlockController>().block.GetType(), 1));
        }
    }

    public void DeserializeData()
    {
        return;
    }

    public bool IsValid()
    {
        return (!playerPosition.Equals(Vector3.zero));
    }

    public void Reset()
    {
        playerPosition = Vector3.zero;
        serializedBuildedBlocks = new List<SaveBuildData>();
    }
}
