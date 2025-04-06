using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using Unity.VisualScripting;
public class PlayerCameraTransition : MonoBehaviour {
	public string triggerTag;
	public CinemachineCamera primaryCamera;
	public CinemachineCamera[] allCameras;
	private void Awake() {
		primaryCamera.Prioritize();

	}
	private void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag(triggerTag)) {
			print("trigger change");
			CinemachineCamera target = other.GetComponentInChildren<CinemachineCamera>();
			PlayerMovement player = GetComponentInParent<PlayerMovement>();
			ConfigureZone newZone = other.GetComponentInParent<ConfigureZone>();
			switchToCamera(target);
			if (newZone != null && player != null) {
				player.SetRespawnPoint(newZone.GetRespawnPoint());
			}
		}
	}
	private void switchToCamera(CinemachineCamera target) {
		foreach (var cam in allCameras) {
			if (cam == target) {
				cam.Prioritize();
				return;
			}
		}
	}
}
