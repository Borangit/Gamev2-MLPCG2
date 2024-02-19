using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerater
{
    private int width, height;
    private List<Vector2Int> pathCells;

    public PathGenerater(int width, int height)
    {
        this.width = width;
        this.height = height;
    }
    public List<Vector2Int> GeneratePath(){
        pathCells = new List<Vector2Int>();
        int y = (int)(height / 2);
        int x = 0;
        // for (int i = 0; i < width; i++)
        // {
        //     pathCells.Add(new Vector2Int(i, y));
        
        // }
        while (x < width){
            pathCells.Add(new Vector2Int(x, y));

            bool validMove = false;
            while (!validMove){
                int move = Random.Range(0, 3);

                if (move == 0 || x % 2 == 0 || x > (width-2)){
                    x++;
                    validMove = true;
                }
                else if (move == 1 && CellisFree(x, y + 1) && y < height - 3){
                    
                    y++;
                    validMove = true;
                }
                else if (move == 2 && CellisFree(x, y - 1) && y > 2){
                    
                    y--;
                    validMove = true;
                }
            }
        }
        return pathCells;
    }

    public bool CellisFree(int x, int y){
        return !pathCells.Contains(new Vector2Int(x, y));
    }
}
