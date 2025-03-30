using UnityEngine.InputSystem;
using UnityEngine;

public sealed class PlayerInput : MonoBehaviour, InputMap.IGameplayActions {
	[SerializeField] private PlayerEvents _playerEvents;

	private InputMap _inputMap;

	private void Awake() {
		_inputMap = new();
		_inputMap.Gameplay.SetCallbacks(this);

		ChangeActionMap(_inputMap.Gameplay);
	}

	private void OnDisable() {
		_inputMap.Gameplay.RemoveCallbacks(this);
		_inputMap = null;
	}

	private void Update() {
		_playerEvents.Input.IsJumpInputHeld = _inputMap.Gameplay.Jump.IsPressed();

		if (_playerEvents.Input.IsJumpInputHeld) {
			_playerEvents.Input.OnJumpInputHeld?.Invoke();
		}
	}

	public static void ChangeActionMap(InputActionMap newActionMap) {
		if (!newActionMap.enabled) {
			newActionMap.Enable();
		}
	}

	public void OnEscInput(InputAction.CallbackContext context) {
		if (context.started) {
			_playerEvents.Input.OnEscInputPressed?.Invoke();
			print("Escape input pressed!");
		}
	}

	public void OnHorizontal(InputAction.CallbackContext context) {
		if (context.performed) {
			float value = context.ReadValue<float>();
			_playerEvents.Input.OnHorizontalHeld?.Invoke(value);
		}

		if (context.canceled) {
			_playerEvents.Input.OnHorizontalReleased?.Invoke();
		}
	}

	public void OnJump(InputAction.CallbackContext context) {
		if (context.started) {
			_playerEvents.Input.OnJumpPressed?.Invoke();
		}

		if (context.canceled) {
			_playerEvents.Input.OnJumpReleased?.Invoke();
		}
	}

	public void OnSense1(InputAction.CallbackContext context) {
		if (context.started) {
			_playerEvents.Input.OnSightPressed?.Invoke();
		}
	}

	public void OnSense2(InputAction.CallbackContext context) {
		if (context.started) {
			_playerEvents.Input.OnHearingPressed?.Invoke();
		}
	}

	public void OnSense3(InputAction.CallbackContext context) {
		if (context.started) {
			_playerEvents.Input.OnSmellPressed?.Invoke();
		}
	}

	public void OnSense4(InputAction.CallbackContext context) {
		if (context.started) {
			_playerEvents.Input.OnTouchPressed?.Invoke();
		}
	}

	public void OnSense5(InputAction.CallbackContext context) {
		if (context.started) {
			_playerEvents.Input.OnTastePressed?.Invoke();
		}
	}
}

