using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealthBar : MonoBehaviour {
	[Header("References")]
	[SerializeField] private PlayerData _player;

	[Header("UI")]
	[SerializeField] private GameObject _healthBarHolder;
	[SerializeField] private Image _healthBarImage;

	[Header("Health Bar Fade Settings")]
	[SerializeField] private float _fadeDelay = 2.0f;
	[SerializeField] private float _fadeDuration = 1.0f;
	[SerializeField] private bool _shakeHealthBar;

	private CanvasGroup _healthBarCanvasGroup;
	private Coroutine _healthBarFadeCoroutine;

	private void Awake() {
		_healthBarCanvasGroup = _healthBarHolder.GetComponent<CanvasGroup>();
		_healthBarCanvasGroup.alpha = 0.0f;
	}

	private void OnEnable() {
		_player.Events.OnHealthChanged += UpdateHealthBar;
	}

	private void OnDisable() {
		_player.Events.OnHealthChanged -= UpdateHealthBar;
	}

	private void UpdateHealthBar(float newHealth, float _) {
		_healthBarCanvasGroup.alpha = 1.0f;
		_healthBarImage.fillAmount = newHealth / _player.MaxHealth;

		if (_shakeHealthBar) {
			StartCoroutine(ShakeHealthBar());
		}

		if (_healthBarFadeCoroutine != null) {
			StopCoroutine(_healthBarFadeCoroutine);
		}

		_healthBarFadeCoroutine = StartCoroutine(FadeHealthBar());
	}

	private IEnumerator ShakeHealthBar() {
		Vector2 originalPosition = _healthBarHolder.transform.localPosition;
		float elapsedTime = 0.0f;
		float shakeDuration = 0.5f;
		float shakeMagnitude = 0.1f;

		while (elapsedTime < shakeDuration) {
			float x = Random.Range(-shakeMagnitude, shakeMagnitude);
			float y = Random.Range(-shakeMagnitude, shakeMagnitude);
			_healthBarHolder.transform.localPosition = new Vector2(originalPosition.x + x, originalPosition.y + y);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		_healthBarHolder.transform.localPosition = originalPosition;
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
}
