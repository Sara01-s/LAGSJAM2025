using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimations : MonoBehaviour {
	[SerializeField] private PlayerData _player;

	[SerializeField] private AnimationClip _playerIdle;
	[SerializeField] private AnimationClip _playerWalk;
	[SerializeField] private AnimationClip _playerJump;
	[SerializeField] private AnimationClip _playerHurt;

	private int _idle, _walk, _jump, _hurt;

	private Animator _animator;

	private void Awake() {
		_idle = Animator.StringToHash(_playerIdle.name);
		_walk = Animator.StringToHash(_playerWalk.name);
		_jump = Animator.StringToHash(_playerJump.name);
		_hurt = Animator.StringToHash(_playerHurt.name);

		_animator = GetComponent<Animator>();
	}

	private void OnEnable() {
		_player.Events.OnPlayerJump += PlayJumpAnimation;
	}

	private void OnDisable() {
		_player.Events.OnPlayerJump -= PlayJumpAnimation;
	}

	private void PlayJumpAnimation() {
		_animator.CrossFade(_jump, 0.0f);
	}
}
