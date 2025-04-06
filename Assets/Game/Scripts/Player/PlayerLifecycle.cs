using UnityEngine;
using UnityEngine.SceneManagement;
using EasyTransition;
using System.Collections;

public class PlayerLifecycle : MonoBehaviour {
	[Header("References")]
	[SerializeField] private PlayerData _player;
	[SerializeField] private TransitionSettings _deathTransition;

	[Header("UI")]
	[SerializeField] private Transform _camera;

	private void Awake() {
		_player.Health = _player.MaxHealth;
		_player.IsFrozen = false;
	}

	private void OnEnable() {
		_player.Events.OnPlayerHurt += HandlePlayerHurt;
	}

	private void OnDisable() {
		_player.Events.OnPlayerHurt -= HandlePlayerHurt;
	}

	private void HandlePlayerHurt(DamageInfo damageInfo) {
		_player.Health -= damageInfo.DamageAmount;
		_player.Events.OnHealthChanged?.Invoke(_player.Health, _player.Health + damageInfo.DamageAmount);

		if (_player.Health <= 0) {
			_player.Events.OnPlayerDeath?.Invoke();
			var scene = SceneManager.GetActiveScene();

			TransitionManager.Instance.Transition(scene.name, _deathTransition, startDelay: 0.0f);
			return;
		}

		if (damageInfo.ShakeCamera) {
			StartCoroutine(Shake(_camera, damageInfo));
		}
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
