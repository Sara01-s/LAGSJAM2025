using static Unity.Mathematics.math;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

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
		_playerEvents.OnPlayerHurt += HandleHurt;
	}

	private void OnDisable() {
		_playerEvents.Input.OnHorizontalHeld -= CalculateHorizontalVelocity;
		_playerEvents.Input.OnHorizontalReleased -= StopMovement;
		_playerEvents.Input.OnJumpPressed -= Jump;
		_playerEvents.OnPlayerHurt -= HandleHurt;
	}

	private void Update() {
		_player.Position = transform.position;
	}

	private void FixedUpdate() {
		if (_player.IsFrozen) {
			return;
		}

		float gravity = _body.linearVelocityY > 0.0f ? _jumpGravity : _fallGravity;

		_body.linearVelocityX = _player.HorizontalVelocity;
		_body.linearVelocityY += gravity * Time.fixedDeltaTime;
	}

	private void StopMovement() {
		_player.HorizontalVelocity = 0.0f;
	}

	private void CalculateHorizontalVelocity(float horizontalInput) {
		var velocity = horizontalInput * _player.Speed;
		var clampedVelocity = clamp(velocity, -_player.Speed, _player.Speed);

		_player.HorizontalVelocity = clampedVelocity;
	}

	private void Jump() {
		bool isMovingVertically = _body.linearVelocityY > 0.1f;
		if (isMovingVertically || !_player.IsGrounded) {
			return;
		}

		_body.linearVelocityY += _jumpVelocity;
	}

	public void Respawn(Vector2 vector) {
		transform.position = vector;
		_player.HorizontalVelocity = 0;
		_body.linearVelocityY = 0;
	}

	private void HandleHurt(DamageInfo damageInfo) {
		if (damageInfo.IsKnockback) {
			StartCoroutine(_HandleHurt(damageInfo));
		}

		IEnumerator _HandleHurt(DamageInfo info) {
			var knockback = new Vector2(x: info.KnockbackForce * 1.0f - sign(_player.HorizontalVelocity),
										y: info.KnockbackForce);

			_player.IsFrozen = true;
			_body.linearVelocity = Vector2.zero;
			_body.AddForce(knockback, ForceMode2D.Impulse);

			yield return new WaitForSeconds(info.KnockbackDuration);
			_player.IsFrozen = false;
		}
	}

}
