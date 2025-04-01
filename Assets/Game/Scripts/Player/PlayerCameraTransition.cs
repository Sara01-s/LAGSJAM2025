using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using Unity.VisualScripting;
public class PlayerCameraTransition : MonoBehaviour {
	public string triggerTag;
	public CinemachineCamera primaryCamera;
	public CinemachineCamera[] allCameras;
	private void Start() {
		switchToCamera(primaryCamera);
	}
	private void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Damage")) {
			print("damage");
			GetComponentInParent<PlayerDeath>().Respawn();
			return;
		}
		if (other.CompareTag(triggerTag)) {
			CinemachineCamera target = other.GetComponentInChildren<CinemachineCamera>();
			PlayerDeath player = GetComponentInParent<PlayerDeath>();
			BoxCollider2D newZone = other.GetComponent<BoxCollider2D>();
			switchToCamera(target);
			if (newZone != null) {
				player.SetCurrentZone(newZone);
			}
			else { print("no new zone"); }
		}
	}
	private void switchToCamera(CinemachineCamera target) {
		foreach (var cam in allCameras) {
			cam.enabled = cam == target;
			//cam.GetComponent<BoxCollider2D>().isTrigger = cam == target;
		}
	}
}
