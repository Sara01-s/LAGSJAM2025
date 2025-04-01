using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using EasyTransition;
using UnityEngine.SceneManagement;

public class PlayerLifecycle : MonoBehaviour {
	[Header("References")]
	[SerializeField] private PlayerData _player;
	[SerializeField] private PlayerEvents _playerEvents;
	[SerializeField] private TransitionSettings _deathTransition;

	[Header("UI")]
	[SerializeField] private Transform _camera;
	[SerializeField] private GameObject _healthBarHolder;
	[SerializeField] private Image _healthBarImage;

	[Header("Health Bar Fade Settings")]
	[SerializeField] private float _fadeDelay = 2.0f; // Tiempo en segundos antes de comenzar a desvanecer
	[SerializeField] private float _fadeDuration = 1.0f; // Duración del desvanecimiento en segundos

	private CanvasGroup _healthBarCanvasGroup;
	private Coroutine _healthBarFadeCoroutine;

	private void Awake() {
		_healthBarCanvasGroup = _healthBarHolder.GetComponent<CanvasGroup>();
		_healthBarCanvasGroup.alpha = 0.0f;
		_player.Health = _player.MaxHealth;
		_player.IsFrozen = false;
	}

	private void OnEnable() {
		_playerEvents.OnPlayerHurt += HandlePlayerHurt;
	}

	private void OnDisable() {
		_playerEvents.OnPlayerHurt -= HandlePlayerHurt;
	}

	private void HandlePlayerHurt(DamageInfo damageInfo) {
		_player.Health -= damageInfo.DamageAmount;

		_healthBarCanvasGroup.alpha = 1.0f;
		_healthBarImage.fillAmount = _player.Health / _player.MaxHealth;

		if (_healthBarFadeCoroutine != null) {
			StopCoroutine(_healthBarFadeCoroutine);
		}
		_healthBarFadeCoroutine = StartCoroutine(FadeHealthBar());

		if (_player.Health <= 0) {
			_playerEvents.OnPlayerDeath?.Invoke();
			var scene = SceneManager.GetActiveScene();
			TransitionManager.Instance().Transition(scene.name, _deathTransition, startDelay: 0.0f);
			return;
		}

		if (damageInfo.ShakeCamera) {
			StartCoroutine(Shake(_camera, damageInfo));
			StartCoroutine(Shake(_healthBarHolder.transform, damageInfo));
		}
	}

	private IEnumerator FadeHealthBar() {
		yield return new WaitForSeconds(_fadeDelay);
		float elapsedTime = 0.0f;

		while (elapsedTime < _fadeDuration) {
			elapsedTime += Time.deltaTime;
			float alpha = Mathf.Lerp(1.0f, 0.0f, elapsedTime / _fadeDuration);

			_healthBarCanvasGroup.alpha = alpha;

			yield return null;
		}

		_healthBarCanvasGroup.alpha = 0.0f;
	}

	private IEnumerator Shake(Transform target, DamageInfo info) {
		Vector2 originalPosition = target.localPosition;
		float elapsedTime = 0.0f;

		while (elapsedTime < info.ShakeDuration) {
			float x = Random.Range(-info.ShakeMagnitude, info.ShakeMagnitude);
			float y = Random.Range(-info.ShakeMagnitude, info.ShakeMagnitude);

			target.localPosition = new Vector2(originalPosition.x + x, originalPosition.y + y);
			elapsedTime += Time.deltaTime;

			yield return null;
		}

		target.localPosition = originalPosition;
	}
}
