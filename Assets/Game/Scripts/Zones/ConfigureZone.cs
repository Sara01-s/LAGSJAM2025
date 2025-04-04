using System;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ConfigureZone : MonoBehaviour {
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	public CircleCollider2D respawnPoint;
	public Tilemap map;
	public BoxCollider2D cameraBounds;
	public BoxCollider2D cameraTrigger;
	public CinemachineConfiner2D confiner;
	[Range(0, 1)]
	public int tileOffsetLeft;
	[Range(0, 1)]
	public int tileOffsetRight;
	[Range(0, 1)]
	public int tileOffsetTop;
	[Range(0, 1)]
	public int tileOffsetBottom;
	[Range(30, 50)]
	public int skySize = 40;
	[Range(10, 50)]
	public int groundSize = 30;
	void Start() {
		SetBounds();
	}
	private void SetBounds() {
		map.CompressBounds();
		Vector3Int mapBounds = map.cellBounds.size;
		float tileSize = map.cellSize.x;

		Vector2 offset = new Vector2(
			(float)((tileOffsetRight - tileOffsetLeft) / 2.0 + map.cellBounds.center.x),
			(float)((tileOffsetTop - tileOffsetBottom + skySize - groundSize) / 2.0 + map.cellBounds.center.y))
			* tileSize;
		Vector2 size = new Vector2(
			mapBounds.x + tileOffsetRight + tileOffsetLeft,
			mapBounds.y + tileOffsetTop + tileOffsetBottom + skySize + groundSize);
		cameraBounds.offset = offset;
		cameraBounds.size = size * tileSize;
		cameraTrigger.offset = offset * tileSize;
		cameraTrigger.size = (size - new Vector2(2, 2)) * tileSize;
		confiner.InvalidateBoundingShapeCache();
	}
	public Vector2 GetRespawnPoint() {
		return respawnPoint.transform.position;
	}
}

