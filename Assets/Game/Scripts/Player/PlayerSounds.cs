using UnityEngine;
using EasyTransition;

public class PlayerSounds : MonoBehaviour {
	[SerializeField] private PlayerData _player;
	[SerializeField] private PlayerEvents _playerEvents;

	private IAudioService _audioService;

	private void Start() {
		_audioService = Services.Instance.GetService<IAudioService>();
		_audioService.PlaySound("amb_forest", Mixer.Ambience, volume: 0.5f, loop: true);
	}

	private void OnEnable() {
		_playerEvents.OnPlayerHurt += PlayPlayerHurtSound;
		_playerEvents.OnPlayerLand += PlayPlayerLandSound;
		_playerEvents.OnPlayerDeath += PlayPlayerDeathSound;
		_playerEvents.Input.OnHorizontalHeld += PlayPlayerWalkSound;
	}

	private void OnDisable() {
		_playerEvents.OnPlayerHurt -= PlayPlayerHurtSound;
		_playerEvents.OnPlayerLand -= PlayPlayerLandSound;
		_playerEvents.Input.OnHorizontalHeld -= PlayPlayerWalkSound;
		_playerEvents.OnPlayerDeath -= PlayPlayerDeathSound;
	}

	private void PlayPlayerHurtSound(DamageInfo damageInfo) {
		if (_player.Health > 0) { // Player is alive.
			_audioService.PlaySound("sfx_player_hurt", pitch: Random.Range(0.85f, 1.2f), volume: 0.7f);
		}
	}

	private void PlayPlayerDeathSound() {
		_audioService.PlaySound("sfx_player_hurt", pitch: 0.4f, volume: 0.8f);
	}

	private void PlayPlayerWalkSound(float _) {
		if (_player.HorizontalVelocity < 0.1f) {
			return;
		}

		string clip = Random.value < 0.5f ? "sfx_player_footstep_01" : "sfx_player_footstep_02";
		_audioService.PlaySound(clip, pitch: Random.Range(0.85f, 1.2f), volume: 0.7f);
	}

	private void PlayPlayerLandSound() {
		_audioService.PlaySound("sfx_player_land_hi", pitch: Random.Range(0.85f, 1.0f), volume: Random.Range(0.3f, 0.5f));
	}
}
