using System.Collections;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class PlayerAnimations : MonoBehaviour {
	[SerializeField] private PlayerData _player;

	[SerializeField] private AnimationClip _playerIdle;
	[SerializeField] private AnimationClip _playerWalk;
	[SerializeField] private AnimationClip _playerJump;
	[SerializeField] private AnimationClip _playerHurt;
	[SerializeField] private AnimationClip _playerDeath;

	private SpriteRenderer _spriteRenderer;
	private int _currentState = 0;
	private Animator _animator;

	private void Awake() {
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
		_player.Events.OnPlayerDeath += PlayDeathAnimation;
	}

	private void OnDisable() {
		UnsubscribeAll();
	}

	private void UnsubscribeAll() {
		_player.Events.OnPlayerJump -= PlayJumpAnimation;
		_player.Events.Input.OnHorizontalHeld -= PlayWalkAnimation;
		_player.Events.OnPlayerLand -= PlayIdleAnimation;
		_player.Events.OnPlayerHurt -= PlayHurtAnimation;
		_player.Events.OnPlayerDeath -= PlayDeathAnimation;
	}

	public void ChangeAnimationState(string newState) {
		if (_currentState == Animator.StringToHash(newState)) {
			return;
		}

		_animator.CrossFade(Animator.StringToHash(newState), normalizedTransitionDuration: 0.0f);
		_currentState = Animator.StringToHash(newState);
	}

	public IEnumerator ChangeAnimationStateComplete(string newState, string nextState) {
		ChangeAnimationState(newState);
		yield return new WaitForSecondsRealtime(_animator.GetCurrentAnimatorStateInfo(layerIndex: 0).length);
		ChangeAnimationState(nextState);
	}

	private void PlayDeathAnimation() {
		UnsubscribeAll();
		ChangeAnimationState(_playerDeath.name);

		StartCoroutine(PLEASE_DIEEEEEEEEEEEEEEEEE());

		IEnumerator PLEASE_DIEEEEEEEEEEEEEEEEE() {
			yield return new WaitForSecondsRealtime(_animator.GetCurrentAnimatorStateInfo(layerIndex: 0).length);
			Destroy(gameObject);
		}
	}

	private void PlayIdleAnimation() {
		PlayWalkAnimation(_player.HorizontalVelocity);
	}

	private void PlayJumpAnimation() {
		ChangeAnimationState(_playerJump.name);
	}

	private void PlayHurtAnimation(DamageInfo _) {
		StartCoroutine(ChangeAnimationStateComplete(_playerHurt.name, _playerIdle.name));
	}

	private void PlayWalkAnimation(float input) {
		if (!_player.IsGrounded) {
			return;
		}

		if (Mathf.Approximately(input, 0.0f)) {
			ChangeAnimationState(_playerIdle.name);
			return;
		}

		ChangeAnimationState(_playerWalk.name);
		_spriteRenderer.flipX = input < 0.0f;
	}
}
