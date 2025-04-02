using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour {
	[SerializeField] private Slider _masterVolumeSlider;
	[SerializeField] private Slider _musicVolumeSlider;
	[SerializeField] private Slider _sfxVolumeSlider;
	[SerializeField] private Slider _ambienceVolumeSlider;

	private IAudioService _audioService;

	// Player prefs keys.
	private const string MasterVolumeKey = "MasterVolume";
	private const string MusicVolumeKey = "MusicVolume";
	private const string SfxVolumeKey = "SfxVolume";
	private const string AmbienceVolumeKey = "AmbienceVolume";

	private const string TestSound = "sfx_ph_nose";

	private bool _playTestSound;

	private void Start() {
		_playTestSound = false;
		_audioService = Services.Instance.GetService<IAudioService>();
		LoadSliderData();
		LoadVolumeValues();
	}

	private void LoadSliderData() {
		_masterVolumeSlider.minValue = _audioService.MinVolume;
		_masterVolumeSlider.maxValue = _audioService.MaxVolume;
		_musicVolumeSlider.minValue = _audioService.MinVolume;
		_musicVolumeSlider.maxValue = _audioService.MaxVolume;
		_sfxVolumeSlider.minValue = _audioService.MinVolume;
		_sfxVolumeSlider.maxValue = _audioService.MaxVolume;
		_ambienceVolumeSlider.minValue = _audioService.MinVolume;
		_ambienceVolumeSlider.maxValue = _audioService.MaxVolume;

		_masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
		_musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
		_sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
		_ambienceVolumeSlider.onValueChanged.AddListener(SetAmbienceVolume);
	}

	private void LoadVolumeValues() {
		_masterVolumeSlider.value = PlayerPrefs.GetFloat(MasterVolumeKey, _audioService.MaxVolume);
		_musicVolumeSlider.value = PlayerPrefs.GetFloat(MusicVolumeKey, _audioService.MaxVolume);
		_sfxVolumeSlider.value = PlayerPrefs.GetFloat(SfxVolumeKey, _audioService.MaxVolume);
		_ambienceVolumeSlider.value = PlayerPrefs.GetFloat(AmbienceVolumeKey, _audioService.MaxVolume);

		SetMasterVolume(_masterVolumeSlider.value);
		SetMusicVolume(_musicVolumeSlider.value);
		SetSfxVolume(_sfxVolumeSlider.value);
		SetAmbienceVolume(_ambienceVolumeSlider.value);

		_playTestSound = true;
	}

	private void SetMasterVolume(float volume) {
		_audioService.SetMixerVolume(Mixer.Master, volume);

		PlayerPrefs.SetFloat(MasterVolumeKey, volume);
		PlayerPrefs.Save();
	}

	private void SetMusicVolume(float volume) {
		_audioService.SetMixerVolume(Mixer.Music, volume);
		PlayTestSound(Mixer.Music);

		PlayerPrefs.SetFloat(MusicVolumeKey, volume);
		PlayerPrefs.Save();
	}

	private void SetSfxVolume(float volume) {
		_audioService.SetMixerVolume(Mixer.Sfx, volume);
		PlayTestSound(Mixer.Sfx);

		PlayerPrefs.SetFloat(SfxVolumeKey, volume);
		PlayerPrefs.Save();
	}

	private void SetAmbienceVolume(float volume) {
		_audioService.SetMixerVolume(Mixer.Ambience, volume);
		PlayTestSound(Mixer.Ambience);

		PlayerPrefs.SetFloat(AmbienceVolumeKey, volume);
		PlayerPrefs.Save();
	}

	private void PlayTestSound(Mixer mixer) {
		if (!_audioService.IsSoundPlaying(TestSound) && _playTestSound) {
			_audioService.PlaySound(TestSound, mixer);
		}
	}

	private void OnDestroy() {
		_masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
		_musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
		_sfxVolumeSlider.onValueChanged.RemoveListener(SetSfxVolume);
		_ambienceVolumeSlider.onValueChanged.RemoveListener(SetAmbienceVolume);
	}
}
