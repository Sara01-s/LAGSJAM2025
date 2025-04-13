using UnityEngine;

public class FloorAndCeil : MonoBehaviour {
	[SerializeField] private PlayerData _player;
	[SerializeField] private GameObject _floor;
	[SerializeField] private GameObject _ceil;

	private void Update() {
		transform.rotation = Quaternion.Euler(0.0f, 0.0f, _player.RollAngles + 90.0f);
	}
}
