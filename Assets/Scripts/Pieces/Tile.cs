using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public int xIndex;
	public int yIndex;
	public bool cancelTile = false;
	private BoardManager board;
	private void Awake() 
	{
        board = Object.FindObjectOfType<BoardManager>();
	}
	public void Init(int x, int y)
	{
		xIndex = x;
		yIndex = y;
	}
	private void OnMouseDown() 
	{
		board.ClickTile(this);
	}
	private void OnMouseEnter() 
	{
		board.DragTile(this);
	}
	private void OnMouseUp() 
	{
		board.ReleaseTile();
	}
}
