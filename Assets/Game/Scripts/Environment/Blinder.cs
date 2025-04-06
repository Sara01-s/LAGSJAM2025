using UnityEngine;

public class Blinder : MonoBehaviour {
	[SerializeField] private PlayerData _player;

	private SpriteRenderer _spriteRenderer;

	private void Awake() => _spriteRenderer = GetComponent<SpriteRenderer>();
	private void OnEnable() => _player.Events.OnPlayerStateChanged += HAHAHHAHAHAHAHHAHAHAA;
	private void OnDisable() => _player.Events.OnPlayerStateChanged -= HAHAHHAHAHAHAHHAHAHAA;

	private void HAHAHHAHAHAHAHHAHAHAA(PlayerState state) {
		_spriteRenderer.enabled = !state.HasFlag(PlayerState.CanSee);
	}

}
