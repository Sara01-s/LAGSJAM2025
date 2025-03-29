using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

#nullable enable

#region Enums

public enum Mixer {
	Master = 0,
	Music = 1,
	SFX = 2,
	Ambience = 3,
}

public enum VolumeMode {
	Linear = 0,
	Decibels = 1,
}

#endregion

#region IAudioService

public interface IAudioService {
	float MinVolume { get; }
	float MaxVolume { get; }

	AudioSource PlaySound(
		string soundFileName,
		Mixer mixer = Mixer.SFX,
		float volume = 1.0f,
		float pitch = 1.0f,
		bool loop = false,
		float spatialBlend = 0.0f,
		byte priority = 128);


	AudioSource PlaySound(
		AudioClip clip,
		Mixer mixer = Mixer.SFX,
		float volume = 1.0f,
		float pitch = 1.0f,
		bool loop = false,
		float spatialBlend = 0.0f,
		byte priority = 128);


	void SetMixerVolume(Mixer mixer, float newVolume);
	float GetMixerVolume(Mixer mixer, VolumeMode volumeMode);

	bool IsSoundPlaying(string clipName);
	void FadeIn(string clipName, float duration = 1.0f);
	void FadeOut(string clipName, float duration = 1.0f);

	void SetGlobalMute(bool mute);
	void SetMute(string clipName, bool mute);
	void StopSound(string clipName);
	void PauseSound(string clipName);
	void ResumeSound(string clipName);
	void StopAllSounds();
}

#endregion

#region CoroutineRunner

public class CoroutineRunner : MonoBehaviour {
	private static CoroutineRunner? _instance;
	public static CoroutineRunner Instance {
		get {
			if (_instance == null) {
				var runnerGO = new GameObject("CoroutineRunner");
				DontDestroyOnLoad(runnerGO);
				_instance = runnerGO.AddComponent<CoroutineRunner>();
			}
			return _instance;
		}
	}

	private void OnDestroy() {
		_instance = null;
	}
}

#endregion

#region AudioStorage

public class AudioStorage {
	public const string MasterVolume = "MasterVolume";
	public const string MusicVolume = "MusicVolume";
	public const string SfxVolume = "SfxVolume";
	public const string AmbienceVolume = "AmbienceVolume";
	public const int BanksPerChannel = 10;

	private const string MusicPath = "Art/Audio/Music";
	private const string MusicPrefix = "music_";
	private const string SfxPath = "Art/Audio/SFX";
	private const string SfxPrefix = "sfx_";
	private const string AmbiencePath = "Art/Audio/Ambience";
	private const string AmbiencePrefix = "amb_";

	public AudioMixer MasterMixer { get; private set; }

	private readonly Dictionary<string, AudioClip> _sfxClips = new();
	private readonly Dictionary<string, AudioClip> _musicClips = new();
	private readonly Dictionary<string, AudioClip> _ambienceClips = new();

	public Dictionary<Mixer, List<AudioSource>> Banks { get; private set; } = new();

	public AudioStorage() {
		MasterMixer = Resources.Load<AudioMixer>("Art/Audio/Mixer/MasterMixer");
		Assert.IsNotNull(MasterMixer, "Master mixer not found. Please check the path.");

		AudioClip[] musicClips = Resources.LoadAll<AudioClip>(MusicPath);
		AudioClip[] sfxClips = Resources.LoadAll<AudioClip>(SfxPath);
		AudioClip[] ambienceClips = Resources.LoadAll<AudioClip>(AmbiencePath);

		var allClips = musicClips.Select(clip => (MusicPath, clip.name))
								 .Concat(sfxClips.Select(clip => (SfxPath, clip.name)))
								 .Concat(ambienceClips.Select(clip => (AmbiencePath, clip.name)));

		CheckAudioClipNames(allClips);

		foreach (var clip in musicClips) _musicClips.Add(clip.name, clip);
		foreach (var clip in sfxClips) _sfxClips.Add(clip.name, clip);
		foreach (var clip in ambienceClips) _ambienceClips.Add(clip.name, clip);

		Debug.Log($"Loaded {musicClips.Length} music clips, {sfxClips.Length} SFX clips, and {ambienceClips.Length} ambience clips.");
		if (musicClips.Length > 0) Debug.Log("Loaded music clips: " + string.Join(", ", musicClips.Select(clip => clip.name)));
		if (sfxClips.Length > 0) Debug.Log("Loaded SFX clips: " + string.Join(", ", sfxClips.Select(clip => clip.name)));
		if (ambienceClips.Length > 0) Debug.Log("Loaded ambience clips: " + string.Join(", ", ambienceClips.Select(clip => clip.name)));

		CreateAudioSourceBanks();
	}

