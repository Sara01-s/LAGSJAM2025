using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerCollision : MonoBehaviour {
	[SerializeField] private PlayerData _player;
	[SerializeField] private float _interactRadius;

	private void Update() {
		if (_player.Input.InteractWasPressedThisFrame) {
			Collider2D[] foundColliders = Physics2D.OverlapCircleAll(transform.position, _interactRadius);

			foreach (var collider in foundColliders) {
				if (collider.TryGetComponent<Interactable>(out var interactable)) {
					interactable.Interact(_player);
					break;
				}
			}

			Debug.Log($"No interactable found in a {_interactRadius} radius.");
		}
	}

	private void OnTriggerEnter2D(Collider2D trigger) {
		if (trigger.CompareTag("Interactable")) {
			if (trigger.TryGetComponent<Interactable>(out var interactable)) {
				interactable.OnInteractEnter(_player);
			}
		}
	}

	private void OnTriggerExit2D(Collider2D trigger) {
		if (trigger.CompareTag("Interactable")) {
			if (trigger.TryGetComponent<Interactable>(out var interactable)) {
				interactable.OnInteractExit(_player);
			}
		}
	}

	private void OnTriggerStay2D(Collider2D trigger) {
		if (trigger.CompareTag("Interactable")) {
			if (trigger.TryGetComponent<Interactable>(out var interactable)) {
				interactable.OnInteractStay(_player);
			}
		}
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, _interactRadius);
	}
}
