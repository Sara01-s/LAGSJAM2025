using System;
using UnityEngine;

[Serializable]
public struct DamageInfo {
	[Header("Damage")]
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
	[SerializeField] private PlayerEvents _playerEvents;

	[Header("Damage Settings")]
	[SerializeField] private DamageInfo _damageInfo;

	private void OnTriggerEnter2D(Collider2D trigger) {
		if (trigger.CompareTag("Player")) {
			_playerEvents.OnPlayerHurt?.Invoke(_damageInfo);
		}
	}

	private void OnDrawGizmos() {
		var collider = GetComponent<Collider2D>();

		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
	}
}
