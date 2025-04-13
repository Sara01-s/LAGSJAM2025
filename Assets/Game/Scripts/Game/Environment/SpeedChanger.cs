using static Unity.Mathematics.math;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SpeedChanger : Interactable {
	[SerializeField] private float _speedChange = 0.0f;
	[SerializeField] private bool _needsInteraction = false;

	[Tooltip("If boost duration is 0, the speed change is permanent.")]
	[SerializeField, Min(0.0f)] private float _boostDurationSec = 0.0f;

	private void Awake() {
		GetComponent<Collider2D>().isTrigger = true;
	}

	public override void Interact(PlayerData player) {
		if (!_needsInteraction) {
			return;
		}

		StartCoroutine(ChangeSpeed(player, _speedChange, _boostDurationSec));
	}

	public override void OnInteractEnter(PlayerData player) {
		if (_needsInteraction) {
			return;
		}

		Debug.Log($"SpeedChanger {_speedChange} entered by {player.name}.");
		StartCoroutine(ChangeSpeed(player, _speedChange, _boostDurationSec));
	}

	private IEnumerator ChangeSpeed(PlayerData player, float speedChange, float speedChangeDurationSec = 0.0f) {
		if (speedChangeDurationSec <= 0.0f) {
			yield return ChangeSpeedGradually(player, speedChange);
		}
		else {
			yield return ChangeSpeedGradually(player, _speedChange);
			yield return new WaitForSeconds(_boostDurationSec);
			yield return ChangeSpeedGradually(player, -_speedChange);
		}
	}

	private IEnumerator ChangeSpeedGradually(PlayerData player, float speedChange) {
		const float speedChangeDurationSec = 1.0f;
		float targetSpeed = player.Speed + speedChange;
		float elapsedTime = 0.0f;

		while (elapsedTime < speedChangeDurationSec) {
			float t = elapsedTime / speedChangeDurationSec;
			float easeOutCubic = 1.0f - pow(1.0f - t, 3.0f);

			player.Speed = lerp(player.Speed, targetSpeed, easeOutCubic);
			elapsedTime += Time.deltaTime;

			yield return null;
		}

		player.Speed = targetSpeed;
	}
}