	public void CreateAudioSourceBanks() {
		var banksGO = new GameObject("AudioSourceBanks");
		UnityEngine.Object.DontDestroyOnLoad(banksGO);

		if (banksGO.GetComponent<CoroutineRunner>() == null) {
			banksGO.AddComponent<CoroutineRunner>();
		}

		foreach (Mixer mixer in new Mixer[] { Mixer.Music, Mixer.SFX, Mixer.Ambience }) {
			var bankChild = new GameObject(mixer.ToString() + "Bank");
			bankChild.transform.parent = banksGO.transform;

			var sources = new List<AudioSource>();
			for (int i = 0; i < BanksPerChannel; i++) {
				var sourceGO = new GameObject($"{mixer}_Source_{i}");

				sourceGO.transform.parent = bankChild.transform;
				AudioSource audioSource = sourceGO.AddComponent<AudioSource>();

				sources.Add(audioSource);
			}

			Banks.Add(mixer, sources);
		}
	}

	public void DestroyAudioSourceBanks() {
		foreach (var bank in Banks) {
			foreach (var source in bank.Value) {
				UnityEngine.Object.Destroy(source.gameObject);
			}
		}

		Banks.Clear();
	}

	public void CheckAudioClipNames(IEnumerable<(string path, string name)> audioClips) {
		foreach (var (path, name) in audioClips) {
			switch (path) {
				case MusicPath:
				if (!name.StartsWith(MusicPrefix)) {
					Debug.LogWarning($"Audio clip {name} in {path} does not start with the {MusicPrefix} prefix. Please rename it.");
				}
				break;
				case SfxPath:
				if (!name.StartsWith(SfxPrefix)) {
					Debug.LogWarning($"Audio clip {name} in {path} does not start with the {SfxPrefix} prefix. Please rename it.");
				}
				break;
				case AmbiencePath:
				if (!name.StartsWith(AmbiencePrefix)) {
					Debug.LogWarning($"Audio clip {name} in {path} does not start with the {AmbiencePrefix} prefix. Please rename it.");
				}
				break;
				default:
				break;
			}
		}
	}

	public bool ClipExists(string clipName) {
		return _sfxClips.ContainsKey(clipName)
			|| _musicClips.ContainsKey(clipName)
			|| _ambienceClips.ContainsKey(clipName);
	}

	public AudioClip? GetClip(string clipName) {
		if (_musicClips.TryGetValue(clipName, out var clip)) return clip;
		if (_sfxClips.TryGetValue(clipName, out clip)) return clip;
		if (_ambienceClips.TryGetValue(clipName, out clip)) return clip;
		return null;
	}

	public AudioMixerGroup GetMixer(Mixer mixer) {
		return mixer switch {
			Mixer.Master => MasterMixer.FindMatchingGroups("Master")[0],
			Mixer.Music => MasterMixer.FindMatchingGroups("Music")[0],
			Mixer.SFX => MasterMixer.FindMatchingGroups("Sfx")[0],
			Mixer.Ambience => MasterMixer.FindMatchingGroups("Ambience")[0],
			_ => throw new NotImplementedException(),
		};
	}

}
#endregion

#region AudioPlayer
public class AudioPlayer : IAudioService, IDisposable {
	public float MinVolume => 0.0f;
	public float MaxVolume => 1.0f;

	private readonly AudioStorage _audioStorage = new();
	private readonly Dictionary<string, AudioSource> _activeSources = new();

	private bool _globalMute = false;

	public void SetMixerVolume(Mixer mixer, float newVolume) {
		if (newVolume < MinVolume || newVolume > MaxVolume) {
			throw new ArgumentOutOfRangeException(nameof(newVolume), $"Volume must be between {MinVolume} and {MaxVolume}.");
		}

		float dbVolume = Mathf.Log10(newVolume) * 20; // Convert Linear to Db.

		switch (mixer) {
			case Mixer.Master:
			_audioStorage.MasterMixer.SetFloat(AudioStorage.MasterVolume, dbVolume);
			return;
			case Mixer.Music:
			_audioStorage.MasterMixer.SetFloat(AudioStorage.MusicVolume, dbVolume);
			return;
			case Mixer.SFX:
			_audioStorage.MasterMixer.SetFloat(AudioStorage.SfxVolume, dbVolume);
			return;
			case Mixer.Ambience:
			_audioStorage.MasterMixer.SetFloat(AudioStorage.AmbienceVolume, dbVolume);
			return;
			default:
			throw new ArgumentOutOfRangeException(nameof(mixer), $"Invalid mixer type: {mixer}.");
		}
	}

	public float GetMixerVolume(Mixer mixer, VolumeMode volumeMode = VolumeMode.Linear) {
		float result = 0;
		switch (mixer) {
			case Mixer.Master:
			_audioStorage.MasterMixer.GetFloat(AudioStorage.MasterVolume, out result);
			break;
			case Mixer.Music:
			_audioStorage.MasterMixer.GetFloat(AudioStorage.MusicVolume, out result);
			break;
			case Mixer.SFX:
			_audioStorage.MasterMixer.GetFloat(AudioStorage.SfxVolume, out result);
			break;
			case Mixer.Ambience:
			_audioStorage.MasterMixer.GetFloat(AudioStorage.AmbienceVolume, out result);
			break;
		}
		return volumeMode switch {
			VolumeMode.Linear => Mathf.Pow(10, result / 20), // Convert Db to Linear.
			_ => result,
		};
	}

