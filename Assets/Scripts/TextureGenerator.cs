using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class TextureGenerator
{
    public static Texture2D TextureFromColorMap(Color[]colorMap,int width,int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(colorMap);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;   
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromHeightMap(float[,] heigthMap)
    {
        int height = heigthMap.GetLength(0); // rows
        int width = heigthMap.GetLength(1);  // columns
        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heigthMap[y, x]);
            }
        }
        return TextureFromColorMap(colorMap, width, height);
    }
    

}
