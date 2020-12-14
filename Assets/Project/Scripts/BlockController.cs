using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


public class BlockController : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Color selectedBlockColor = new Color(1f, 0.53f, 0.3f, 1f);
    
    public World world;
    public Block block;
    
    private int hardness;

    private void Start()
    {
        if (block != null) 
        {
            hardness = (int) block.GetType() * 5;
        }
    }

    void Update()
    {
        if (!meshRenderer)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
    }

    public void Select()
    {
        if (!meshRenderer) return;
        meshRenderer.material.SetColor("_ColorFilter", selectedBlockColor);
    }

    public void Unselect()
    {
        if (!meshRenderer) return;
        meshRenderer.material.SetColor("_ColorFilter", Color.white);
    }

    public void AddBlock(Vector3 atPosition, BlockType blockType)
    {
        if (!world) return;
        world.BlockAdd(atPosition, blockType);
    }

    public void RemoveBlock()
    {
        if (!world) return;
        world.BlockRemove(gameObject);
    }

    public void GraduallyDestroyBlock()
    {
        hardness -= 1;
        if (hardness < 1)
        {
            RemoveBlock();
        }
    }
}
