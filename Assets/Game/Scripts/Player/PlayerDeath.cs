using Unity.Cinemachine;
using UnityEngine;

public class PlayerDeath : MonoBehaviour {
	private BoxCollider2D currentZone;
	private void Start() {
		currentZone = GetComponentInChildren<PlayerCameraTransition>()
		.primaryCamera.GetComponentInParent<BoxCollider2D>();
	}
	public void SetCurrentZone(BoxCollider2D newZone) {
		currentZone = newZone;
	}
	public void Respawn() {
		Vector2 respawn = (Vector2)currentZone.GetComponentInChildren<ConfigureZone>().GetRespawnPoint();
		GetComponent<PlayerMovement>().Respawn(respawn);
	}
}
