using static Unity.Mathematics.math;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class PlayerAnimations : MonoBehaviour {
	[SerializeField] private PlayerData _player;

	[SerializeField] private AnimationClip _playerIdle;
	[SerializeField] private AnimationClip _playerWalk;
	[SerializeField] private AnimationClip _playerJump;
	[SerializeField] private AnimationClip _playerHurt;

	private int _idle, _walk, _jump, _hurt;

	private SpriteRenderer _spriteRenderer;
	private Animator _animator;

	private void Awake() {
		_idle = Animator.StringToHash(_playerIdle.name);
		_walk = Animator.StringToHash(_playerWalk.name);
		_jump = Animator.StringToHash(_playerJump.name);
		_hurt = Animator.StringToHash(_playerHurt.name);

		_spriteRenderer = GetComponent<SpriteRenderer>();
		_animator = GetComponent<Animator>();
	}

	private void Start() {
		PlayIdleAnimation();
	}

	private void OnEnable() {
		_player.Events.OnPlayerJump += PlayJumpAnimation;
		_player.Events.Input.OnHorizontalHeld += PlayWalkAnimation;
		_player.Events.OnPlayerLand += PlayIdleAnimation;
		_player.Events.OnPlayerHurt += PlayHurtAnimation;
	}

	private void OnDisable() {
		_player.Events.OnPlayerJump -= PlayJumpAnimation;
		_player.Events.Input.OnHorizontalHeld -= PlayWalkAnimation;
		_player.Events.OnPlayerLand -= PlayIdleAnimation;
		_player.Events.OnPlayerHurt -= PlayHurtAnimation;
	}

	private void PlayIdleAnimation() {
		PlayWalkAnimation(_player.HorizontalVelocity);
	}

	private void PlayJumpAnimation() {
		_animator.CrossFade(_jump, 0.0f);
	}

	private void PlayHurtAnimation(DamageInfo _) {
		_animator.CrossFade(_hurt, 0.0f);
	}

	private void PlayWalkAnimation(float input) {
		if (!_player.IsGrounded) {
			return;
		}

		if (Mathf.Approximately(input, 0.0f)) {
			_animator.CrossFade(_idle, 0.0f);
			return;
		}

		_animator.CrossFade(_walk, 0.0f);
		_spriteRenderer.flipX = input < 0.0f;
	}
}
