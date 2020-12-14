using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

// Texture types
public enum TextureType
{
    grassTop,
    grassSide,
    soil,
    stone
}

// Depth of voxel on y axis
public enum DepthType
{
    top,
    middle,
    bottom,
}

public enum BlockType
{
    red,
    green,
    blue,
    yellow
}



public class Block: IEquatable<Block>
{
    private GameObject rootObject;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private BlockController blockController;
    
    MeshCollider meshCollider;
    
    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uv = new List<Vector2>();
    private bool[,,] voxelMap;
    private BlockType type;
    
    private TextureType[,] textureMap = new TextureType[3, 6]
    {
        //top
        {
            TextureType.grassSide, TextureType.grassSide,TextureType.grassTop,
            TextureType.soil, TextureType.grassSide, TextureType.grassSide
        },
        // middle
        {
            TextureType.soil, TextureType.soil, TextureType.soil, TextureType.soil,
            TextureType.soil, TextureType.soil
        },
        // bottom
        {
            TextureType.stone, TextureType.stone, TextureType.stone, TextureType.stone,
            TextureType.stone, TextureType.stone
        }
    };

    private int height = 1;
    private Vector3 position;
    public Material material;
    private World world;

    private bool isTerrainBlock = true;

    public World World
    {
        get => world;
    }

    /**
     * Use this method to spawn new (1,1,1)-block with given material
     */
    public Block(World worldReference, Vector3 atPosition, BlockType blockType)
    {
        Setup(worldReference, atPosition, worldReference.config.blockMaterials[(int)blockType]);
        
        height = 0;
        rootObject.transform.position = new Vector3(position.x * Voxel.width, position.y * Voxel.height, position.z * Voxel.width);
        rootObject.name = "Non world Block " + position.x + ":" + position.y + ":" + position.z;
        type = blockType;
        isTerrainBlock = false;
        
        PostSetup();
    } 

    /**
     * Spawn this block when digging or generating world
     */
    public Block(World worldReference,Vector3 mapPositionWithHeight)
    {
        Setup(worldReference, mapPositionWithHeight, worldReference.Material);
        
        rootObject.transform.position = new Vector3(position.x * Voxel.width, 0, position.z * Voxel.width);
        rootObject.name = "Block " + position.x + ":" + position.y + ":" + position.z;
        PostSetup();
    }

    private void Setup(World worldReference, Vector3 atPosition, Material blockMaterial)
    {
        world = worldReference;
        position = atPosition;
        material = blockMaterial;
        rootObject = new GameObject();
        
        meshFilter = rootObject.AddComponent<MeshFilter>();
        meshRenderer = rootObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
        meshCollider = rootObject.AddComponent<MeshCollider>();
        

        blockController = rootObject.AddComponent<BlockController>();
        blockController.world = world;
        blockController.block = this;
        
        height = Mathf.RoundToInt(atPosition.y);
        
        rootObject.transform.parent = world.transform;
    }

    void PostSetup()
    {
        AddToVoxelMap();
        
        GenerateVoxelMap();
        CreateMeshData();
        GenerateMesh();

        meshCollider.sharedMesh = meshFilter.mesh;
    }

    void GenerateVoxelMap()
    {
        for (int x = 0; x < Voxel.width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < Voxel.width; z++)
                {
                    voxelMap[x, y, z] = true;
                }
            }
        }
    }

    void CreateMeshData()
    {
        DepthType selectedDepthType;
        for (int x = 0; x < Voxel.width; x++)
        {
            for (int y = 0; y <= height; y++)
            {
                if (y > (height - 1))
                {
                    selectedDepthType = DepthType.top;
                } else if (y == 0)
                {
                    selectedDepthType = DepthType.bottom;
                }
                else
                {
                    selectedDepthType = DepthType.middle;
                }
                for (int z = 0; z < Voxel.width; z++)
                {
                    AddVoxel(new Vector3(x,y,z), selectedDepthType);            
                }
            }
        }
    }

    public bool isActive
    {
        get { return rootObject.activeSelf; }
        set
        {
            if (rootObject)
            {
                rootObject.SetActive(value);
            }
        }
    }

    bool CheckVoxel(Vector3 onPosition)
    {
        int x = Mathf.FloorToInt(onPosition.x);
        int y = Mathf.FloorToInt(onPosition.y);
        int z = Mathf.FloorToInt(onPosition.z);

        if (x < 0 || x > Voxel.width - 1 || y < 0 || y > height -1 || z < 0 || z > Voxel.width -1)
            return false;
        
        return voxelMap[x, y, z];
    }


    void AddVoxel(Vector3 onPosition, DepthType depthType)
    {
        for (int faceIndex = 0; faceIndex < 6; faceIndex++)
        {
            if (!CheckVoxel(onPosition + Voxel.directions[faceIndex]))
            {
                vertices.Add(Voxel.vertices[Voxel.triangles[faceIndex, 0]] + onPosition);
                vertices.Add(Voxel.vertices[Voxel.triangles[faceIndex, 1]] + onPosition);
                vertices.Add(Voxel.vertices[Voxel.triangles[faceIndex, 2]] + onPosition);
                vertices.Add(Voxel.vertices[Voxel.triangles[faceIndex, 3]] + onPosition);
                SetTexture(textureMap[(int)depthType, faceIndex]);
                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);
                vertexIndex += 4;

            }
        }
    }

    void GenerateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    void SetTexture(TextureType type)
    {
        Vector2 bottomLeft;
        Vector2 topLeft;
        Vector2 bottomRight;
        Vector2 topRight;
        switch (type)
        {
            case TextureType.grassSide:
                bottomLeft = new Vector2(0,0);
                topLeft = new Vector2(0,0.5f);
                bottomRight = new Vector2(0.5f, 0);
                topRight = new Vector2(0.5f, 0.5f);
                break;
            case TextureType.grassTop:
                bottomLeft = new Vector2(0.5f,0.5f);
                topLeft = new Vector2(0.5f,1f);
                bottomRight = new Vector2(1f, 0.5f);
                topRight = new Vector2(1f, 1f);
                break;
            case TextureType.soil:
                bottomLeft = new Vector2(0.5f,0f);
                topLeft = new Vector2(0.5f,0.5f);
                bottomRight = new Vector2(1f, 0f);
                topRight = new Vector2(1f, 0.5f);
                break;
            case TextureType.stone:
                bottomLeft = new Vector2(0,0.5f);
                topLeft = new Vector2(0,1f);
                bottomRight = new Vector2(0.5f, 0.5f);
                topRight = new Vector2(0.5f, 1f);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, "TextureType error");
        }
        
        uv.Add(bottomLeft);
        uv.Add(topLeft);
        uv.Add(bottomRight);
        uv.Add(topRight);
    }

    public Vector3 GetDefinitionData()
    {
        // check if is part of generated world or new block 
        float yCoordinate = (height == 0) ? position.y : height; 
        return new Vector3(position.x, yCoordinate, position.z);
    }

    public void Destroy()
    {
        Destroy();
    }

    private void AddToVoxelMap()
    {
        voxelMap = new bool[Voxel.width, height, Voxel.width];
    }

    public BlockType GetType()
    {
        return type;
    }

    public bool Equals(Block other)
    {
        var lb = Vector3Int.RoundToInt(GetDefinitionData());
        var rb = Vector3Int.RoundToInt(other.GetDefinitionData());
        return (lb == rb);
    }

    public GameObject GetObject()
    {
        return rootObject;
    }

    public bool isPartOfTerrain()
    {
        return isTerrainBlock;
    }
}
