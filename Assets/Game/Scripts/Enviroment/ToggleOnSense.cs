using UnityEngine;

public class ToggleOnSense : MonoBehaviour {
	[SerializeField] private PlayerEvents _playerEvents;

	[Tooltip("This gameobject will be active only in the specified state.")]
	[SerializeField] private PlayerState _state;

	private void OnEnable() => _playerEvents.OnPlayerStateChanged += OnPlayerStateChanged;
	private void OnDisable() => _playerEvents.OnPlayerStateChanged -= OnPlayerStateChanged;

	private void OnPlayerStateChanged(PlayerState state) {
		gameObject.SetActive(state == _state);
	}
}
