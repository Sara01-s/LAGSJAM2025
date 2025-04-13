using UnityEngine;
using EasyTransition;

public class Core : MonoBehaviour {

	private IAudioService _audioService;

	private void Awake() {
		DontDestroyOnLoad(gameObject);

		name = name[..4];
		Services.Instance.RegisterService<IAudioService, AudioPlayer>();

		_audioService = Services.Instance.GetService<IAudioService>();

		TransitionManager.Instance._onTransitionBegin += PlayTransitionUpSound;
		TransitionManager.Instance._onTransitionCutPointReached += PlayTransitionDownSound;
	}

	private void OnDestroy() {
		TransitionManager.Instance._onTransitionBegin -= PlayTransitionUpSound;
		TransitionManager.Instance._onTransitionCutPointReached -= PlayTransitionDownSound;

		Services.Instance.Dispose();
	}

	private void PlayTransitionUpSound() {
		_audioService.PlaySound("sfx_ph_rise_up_01");
	}

	private void PlayTransitionDownSound() {
		_audioService.PlaySound("sfx_ph_rise_down_01");
	}

}
