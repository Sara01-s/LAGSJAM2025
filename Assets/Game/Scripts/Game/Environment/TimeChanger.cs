using static Unity.Mathematics.math;
using System.Collections;
using UnityEngine;

public class TimeChanger : Interactable {
	[SerializeField] private float _timeChange = 0.5f;
	[SerializeField] private float _timeChangeDurationSec = 1.0f;
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
			StartCoroutine(ChangeTimeUntilInput(_timeChange, player.Input.IsMainInputHeld, _timeChangeDurationSec));
		}
		else if (_needsInteractInput) {
			StartCoroutine(ChangeTimeUntilInput(_timeChange, player.Input.InteractWasPressedThisFrame, _timeChangeDurationSec));
		}
		else {
			StartCoroutine(ChangeTimeTemporarily(_timeChange, _timeChangeDurationSec, _timeTransitionDurationSec));
		}
	}

	private IEnumerator ChangeTimeTemporarily(float timeChange, float timeChangeDurationSec, float timeTransitionDurationSec = 1.0f) {
		yield return ChangeTimeGradually(timeChange, timeTransitionDurationSec);
		yield return new WaitForSeconds(timeChangeDurationSec);
		yield return ChangeTimeGradually(_startTimeScale, timeTransitionDurationSec);
	}

	private IEnumerator ChangeTimeUntilInput(float timeChange, bool input, float timeTransitionDurationSec = 1.0f) {
		yield return ChangeTimeGradually(timeChange, timeTransitionDurationSec);
		yield return new WaitUntil(() => input);
		yield return ChangeTimeGradually(_startTimeScale, timeTransitionDurationSec);
	}

	private IEnumerator ChangeTimeGradually(float timeChange, float timeTransitionDurationSec = 1.0f) {
		float startTimeScale = Time.timeScale;
		float targetTimeScale = startTimeScale * timeChange;
		float elapsedTime = 0.0f;

		while (elapsedTime < timeTransitionDurationSec) {
			float t = elapsedTime / timeTransitionDurationSec;
			float easeOutCubic = 1.0f - pow(1.0f - t, 3.0f);

			Time.timeScale = lerp(startTimeScale, targetTimeScale, easeOutCubic);
			elapsedTime += Time.unscaledDeltaTime;

			yield return null;
		}
	}

}
