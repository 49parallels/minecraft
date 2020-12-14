using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.Rendering;

public class World : MonoBehaviour
{
    private Material material;
    public Material Material
    {
        get => material;
        set => material = value;
    }

    private Vector2Int areaSize;
    public Vector2Int AreaSize
    {
        get => areaSize;
        set => areaSize = value;
    }

    private int scale;

    public int Scale
    {
        get => scale;
        set => scale = value;
    }

    public int FieldOfView
    {
        get => fieldOfView;
        set => fieldOfView = value;
    }

    // number of blocks player sees (used for fog of war like map)
    private int fieldOfView = 10;
    
    // Perlin noise generator
    private PerlinGenerator perlinGenerator;

    // Player
    public Transform player;

    private Vector3[,] mapData;
    private List<GameObject> saveBuilderData = new List<GameObject>();
    
    // map of blocks in world
    private Block[,] blocks;
    private List<GameObject>[,] builderBlocks;

    // Blocks inside Field of View (used for fog of war like map style)
    private List<Block> blocksInView = new List<Block>();
    private List<GameObject> buildingBlocksInView = new List<GameObject>();

    // World config
    public WorldConfig config;
    public SaveTemplate saveSlot;
    
    
    void Start()
    {
        SaveManager.Instance.LoadGame();
        
        AreaSize = config.areaSize;
        Material = config.material;
        Scale = config.scale;
        
        SetupWorldData();

        //GenerateWorld();
    }

    private void GenerateWorld()
    {
        foreach (var data in mapData)
        {
            InitBlock(data);
        }
    }
    
    private void GeneratePerlinData()
    {
        perlinGenerator = new PerlinGenerator(transform.position, areaSize, scale);
        mapData = perlinGenerator.Generate();
    }

    private void SetupWorldData()
    {
        blocks = new Block[areaSize.x, areaSize.y];
        builderBlocks = new List<GameObject>[areaSize.x,areaSize.y];
        
        if (null != saveSlot && saveSlot.IsValid())
        {
            LoadSavedData();
        }
        else
        {
            Vector3Int spawnLocation = new Vector3Int(Mathf.FloorToInt(areaSize.x / 2f), 12, Mathf.FloorToInt(areaSize.y / 2f));
            player.transform.position = spawnLocation;
        }
        
        GeneratePerlinData();
    }

    //used for fog of war like map (experimental not using)
    void GenerateInRadiusOf(Vector3 position)
    {
        List<Block> previouslyActiveBlocks = new List<Block>(blocksInView);
        List<GameObject> previouslyActiveBuildingBlocks = new List<GameObject>(buildingBlocksInView);
        
        int xCoordinate = (int)position.x;
        int yCoordinate = (int)position.z;
        
        for(int x = xCoordinate - fieldOfView; x < xCoordinate + fieldOfView; x++)
        {
            for (int y = yCoordinate - fieldOfView; y < yCoordinate + fieldOfView; y++)
            {
                // uncover world terrain blocks
                if (!IsWithinArea(x, y)) continue;
                if (blocks[x, y] == null && mapData[x, y] != null)
                {
                    InitBlock(mapData[x, y]);
                }
                else if(!blocks[x, y].isActive)
                {
                    blocks[x, y].isActive = true;
                    blocksInView.Add(blocks[x, y]);
                }
                
                for (int blockIndex = 0; blockIndex < previouslyActiveBlocks.Count; blockIndex++)
                {
                    if (previouslyActiveBlocks[blockIndex].Equals(blocks[x, y]))
                        previouslyActiveBlocks.RemoveAt(blockIndex);
                    
                }
                
                if (builderBlocks[x, y] != null)
                {
                    
                    foreach (var buildingBlock in builderBlocks[x,y])
                    {
                        if (!buildingBlock) continue;
                        buildingBlock.SetActive(true);
                        buildingBlocksInView.Add(buildingBlock);
                        foreach (var prevBuildingBlock in previouslyActiveBuildingBlocks.ToList())
                        {
                            if (buildingBlock.Equals(prevBuildingBlock))
                                previouslyActiveBuildingBlocks.Remove(prevBuildingBlock);
                        }
                    }
                }
            }
        }

        foreach (var block in previouslyActiveBlocks)
        {
            block.isActive = false;
        }

        foreach (var buildingBlock in previouslyActiveBuildingBlocks)
        {
            if (buildingBlock) buildingBlock.SetActive(false);
        }
    }
    
    bool IsWithinArea(int x, int y)
    {
        if (x > 0 && x < AreaSize.x && y > 0 && y < AreaSize.y)
            return true;
            
        return false;
    }

    private void LoadSavedData()
    {
        player.transform.position = saveSlot.playerPosition;
        foreach (var data in saveSlot.serializedBuildedBlocks)
        {
            BlockAdd(data.definition, data.material);
        }
    }
    
    private void Update()
    {
        SetupSaveData();
        //  used for fog of war like map uncomment if you want to try (experimental)
        GenerateInRadiusOf(player.transform.position);
    }

    private void InitBlock(Vector3 withData)
    {
        Block newBlock = new Block(this, withData);
        blocks[(int)withData.x, (int)withData.z] = newBlock;
        blocksInView.Add(newBlock);
    }

    public void BlockAdd(Vector3 atPosition, BlockType blockType)
    {
        Block newBlock = new Block(this, atPosition, blockType);
        BlockAddToBuildersData(newBlock.GetObject());
        saveBuilderData.Add(newBlock.GetObject());
        newBlock.GetObject().SetActive(false);
    }

    public void BlockRemove(GameObject blockGameObject)
    {
        if (!blockGameObject.GetComponent<BlockController>().block.isPartOfTerrain())
        {
            saveBuilderData.Remove(blockGameObject);
            Destroy(blockGameObject);
        }
    }

    private void SetupSaveData()
    {
        saveSlot.buildedBlocks = saveBuilderData;
        saveSlot.playerPosition = player.transform.position;
    }

    private void BlockAddToBuildersData(GameObject block)
    {
        Vector3 atPosition = block.transform.position;
        if (null == builderBlocks[(int) atPosition.x, (int) atPosition.z])
        {
            builderBlocks[(int) atPosition.x, (int) atPosition.z] = new List<GameObject>();
        }
        builderBlocks[(int) atPosition.x, (int) atPosition.z].Add(block);
    }
}
