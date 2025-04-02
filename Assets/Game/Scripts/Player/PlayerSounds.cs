using static Unity.Mathematics.math;
using UnityEngine;
using System.Collections;

public class PlayerSounds : MonoBehaviour {
	[Header("References")]
	[SerializeField] private PlayerData _player;

	[Header("Footstep sounds")]
	[SerializeField] private float _footstepInterval = 0.5f;

	private IAudioService _audioService;

	private void Start() {
		_audioService = Services.Instance.GetService<IAudioService>();
		_audioService.PlaySound("amb_forest", Mixer.Ambience, volume: 0.5f, loop: true);
	}

	private void OnEnable() {
		_player.Events.OnHealthChanged += PlayHealthSounds;
		_player.Events.OnPlayerStateChanged += PlaySenseSounds;
		_player.Events.OnPlayerLand += PlayPlayerLandSound;
		_player.Events.OnPlayerDeath += PlayPlayerDeathSound;
		_player.Events.Input.OnHorizontalHeld += PlayPlayerWalkSound;
		_player.Events.OnPlayerInteract += PlayInteractSound;
	}

	private void OnDisable() {
		_player.Events.OnHealthChanged -= PlayHealthSounds;
		_player.Events.OnPlayerStateChanged -= PlaySenseSounds;
		_player.Events.OnPlayerLand -= PlayPlayerLandSound;
		_player.Events.Input.OnHorizontalHeld -= PlayPlayerWalkSound;
		_player.Events.OnPlayerDeath -= PlayPlayerDeathSound;
		_player.Events.OnPlayerInteract -= PlayInteractSound;
	}

	private void PlayHealthSounds(float currentHealth, float previousHealth) {
		bool playerIsAlive = currentHealth > 0.0f;
		if (!playerIsAlive) {
			return; // Player is dead, no need to play health sounds.
		}

		if (currentHealth < previousHealth) {
			_audioService.PlaySound("sfx_ph_player_hurt", pitch: Random.Range(1.0f, 1.1f), volume: 0.7f);
		}
		else if (currentHealth > previousHealth) {
			_audioService.PlaySound("sfx_ph_rise_up_01", pitch: Random.Range(0.9f, 1.1f), volume: 0.7f);
		}
	}

	private void PlaySenseSounds(PlayerState state) {
		// TODO - detect wether the new state is a toggle on or toggle off.
	}

	private void PlayInteractSound(Interactable _) {
		_audioService.PlaySound("sfx_ph_interaction_grab", pitch: Random.Range(0.85f, 1.2f), volume: 0.7f);
	}

	private void PlayPlayerDeathSound() {
		StopAllCoroutines(); // Stop ongoing footstep sounds.
		_audioService.PlaySound("sfx_ph_player_death", pitch: 0.9f, volume: 0.8f);
	}

	private void PlayPlayerWalkSound(float _) {
		if (!gameObject.activeSelf) {
			return;
		}

		StartCoroutine(_PlayFootstepSound());

		IEnumerator _PlayFootstepSound() {
			while (abs(_player.HorizontalVelocity) > 0.1f && _player.IsGrounded) {
				const string clip = "sfx_ph_player_footstep_02";
				if (_audioService.IsSoundPlaying(clip)) {
					yield break;
				}

				_audioService.PlaySound(clip, pitch: Random.Range(0.85f, 1.0f), volume: Random.Range(0.8f, 1.0f));

				yield return new WaitForSeconds(_footstepInterval);
			}
		}
	}

	private void PlayPlayerLandSound() {
		_audioService.PlaySound("sfx_ph_player_land", pitch: Random.Range(0.85f, 1.0f), volume: Random.Range(0.3f, 0.5f));
	}
}
