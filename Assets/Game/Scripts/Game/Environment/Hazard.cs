using UnityEngine;

public class Hazard : Interactable {
	public override void OnInteractEnter(PlayerData player) {
		player.Events.OnDeath?.Invoke(player.LastCheckpointNsp, player.LastCheckpointNormalOffset);
		Debug.Log("Player has died.");
	}
}
