using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class PlayerCollision : MonoBehaviour {
	[SerializeField] private PlayerData _player;

	private bool _wasGroundedLastFrame;

	private void OnEnable() {
		_player.Events.Input.OnInteractPressed += CheckForInterctables;
	}

	private void OnDisable() {
		_player.Events.Input.OnInteractPressed -= CheckForInterctables;
	}

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
				_player.Events.OnPlayerLand?.Invoke();
			}
		}

		_wasGroundedLastFrame = _player.IsGrounded;
	}

	private void CheckForInterctables() {
		var boxOrigin = _player.Position + _player.GroundCheckBoxOffset;
		var boxScale = Vector2.right * _player.GroundCheckBox.x + Vector2.up * _player.GroundCheckBox.y;
		var colliders = Physics2D.OverlapBoxAll(boxOrigin, boxScale, angle: 0.0f);

		foreach (var collider in colliders) {
			if (collider.TryGetComponent<Interactable>(out var interactable)) {
				interactable.Interact(_player);
				_player.Events.OnPlayerInteract?.Invoke(interactable);
			}
		}
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
