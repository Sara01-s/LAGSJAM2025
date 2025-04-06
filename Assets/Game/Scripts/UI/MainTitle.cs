using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyTransition;
using UnityEngine.SceneManagement;

public class MainTitle : MonoBehaviour {
	[Header("References")]
	[SerializeField] private PlayerData _player;

	[Header("UI")]
	[SerializeField] private BetterButton _playButton;
	[SerializeField] private BetterButton _settingsButton;
	[SerializeField] private BetterButton _quitButton;

	[Header("Settings Panel")]
	[SerializeField] private SettingsPanel _settingsPanel;

	[Header("Transitions")]
	[SerializeField] private TransitionSettings _transition;

	private IAudioService _audioService;

	private void Start() {
		_audioService = Services.Instance.GetService<IAudioService>();
	}

	private void OnEnable() {
		_playButton.OnClick.RemoveAllListeners();
		_playButton.OnClick.AddListener(OnPlayButtonClicked);
	}

	private void OnDisable() {
		_playButton.OnClick.RemoveAllListeners();
	}

	private void OnPlayButtonClicked() {
		_audioService.PlaySound("sfx_ph_play");
		TransitionManager.Instance.Transition("Game", _transition, startDelay: 0);
	}

	public void QuitApplication() {
		print("TODO - Quit!");
	}
}
