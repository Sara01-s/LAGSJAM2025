using static Unity.Mathematics.math;
using System.Collections;
using UnityEngine;

public class TipShower : Interactable {
	[SerializeField] private GameObject _tip;

	[Header("If duration is 0, the tip will show indefinitely.")]
	[SerializeField, Min(0.0f)] private float _displayDuration = 5.0f;

	[Header("If true, the tip will show until the player presses the specified button.")]
	[SerializeField] private bool _needsMainInput = false;
	[SerializeField] private bool _needsInteractInput = false;

	private void Awake() {
		if (_tip == null) {
			Debug.LogError("TipShower: Tip GameObject is not assigned.");
			return;
		}

		if (_needsMainInput && _needsInteractInput) {
			Debug.LogError("TimeChanger: Cannot have both needsMainInput and needsInteractInput set to true.");
		}
	}

	public override void OnInteractEnter(PlayerData player) {
		var tipCanvasGroup = _tip.GetComponent<CanvasGroup>();

		if (_needsMainInput) {
			StartCoroutine(ChangeTipAlphaUntilInput(tipCanvasGroup, 1.0f, 1.0f, player.Input.IsMainInputHeld));
			return;
		}

		if (_needsInteractInput) {
			StartCoroutine(ChangeTipAlphaUntilInput(tipCanvasGroup, 1.0f, 1.0f, player.Input.InteractWasPressedThisFrame));
			return;
		}

		if (_displayDuration >= 0.0f) {
			StartCoroutine(ChangeTipAlphaTemporarily(tipCanvasGroup, targetAlpha: 1.0f, fadeDurationSec: 1.0f));
		}
		else {
			StartCoroutine(ChangeTipAlphaGradually(tipCanvasGroup, targetAlpha: 1.0f, transitionDuration: 1.0f));
		}
	}

	private IEnumerator ChangeTipAlphaTemporarily(CanvasGroup tipCanvasGroup, float targetAlpha, float fadeDurationSec) {
		yield return ChangeTipAlphaGradually(tipCanvasGroup, targetAlpha, fadeDurationSec);
		yield return new WaitForSeconds(_displayDuration);
		yield return ChangeTipAlphaGradually(tipCanvasGroup, 0.0f, fadeDurationSec);
	}

	private IEnumerator ChangeTipAlphaUntilInput(CanvasGroup tipCanvasGroup, float targetAlpha, float fadeDurationSec, bool input) {
		yield return ChangeTipAlphaGradually(tipCanvasGroup, targetAlpha, fadeDurationSec);
		yield return new WaitUntil(() => input);
		yield return ChangeTipAlphaGradually(tipCanvasGroup, 0.0f, fadeDurationSec);
	}

	private IEnumerator ChangeTipAlphaGradually(CanvasGroup tipCanvasGroup, float targetAlpha, float transitionDuration) {
		float startAlpha = tipCanvasGroup.alpha;
		float elapsedTime = 0.0f;

		while (elapsedTime < transitionDuration) {
			float t = elapsedTime / transitionDuration;
			float easeOutCubic = 1.0f - pow(1.0f - t, 3.0f);

			tipCanvasGroup.alpha = lerp(startAlpha, targetAlpha, easeOutCubic);

			elapsedTime += Time.deltaTime;
			yield return null;
		}

		tipCanvasGroup.alpha = targetAlpha;
	}

}
