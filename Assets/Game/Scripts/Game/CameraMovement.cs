using static Unity.Mathematics.math;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
	[SerializeField] private PlayerData _player;
	[SerializeField] private CinemachineCamera _camera;
	[SerializeField] private CinemachineSplineDolly _dollyCart;

	private void OnEnable() {
		_player.Events.OnSpeedChanged += ChangeOrtographicSize;
		_player.Input.OnInteractPressed += TryShakeCamera;
	}

	private void OnDisable() {
		_player.Events.OnSpeedChanged -= ChangeOrtographicSize;
		_player.Input.OnInteractPressed -= TryShakeCamera;
	}

	private void Update() {
		_dollyCart.CameraPosition = _player.Nsp;
	}

	private void ChangeOrtographicSize(float newSpeed, float oldSpeed) {
		if (newSpeed > oldSpeed) {
			StartCoroutine(ChangeCameraOrtographicSizeGradually(20.0f));
		}
		else if (newSpeed < oldSpeed) {
			StartCoroutine(ChangeCameraOrtographicSizeGradually(15.0f));
		}
	}

	private IEnumerator ChangeCameraOrtographicSizeGradually(float newOrtographicSize) {
		const float speedChangeDurationSec = 1.0f;
		float startOrtographicSize = _camera.Lens.OrthographicSize;
		float targetOrtographicSize = newOrtographicSize;
		float elapsedTime = 0.0f;

		while (elapsedTime < speedChangeDurationSec) {
			float t = elapsedTime / speedChangeDurationSec;
			float easeOutCubic = 1.0f - pow(1.0f - t, 3.0f);

			_camera.Lens.OrthographicSize = lerp(startOrtographicSize, targetOrtographicSize, easeOutCubic);
			elapsedTime += Time.deltaTime;

			yield return null;
		}

		_camera.Lens.OrthographicSize = targetOrtographicSize;
	}

	private void TryShakeCamera() {
		const float intensity = 0.1f;
		const float duration = 0.2f;

		if (_player.HasState(PlayerState.OnInputGoalMinigame)) {
			StartCoroutine(ChangeCameraOrtographicSizeGradually(_camera.Lens.OrthographicSize - 0.5f));
			ShakeCamera(intensity, duration);
		}
	}

	private void ShakeCamera(float intensity, float duration) {
		StartCoroutine(_ShakeCamera(intensity, duration));

		IEnumerator _ShakeCamera(float intensity, float duration) {
			Vector3 originalPosition = _camera.transform.localPosition;
			float elapsed = 0.0f;

			while (elapsed < duration) {
				Vector2 randomOffset = Random.insideUnitCircle * intensity;

				_camera.transform.localPosition = new Vector3(randomOffset.x, randomOffset.y, originalPosition.z);

				elapsed += Time.deltaTime;
				yield return null;
			}

			_camera.transform.localPosition = originalPosition;
		}
	}
}
