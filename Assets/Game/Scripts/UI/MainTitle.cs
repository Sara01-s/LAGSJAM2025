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
		_playButton.OnClick.AddListener(OnPlayButtonClicked);
		TransitionManager.Instance.onTransitionBegin += PlayTransitionInSound;
		TransitionManager.Instance.onTransitionEnd += PlayTransitionOutSound;
	}

	private void OnDisable() {
		_playButton.OnClick.RemoveListener(OnPlayButtonClicked);
		TransitionManager.Instance.onTransitionBegin -= PlayTransitionInSound;
		TransitionManager.Instance.onTransitionEnd -= PlayTransitionOutSound;
	}

	private void PlayTransitionInSound() {
		_audioService.PlaySound("sfx_ph_rise_up_01", volume: 0.8f);
	}

	private void PlayTransitionOutSound() {
		_audioService.PlaySound("sfx_ph_rise_down_01", volume: 0.8f);
	}

	private void OnPlayButtonClicked() {
		TransitionManager.Instance.Transition("Game", _transition, startDelay: 0);
	}

	private void QuitApplication() {
		print("TODO - Quit!");
	}
}
