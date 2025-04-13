using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class Checkpoint : Interactable {
	[SerializeField] private SplineContainer _splinePath;

	public override void OnInteractEnter(PlayerData player) {
		// Get Normalized spline position of the checkpoint.
		SplineUtility.GetNearestPoint(_splinePath.Spline, transform.position, out float3 _, out float nsp);
		player.LastCheckpointNsp = nsp;
		player.LastCheckpointNormalOffset = player.NormalOffset;

		Debug.Log($"Checkpoint {nsp} entered by {player.name}.");
	}
}
