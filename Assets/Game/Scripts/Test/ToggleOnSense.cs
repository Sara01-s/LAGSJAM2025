using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ToggleOnSense : MonoBehaviour {
	[SerializeField] private PlayerEvents _playerEvents;
	[SerializeField] private PlayerState _stateToToggleOn;
	private SpriteRenderer _renderer;

	private void Awake() {
		_renderer = GetComponent<SpriteRenderer>();
	}

	private void OnEnable() {
		_playerEvents.OnPlayerStateChange += ToggleVisibility;
	}

	private void OnDisable() {
		_playerEvents.OnPlayerStateChange -= ToggleVisibility;
	}

	private void ToggleVisibility(PlayerState state) {
		if (!state.HasFlag(PlayerState.CanSee)) {
			_renderer.color = Color.white;
		}
		else {
			_renderer.color = new Color(1.0f, 1.0f, 1.0f, 0.25f);
		}
	}
}
