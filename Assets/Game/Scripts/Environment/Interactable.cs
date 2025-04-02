using static Unity.Mathematics.math;
using System.Collections;
using UnityEngine;

// TODO - If the player exists to fast the scale down coroutine will not be called.
[RequireComponent(typeof(BoxCollider2D))]
public abstract class Interactable : MonoBehaviour {
	[Tooltip("The target transform to scale.")]
	[SerializeField] private Transform _target;

	private Vector3 _initTargetLocalScale;
	private IAudioService _audioService;

	private void Awake() {
		if (_target == null) {
			Debug.LogError("Interactable target not found, please assign a target to scale.");
		}

		_initTargetLocalScale = _target.localScale;
	}

	private void Start() {
		_audioService = Services.Instance.GetService<IAudioService>();
	}

	public abstract void Interact(PlayerData player);

	private void OnTriggerEnter2D(Collider2D trigger) {
		if (trigger.CompareTag("Player")) {
			StopCoroutine(ScaleDown());
			StartCoroutine(ScaleUp());
			_audioService.PlaySound("sfx_ph_interaction_enter");
		}
	}

	private void OnTriggerExit2D(Collider2D trigger) {
		if (trigger.CompareTag("Player")) {
			if (trigger.gameObject.activeSelf == false) {
				return; // Player is dead, no need to scale down.
			}

			StopCoroutine(ScaleUp());
			StartCoroutine(ScaleDown());
			_audioService.PlaySound("sfx_ph_interaction_exit");
		}
	}

	private IEnumerator ScaleUp() {
		// First, scale up the object.
		const float duration = 0.3f;
		float elapsedTime = 0.0f;
		var initScale = _initTargetLocalScale;
		var targetScale = _initTargetLocalScale * 1.5f;

		while (elapsedTime < duration) {
			float t = elapsedTime / duration;

			_target.localScale = lerp(initScale, targetScale, pow(t, 3.0f));

			elapsedTime += Time.deltaTime;
			yield return null;
		}

		_target.localScale = targetScale;
	}

	private IEnumerator ScaleDown() {
		const float duration = 0.1f;
		float elapsedTime = 0.0f;
		var initScale = _target.localScale;

		// Return to original scale over time.
		while (elapsedTime < duration) {
			float t = elapsedTime / duration;

			_target.localScale = lerp(initScale, _initTargetLocalScale, pow(t, 3.0f));

			elapsedTime += Time.deltaTime;
			yield return null;
		}

		_target.localScale = _initTargetLocalScale;
	}

	private void OnDrawGizmos() {
		var collider = GetComponent<BoxCollider2D>();

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
	}
}
