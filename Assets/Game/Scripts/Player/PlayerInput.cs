using UnityEngine.InputSystem;
using UnityEngine;

public sealed class PlayerInput : MonoBehaviour, InputMap.IGameplayActions {
	[Header("References")]
	[SerializeField] private PlayerData _player;

	private InputMap _inputMap;

	private void Awake() {
		_inputMap = new();
		_inputMap.Gameplay.SetCallbacks(this);

		ChangeActionMap(_inputMap.Gameplay);
	}

	private void OnDisable() {
		_inputMap.Disable();
		_inputMap.Gameplay.RemoveCallbacks(this);
		_inputMap = null;
	}

	private void Update() {
		_player.Events.Input.IsJumpInputHeld = _inputMap.Gameplay.Jump.IsPressed();

		if (_player.Events.Input.IsJumpInputHeld) {
			_player.Events.Input.OnJumpInputHeld?.Invoke();
		}
	}

	public static void ChangeActionMap(InputActionMap newActionMap) {
		if (!newActionMap.enabled) {
			newActionMap.Enable();
		}
	}

	public void OnEscInput(InputAction.CallbackContext context) {
		if (context.started) {
			_player.Events.Input.OnEscInputPressed?.Invoke();
			print("Escape input pressed!");
		}
	}

	public void OnHorizontal(InputAction.CallbackContext context) {
		float value = context.ReadValue<float>();
		_player.Events.Input.OnHorizontalHeld?.Invoke(value);

		if (context.canceled) {
			_player.Events.Input.OnHorizontalReleased?.Invoke();
		}
	}
	public void OnVertical(InputAction.CallbackContext context) {
		float value = context.ReadValue<float>();
		_player.Events.Input.OnVerticalHeld?.Invoke(value);

		if (context.canceled) {
			_player.Events.Input.OnVerticalReleased?.Invoke();
		}
	}

	public void OnJump(InputAction.CallbackContext context) {
		if (context.started) {
			_player.Events.Input.OnJumpPressed?.Invoke();
		}

		if (context.canceled) {
			_player.Events.Input.OnJumpReleased?.Invoke();
		}
	}

	public void OnSense1(InputAction.CallbackContext context) {
		if (context.started) {
			_player.Events.Input.OnSightPressed?.Invoke();
		}
	}

	public void OnSense2(InputAction.CallbackContext context) {
		if (context.started) {
			_player.Events.Input.OnHearingPressed?.Invoke();
		}
	}

	public void OnSense3(InputAction.CallbackContext context) {
		if (context.started) {
			_player.Events.Input.OnSmellPressed?.Invoke();
		}
	}

	public void OnSense4(InputAction.CallbackContext context) {
		if (context.started) {
			_player.Events.Input.OnTouchPressed?.Invoke();
		}
	}

	public void OnSense5(InputAction.CallbackContext context) {
		if (context.started) {
			_player.Events.Input.OnTastePressed?.Invoke();
		}
	}

	public void OnInteract(InputAction.CallbackContext context) {
		if (context.started) {
			_player.Events.Input.OnInteractPressed?.Invoke();
		}
	}
}

