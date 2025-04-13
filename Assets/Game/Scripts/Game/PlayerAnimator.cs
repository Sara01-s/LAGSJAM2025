using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour {
	[SerializeField] private PlayerData _player;
	[SerializeField] private TrailRenderer _trail;

	private float _startTrailTime;
	private Animator _animator;

	private void Awake() {
		_animator = GetComponent<Animator>();
		_startTrailTime = _trail.time;
	}

	private void OnEnable() {
		_player.Events.OnDeath += DisableTrail;
		_player.Events.OnRespawn += EnableTrail;
	}

	private void OnDisable() {
		_player.Events.OnDeath -= DisableTrail;
		_player.Events.OnRespawn -= EnableTrail;
	}

	private void Update() {
		if (_player.Input.IsMainInputHeld) {
			SetAnimatorState(_animator, "Fly");
		}
		else {
			SetAnimatorState(_animator, "Idle");
		}
	}

	private void EnableTrail() {
		_trail.time = _startTrailTime;
		_trail.enabled = true;
	}

	private void DisableTrail(float _, float __) {
		_trail.time = 0.0f;
		_trail.enabled = false;
	}

	private static void SetAnimatorState(Animator animator, string newStateName) {
		int animationHash = Animator.StringToHash(newStateName);
		animator.CrossFade(animationHash, normalizedTransitionDuration: 0.0f);
	}

	private static IEnumerator SetAnimatorStateComplete(Animator animator, string newStateName, string nextStateName) {
		int animationHash = Animator.StringToHash(newStateName);
		float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;

		animator.CrossFade(animationHash, normalizedTransitionDuration: 0.0f);
		yield return new WaitForSeconds(animationLength);
		animator.CrossFade(Animator.StringToHash(nextStateName), normalizedTransitionDuration: 0.0f);
	}
}