	public AudioSource PlaySound(
		AudioClip clip,
		Mixer mixer = Mixer.SFX,
		float volume = 1,
		float pitch = 1,
		bool loop = false,
		float spatialBlend = 0,
		byte priority = 128
	) {
		if (mixer == Mixer.Master) {
			throw new ArgumentException("Cannot play sound on Master mixer. Use Sfx or other mixers.", nameof(mixer));
		}

		if (!_audioStorage.Banks.TryGetValue(mixer, out List<AudioSource> bankSources)) {
			_audioStorage.CreateAudioSourceBanks();
			Debug.Log("Audio source banks created.");
		}

		AudioSource source = bankSources.FirstOrDefault(s => !s.isPlaying);
		if (source == null) {
			Debug.LogWarning($"No available AudioSource in the {mixer} bank. The first one will be used.");
			source = bankSources[0];
		}

		source.clip = clip;
		source.volume = volume;
		source.pitch = pitch;
		source.loop = loop;
		source.spatialBlend = spatialBlend;
		source.priority = priority;

		source.outputAudioMixerGroup = _audioStorage.GetMixer(mixer);
		source.mute = _globalMute;

		source.Play();

		_activeSources[clip.name] = source;

		return source;
	}

	public AudioSource PlaySound(
		string soundFileName,
		Mixer mixer = Mixer.SFX,
		float volume = 1,
		float pitch = 1,
		bool loop = false,
		float spatialBlend = 0,
		byte priority = 128
	) {
		AudioClip? clip = _audioStorage.GetClip(soundFileName);
		if (clip == null) {
			throw new ArgumentException($"Clip with name '{soundFileName}' was not found.", nameof(soundFileName));
		}

		return PlaySound(clip, mixer, volume, pitch, loop, spatialBlend, priority);
	}

	public bool IsSoundPlaying(string clipName) {
		if (_activeSources.TryGetValue(clipName, out AudioSource source)) {
			if (source.isPlaying) {
				return true;
			}
			else {
				_activeSources.Remove(clipName);
			}
		}
		return false;
	}

	public void FadeIn(string clipName, float duration = 1f) {
		if (!_activeSources.TryGetValue(clipName, out AudioSource source)) {
			Debug.LogWarning($"AudioSource not found for clip: {clipName}");
			return;
		}

		CoroutineRunner.Instance.StartCoroutine(FadeCoroutine(source, 0f, 1f, duration));
	}

	public void FadeOut(string clipName, float duration = 1f) {
		if (!_activeSources.TryGetValue(clipName, out AudioSource source)) {
			Debug.LogWarning($"AudioSource not found for clip: {clipName}");
			return;
		}

		CoroutineRunner.Instance.StartCoroutine(FadeCoroutine(source, source.volume, 0f, duration, () => {
			source.Stop();
			_activeSources.Remove(clipName);
		}));
	}

	public void SetGlobalMute(bool mute) {
		_globalMute = mute;
		foreach (var source in _activeSources.Values) {
			source.mute = mute;
		}
	}

	public void SetMute(string clipName, bool mute) {
		if (_activeSources.TryGetValue(clipName, out AudioSource source)) {
			source.mute = mute;
		}
		else {
			Debug.LogWarning($"AudioSource not found for clip: {clipName}");
		}
	}

	public void StopSound(string clipName) {
		if (_activeSources.TryGetValue(clipName, out AudioSource source)) {
			source.Stop();
			_activeSources.Remove(clipName);
		}
		else {
			Debug.LogWarning($"AudioSource not found for clip: {clipName}");
		}
	}

	public void PauseSound(string clipName) {
		if (_activeSources.TryGetValue(clipName, out AudioSource source)) {
			source.Pause();
		}
		else {
			Debug.LogWarning($"AudioSource not found for clip: {clipName}");
		}
	}

	public void ResumeSound(string clipName) {
		if (_activeSources.TryGetValue(clipName, out AudioSource source)) {
			source.UnPause();
		}
		else {
			Debug.LogWarning($"AudioSource not found for clip: {clipName}");
		}
	}

	public void StopAllSounds() {
		foreach (var source in _activeSources.Values) {
			source.Stop();
		}
		_activeSources.Clear();
	}

	private IEnumerator FadeCoroutine(
		AudioSource source,
		float startVolume,
		float targetVolume,
		float duration,
		Action? onComplete = null
	) {
		float timer = 0f;
		source.volume = startVolume;
		while (timer < duration) {
			timer += Time.deltaTime;
			source.volume = Mathf.Lerp(startVolume, targetVolume, timer / duration);
			yield return null;
		}
		source.volume = targetVolume;
		onComplete?.Invoke();
	}

	public void Dispose() {
		_audioStorage.DestroyAudioSourceBanks();
	}
}
#endregion
