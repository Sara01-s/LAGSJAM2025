using UnityEngine;

public class PlayerSenses : MonoBehaviour {
	[SerializeField] private WorldData _world;
	[SerializeField] private PlayerData _player;

	private void Awake() {
		_player.State = _player.InitialState;
		Debug.Log($"Player state initialized: {_player.State}");
	}

	private void Start() {
		// Send initial state.
		_player.Events.OnPlayerStateChanged?.Invoke(_player.State);
		UpdateSenseTogglables();
	}

	private void OnEnable() {
		_player.Events.Input.OnSightPressed += OnSightPressed;
		_player.Events.Input.OnSmellPressed += OnSmellPressed;
		_player.Events.Input.OnTastePressed += OnTastePressed;
		_player.Events.Input.OnTouchPressed += OnTouchPressed;
		_player.Events.Input.OnHearingPressed += OnHearingPressed;
	}

	private void OnDisable() {
		_player.Events.Input.OnSightPressed -= OnSightPressed;
		_player.Events.Input.OnSmellPressed -= OnSmellPressed;
		_player.Events.Input.OnTastePressed -= OnTastePressed;
		_player.Events.Input.OnTouchPressed -= OnTouchPressed;
		_player.Events.Input.OnHearingPressed -= OnHearingPressed;
	}

	private void OnSightPressed() => ToggleState(PlayerState.CanSee);
	private void OnSmellPressed() => ToggleState(PlayerState.CanSmell);
	private void OnTastePressed() => ToggleState(PlayerState.CanTaste);
	private void OnTouchPressed() => ToggleState(PlayerState.CanTouch);
	private void OnHearingPressed() => ToggleState(PlayerState.CanHear);

	private void ToggleState(PlayerState state) {
		int activeStatesCount = CountActiveFlags((int)_player.State);

		const int minActiveStates = 3;
		const int maxActiveStates = 4;

		if (_player.State.HasFlag(state)) {
			if (activeStatesCount <= minActiveStates) {
				Debug.LogWarning("Cannot deactivate more than 2 states at the same time.");
				return;
			}

			_player.State &= ~state;
		}
		else {
			if (activeStatesCount >= maxActiveStates) {
				Debug.LogWarning("At least 1 state must remain inactive.");
				return;
			}

			_player.State |= state;
		}

		_player.Events.OnPlayerStateChanged?.Invoke(_player.State);
		UpdateSenseTogglables();

		// Debug stuff.
		int newActiveStatesCount = CountActiveFlags((int)_player.State);
		Debug.Log($"New player state: ({newActiveStatesCount}) {_player.State}");
	}

	private void UpdateSenseTogglables() {
		foreach (var togglable in _world.GetSenseTogglables()) {
			bool hasState = _player.State.HasFlag(togglable.State);
			togglable.gameObject.SetActive(hasState);
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
