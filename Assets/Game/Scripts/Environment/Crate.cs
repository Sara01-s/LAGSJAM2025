using Unity.VisualScripting;
using UnityEngine;

public class Crate : Interactable {
	private PlayerData _player;

	private Vector3 initialPosition;
	private int facing = 1;
	private bool active = false;
	[SerializeField, Min(0f)] private float offsetX;
	[SerializeField] private float offsetY;
	[SerializeField] private Transform box;
	public override void Interact(PlayerData player) {
		if (!active) {
			active = true;
			_player = player;
			facing = player.Position.x - box.position.x > 0 ? -1 : 1;
		}
		else {
			active = false;
			_player = null;
		}
	}
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	public override void Awake() {
		base.Awake();
		initialPosition = box.position;

	}

	// Update is called once per frame
	void Update() {
		if (_player != null & active) {
			TrackPlayer();
		}
	}
	private void TrackPlayer() {
		Vector3 newPos = _player.Position;
		float vel = _player.HorizontalVelocity;
		if (vel != 0) { facing = _player.HorizontalVelocity > 0 ? 1 : -1; }
		box.position = newPos + new Vector3(offsetX * facing, offsetY);
	}
	public override void OnTriggerEnter2D(Collider2D trigger) {
	}
	public override void OnTriggerExit2D(Collider2D trigger) {
	}
}
