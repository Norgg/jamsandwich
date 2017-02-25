using System;
using UnityEngine;


public static class GridUtil{
	public static Vector3 GridCoordToWorld(GridCoord gridCoord, float tSize){
		float rowOffset = Camera.main.orthographicSize;
		float colOffset = Camera.main.orthographicSize * Camera.main.aspect;
		float x = gridCoord.col * tSize - colOffset + tSize / 2;
		float y = gridCoord.row * tSize - rowOffset + tSize / 2;
		return new Vector3(x, y, 0);
	}
	
	public static GridCoord MouseToGridCoord(Vector3 mousePos, float tSize){
		Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePos);
		float rowOffset = Camera.main.orthographicSize;
		float colOffset = Camera.main.orthographicSize * Camera.main.aspect;
		
		int row = Mathf.FloorToInt((mousePosWorld.y + rowOffset) / tSize);
		int col = Mathf.FloorToInt((mousePosWorld.x + colOffset) / tSize);
		
		return new GridCoord(row,col);
	}
}

[Serializable]
public class GridCoord
{
	public int row;
	public int col;
	
	public GridCoord (){}
	public GridCoord(int r, int c){
		row = r;
		col = c;
	}
	
	public GridCoord(GridCoord gc){
		row = gc.row;
		col = gc.col;
	}
	
}
