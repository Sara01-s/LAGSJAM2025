using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PlayerSenses : MonoBehaviour {
	[SerializeField] private PlayerData _playerData;
	[SerializeField] private PlayerEvents _playerEvents;

	private void Awake() {
		_playerData.State = _playerData.InitialState;
		_playerEvents.OnPlayerStateChange?.Invoke(_playerData.State);
		Debug.Log($"Player state initialized: {_playerData.State}");
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

		// By default, the player should have 4 active states.

		if (_playerData.State.HasFlag(state)) {
			if (activeStatesCount <= 3) {
				Debug.LogWarning("Cannot deactivate more than 2 states at the same time.");
				return;
			}

			_playerData.State &= ~state;
		}
		else {
			if (activeStatesCount >= 4) {
				Debug.LogWarning("At least 1 state must remain inactive.");
				return;
			}

			_playerData.State |= state;
		}

		_playerEvents.OnPlayerStateChange?.Invoke(_playerData.State);
		int newActiveStatesCount = CountActiveFlags((int)_playerData.State);
		Debug.Log($"New player state: ({newActiveStatesCount}) {_playerData.State}");
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
