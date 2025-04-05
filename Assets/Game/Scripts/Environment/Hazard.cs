using System;
using UnityEngine;

[Serializable]
public struct DamageInfo {
	[Header("Damage")]
	public PlayerState ActiveState;
	public float DamageAmount;

	[Header("Knockback")]
	public bool IsKnockback;
	public float KnockbackForce;
	public float KnockbackDuration;

	[Header("Damage Over Time")]
	public bool IsDamageOverTime;
	public float DamageInterval;

	[Header("Camera Shake Settings")]
	public bool ShakeCamera;
	public float ShakeDuration;
	public float ShakeMagnitude;
}

public class Hazard : MonoBehaviour {
	[Header("References")]
	[SerializeField] private PlayerData _player;

	[Header("Damage Settings")]
	[SerializeField] private DamageInfo _damageInfo;

	private float _currentDamageInterval = 0.0f;

	private void OnTriggerStay2D(Collider2D trigger) {
		if (!_player.State.HasFlag(_damageInfo.ActiveState) || !_damageInfo.IsDamageOverTime) {
			return;
		}

		if (trigger.CompareTag("Player")) {
			if (_currentDamageInterval > 0) {
				_currentDamageInterval -= Time.deltaTime;
				return;
			}

			_currentDamageInterval = _damageInfo.DamageInterval;
			_player.Events.OnPlayerHurt?.Invoke(_damageInfo);
		}
	}

	private void OnTriggerEnter2D(Collider2D trigger) {
		if (!_player.State.HasFlag(_damageInfo.ActiveState)) {
			return;
		}

		if (trigger.CompareTag("Player")) {
			_player.Events.OnPlayerHurt?.Invoke(_damageInfo);
		}
	}

	private void OnDrawGizmos() {
		var collider = GetComponent<Collider2D>();

		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
	}
}
