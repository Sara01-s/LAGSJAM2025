using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class PlayerCollision : MonoBehaviour {
	[SerializeField] private PlayerData _player;
	[SerializeField] private PlayerEvents _playerEvents;

	private bool _wasGroundedLastFrame;

	private void Update() {
		_player.IsGrounded = CheckIfGrounded();
		CheckIfLanded();
	}

	private bool CheckIfGrounded() {
		var boxOrigin = _player.Position + _player.GroundCheckBoxOffset;
		var boxScale = Vector2.right * _player.GroundCheckBox.x + Vector2.up * _player.GroundCheckBox.y;
		return Physics2D.OverlapBox(boxOrigin, boxScale, angle: 0.0f, layerMask: _player.GroundLayer);
	}

	private void CheckIfLanded() {
		if (_player.IsGrounded != _wasGroundedLastFrame) {
			if (_player.IsGrounded) {
				_playerEvents.OnPlayerLand?.Invoke();
			}
		}

		_wasGroundedLastFrame = _player.IsGrounded;
	}

	// Debug.
	private void OnDrawGizmos() {
		Vector2 playerPos = FindFirstObjectByType<PlayerMovement>().transform.position;

		Gizmos.color = Color.green;
		var boxScale = Vector2.right * _player.GroundCheckBox.x + Vector2.up * _player.GroundCheckBox.y;
		var boxOrigin = playerPos + _player.GroundCheckBoxOffset;

		Gizmos.DrawWireCube(boxOrigin, boxScale);
	}

}
