using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator
{
    public static int length, width;
    public int[,] grid;



    public GridGenerator(int length,int width)
    {
        GenerateRandomGrid(length,width);
    }

    void GenerateRandomGrid(int length,int width)
    {
        grid = new int[length, width];
        int spawnrand = Random.Range(0, width);
        int baserand = Random.Range(0, width);
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                
                // Generate a random value (0 or 1)
                int randomValue = Random.Range(0, 2);
                grid[i, j] = 1;

                if (i == 0 && j == spawnrand)
                {
                    grid[i, j] = 2;
                    
                }
                if (i == length-1 && j == baserand)
                {
                    grid[i, j] = 3;
                    
                }
            }
        }
    }


}
