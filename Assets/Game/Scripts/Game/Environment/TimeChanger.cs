using static Unity.Mathematics.math;
using System.Collections;
using UnityEngine;

public class TimeChanger : Interactable {
	[SerializeField] private float _newTime = 0.5f;

	[Tooltip("If the new time duration is 0, the time will be changed indefinitely.")]
	[SerializeField] private float _newTimeDurationSec = 1.0f;
	[SerializeField] private float _timeTransitionDurationSec = 1.0f;

	[SerializeField] private bool _needsMainInput;
	[SerializeField] private bool _needsInteractInput;

	private float _startTimeScale;

	private void Awake() {
		if (_needsMainInput && _needsInteractInput) {
			Debug.LogError("TimeChanger: Cannot have both needsMainInput and needsInteractInput set to true.");
		}

		_startTimeScale = Time.timeScale;
	}

	public override void OnInteractEnter(PlayerData player) {
		if (_needsMainInput) {
			StartCoroutine(ChangeTimeUntilInput(_newTime, player.Input.IsMainInputHeld, _newTimeDurationSec));
		}
		else if (_needsInteractInput) {
			StartCoroutine(ChangeTimeUntilInput(_newTime, player.Input.InteractWasPressedThisFrame, _newTimeDurationSec));
		}
		else if (_newTimeDurationSec <= 0.0f) {
			StartCoroutine(ChangeTimeGradually(_newTime, _timeTransitionDurationSec));
		}
		else {
			StartCoroutine(ChangeTimeTemporarily(_newTime, _newTimeDurationSec, _timeTransitionDurationSec));
		}
	}

	private IEnumerator ChangeTimeTemporarily(float newTime, float timeChangeDurationSec, float timeTransitionDurationSec = 1.0f) {
		yield return ChangeTimeGradually(newTime, timeTransitionDurationSec);
		yield return new WaitForSecondsRealtime(timeChangeDurationSec);
		yield return ChangeTimeGradually(_startTimeScale, timeTransitionDurationSec);
	}

	private IEnumerator ChangeTimeUntilInput(float newTime, bool input, float timeTransitionDurationSec = 1.0f) {
		yield return ChangeTimeGradually(newTime, timeTransitionDurationSec);
		yield return new WaitUntil(() => input);
		yield return ChangeTimeGradually(_startTimeScale, timeTransitionDurationSec);
	}

	private IEnumerator ChangeTimeGradually(float newTime, float timeTransitionDurationSec = 1.0f) {
		float startTimeScale = Time.timeScale;
		float targetTimeScale = newTime;
		float elapsedTime = 0.0f;

		while (elapsedTime < timeTransitionDurationSec) {
			float t = elapsedTime / timeTransitionDurationSec;
			float easeOutCubic = 1.0f - pow(1.0f - t, 3.0f);

			Time.timeScale = lerp(startTimeScale, targetTimeScale, easeOutCubic);
			elapsedTime += Time.unscaledDeltaTime;

			yield return null;
		}

		Time.timeScale = targetTimeScale;
	}

}
