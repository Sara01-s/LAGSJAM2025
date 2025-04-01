using UnityEngine;

public class PlayerSenses : MonoBehaviour {
	[SerializeField] private WorldData _world;
	[SerializeField] private PlayerData _playerData;
	[SerializeField] private PlayerEvents _playerEvents;

	private void Awake() {
		_playerData.State = _playerData.InitialState;
		Debug.Log($"Player state initialized: {_playerData.State}");
	}

	private void Start() {
		// Send initial state.
		_playerEvents.OnPlayerStateChanged?.Invoke(_playerData.State);
		UpdateSenseTogglables();
	}

	private void OnEnable() {
		_playerEvents.Input.OnSightPressed += OnSightPressed;
		_playerEvents.Input.OnSmellPressed += OnSmellPressed;
		_playerEvents.Input.OnTastePressed += OnTastePressed;
		_playerEvents.Input.OnTouchPressed += OnTouchPressed;
		_playerEvents.Input.OnHearingPressed += OnHearingPressed;
	}

	private void OnDisable() {
		_playerEvents.Input.OnSightPressed -= OnSightPressed;
		_playerEvents.Input.OnSmellPressed -= OnSmellPressed;
		_playerEvents.Input.OnTastePressed -= OnTastePressed;
		_playerEvents.Input.OnTouchPressed -= OnTouchPressed;
		_playerEvents.Input.OnHearingPressed -= OnHearingPressed;
	}

	private void OnSightPressed() => ToggleState(PlayerState.CanSee);
	private void OnSmellPressed() => ToggleState(PlayerState.CanSmell);
	private void OnTastePressed() => ToggleState(PlayerState.CanTaste);
	private void OnTouchPressed() => ToggleState(PlayerState.CanTouch);
	private void OnHearingPressed() => ToggleState(PlayerState.CanHear);

	private void ToggleState(PlayerState state) {
		int activeStatesCount = CountActiveFlags((int)_playerData.State);

		const int minActiveStates = 3;
		const int maxActiveStates = 4;

		if (_playerData.State.HasFlag(state)) {
			if (activeStatesCount <= minActiveStates) {
				Debug.LogWarning("Cannot deactivate more than 2 states at the same time.");
				return;
			}

			_playerData.State &= ~state;
		}
		else {
			if (activeStatesCount >= maxActiveStates) {
				Debug.LogWarning("At least 1 state must remain inactive.");
				return;
			}

			_playerData.State |= state;
		}

		_playerEvents.OnPlayerStateChanged?.Invoke(_playerData.State);
		UpdateSenseTogglables();

		// Debug stuff.
		int newActiveStatesCount = CountActiveFlags((int)_playerData.State);
		Debug.Log($"New player state: ({newActiveStatesCount}) {_playerData.State}");
	}

	private void UpdateSenseTogglables() {
		foreach (var togglable in _world.GetSenseTogglables()) {
			if (_playerData.State.HasFlag(togglable.State)) {
				togglable.gameObject.SetActive(true);
			}
			else {
				togglable.gameObject.SetActive(false);
			}
		}
	}

	// Thanks to,
	// src: https://github.com/MarioSieg/Bit-Twiddling-Hacks-Collection/blob/master/bithax.h#:~:text=%7D-,//%20Kerninghan's%20population%20count%20(popcnt).,%7D,-//%20Counts%20the
	private static int CountActiveFlags(int x) {
		int count = 0;
		while (x != 0) {
			x &= (x - 1); // Zero the lowest 1 bit as long as x is not zero.
			count++;
		}

		return count;
	}

}
