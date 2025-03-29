using static Unity.Mathematics.math;
using UnityEngine;

[SelectionBase, RequireComponent(typeof(Rigidbody2D))]
internal sealed class PlayerMovement : MonoBehaviour {

	[Header("References")]
	[SerializeField] private PlayerData _player;
	[SerializeField] private PlayerEvents _playerEvents;

	private Rigidbody2D _body;

	private float _jumpVelocity;
	private float _jumpGravity;
	private float _fallGravity;

	private void Awake() {
		_body = GetComponent<Rigidbody2D>();

		_jumpVelocity = 2.0f * _player.JumpHeight / _player.SecondsToJumpPeak;
		_jumpGravity = -2.0f * _player.JumpHeight / pow(_player.SecondsToJumpPeak, 2.0f);
		_fallGravity = -2.0f * _player.JumpHeight / pow(_player.SecondsToLand, 2.0f);
	}

	private void OnEnable() {
		_playerEvents.Input.OnHorizontalHeld += CalculateHorizontalVelocity;
		_playerEvents.Input.OnHorizontalReleased += StopMovement;
		_playerEvents.Input.OnJumpPressed += Jump;
	}

	private void OnDisable() {
		_playerEvents.Input.OnHorizontalHeld -= CalculateHorizontalVelocity;
		_playerEvents.Input.OnHorizontalReleased -= StopMovement;
		_playerEvents.Input.OnJumpPressed -= Jump;
	}

	private void Update() {
		_player.Position = transform.position;
	}

	private void FixedUpdate() {
		if (_player.State.HasFlag(PlayerState.Dead)) {
			_body.linearVelocity = Vector2.zero;
			return;
		}

		float gravity = _body.linearVelocityY > 0.0f ? _jumpGravity : _fallGravity;
		_body.linearVelocityY += gravity * Time.fixedDeltaTime;

		_body.linearVelocityX = _player.HorizontalVelocity;
	}

	private void StopMovement() {
		_player.HorizontalVelocity = 0.0f;
	}

	private void CalculateHorizontalVelocity(float horizontalInput) {
		var velocity = horizontalInput * _player.Speed;
		var clampedVelocity = clamp(velocity, -_player.Speed, _player.Speed);

		_player.HorizontalVelocity = clampedVelocity;
		print("Moviéndose.");
	}

	private void Jump() {
		bool isMovingVertically = _body.linearVelocityY > 0.1f;
		if (isMovingVertically || !_player.IsGrounded) {
			return;
		}

		_body.linearVelocityY += _jumpVelocity;
		print($"Linear velocity Y: {_body.linearVelocityY}");
	}

}
