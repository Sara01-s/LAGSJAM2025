using UnityEngine;
using System.Linq;

public class PlayerInput : MonoBehaviour {
	[SerializeField] private PlayerData _player;

	private readonly KeyCode[] _flyKeys = {
		KeyCode.Space,
		KeyCode.UpArrow,
		KeyCode.W,
		KeyCode.Mouse0
	};

	private readonly KeyCode[] _interactKeys = {
		KeyCode.E,
	};

	private void Start() {
		_player.Input.Enabled = true;
	}

	private void Update() {
		if (!_player.Input.Enabled) {
			return;
		}

		_player.Input.IsMainInputHeld = _flyKeys.Any(key => Input.GetKey(key));
		_player.Input.InteractWasPressedThisFrame = _interactKeys.Any(key => Input.GetKeyDown(key));

		if (_player.Input.InteractWasPressedThisFrame) {
			_player.Input.OnInteractPressed?.Invoke();
		}
	}
}
