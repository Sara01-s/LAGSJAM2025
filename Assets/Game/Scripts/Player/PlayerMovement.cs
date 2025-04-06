using static Unity.Mathematics.math;
using UnityEngine;
using System.Collections;

[SelectionBase, RequireComponent(typeof(Rigidbody2D))]
internal sealed class PlayerMovement : MonoBehaviour {

	[Header("References")]
	[SerializeField] private PlayerData _player;

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

	private void Start() {
		_body.bodyType = RigidbodyType2D.Dynamic;
		_player.IsFrozen = false;
		transform.position = _player.RespawnPoint;
	}

	private void OnEnable() {
		_player.Events.Input.OnHorizontalHeld += CalculateHorizontalVelocity;
		_player.Events.Input.OnHorizontalReleased += StopMovement;
		_player.Events.OnPlayerDeath += FreezePlayer;
		_player.Events.Input.OnJumpPressed += Jump;
		_player.Events.OnPlayerHurt += HandleHurt;
	}

	private void OnDisable() {
		_player.Events.Input.OnHorizontalHeld -= CalculateHorizontalVelocity;
		_player.Events.Input.OnHorizontalReleased -= StopMovement;
		_player.Events.OnPlayerDeath -= FreezePlayer;
		_player.Events.Input.OnJumpPressed -= Jump;
		_player.Events.OnPlayerHurt -= HandleHurt;
	}

	private void Update() {
		// This is the source of truth for the player's position.
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

	private void FreezePlayer() {
		_player.HorizontalVelocity = 0.0f;
		_body.bodyType = RigidbodyType2D.Kinematic;
		_player.IsFrozen = true;
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
		_player.Events.OnPlayerJump?.Invoke();
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
	public void SetRespawnPoint(Vector2 respawn) {
		_player.RespawnPoint = respawn;
	}
}
