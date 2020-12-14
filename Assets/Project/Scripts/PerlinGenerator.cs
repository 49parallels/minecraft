using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinGenerator
{
    private float xOrigin;
    private float yOrigin;

    private int scaleFactor;
    
    private Vector2Int areaSize;

    //public List<Vector3> mapData = new List<Vector3>();
    
    public PerlinGenerator(Vector2 origin, Vector2Int area, int scale)
    {
        xOrigin = origin.x;
        yOrigin = origin.y;
        areaSize = area;
        //mapData = new Vector3[areaSize.x,areaSize.y];
        scaleFactor = scale;
    }

    public void Generate(out Vector3[,] mapData)
    {
        mapData = new Vector3[areaSize.x, areaSize.y];
        
        float y = 0.0f;

        while (y < areaSize.y)
        {
            float x = 0.0f;
            while (x < areaSize.x)
            {
                float xPos = xOrigin + x;
                float yPos = yOrigin + y;
                float xCoord = xPos / areaSize.x * scaleFactor;
                float yCoord = yPos / areaSize.y * scaleFactor;
                float height = Mathf.PerlinNoise(xCoord, yCoord);
                //mapData.Add(new Vector3(xPos,height*10, yPos));
                mapData[(int)xPos, (int)yPos] = new Vector3(xPos, height * 10, yPos);
                x++;
            }
            y++;
        }

    }
}
