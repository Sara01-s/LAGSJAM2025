using static Unity.Mathematics.math;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SpeedChanger : Interactable {
	private enum Mode {
		Automatic,
		Manual,
		InputGoal,
	}

	[SerializeField] private Mode _mode = Mode.Automatic;
	[SerializeField] private float _speedChange = 0.0f;

	[Header("If boost duration is 0, the speed change is permanent.")]
	[SerializeField, Min(0.0f)] private float _speedDurationSec = 0.0f;

	[Header("Only works in InputGoal mode.")]
	[SerializeField, Min(0)] private int _inputsNeeded;
	[SerializeField] private float _speedChangeOnEnter;
	[SerializeField, Min(0.0f)] private float _inputGoalDurationSec;

	private void Awake() {
		GetComponent<Collider2D>().isTrigger = true;

		if (_mode != Mode.InputGoal && (_inputsNeeded > 0 || _speedChangeOnEnter > 0.0f)) {
			Debug.LogWarning($"SpeedChanger {_mode} mode cannot have inputs needed or speed change on enter.");
		}
	}

	public override void Interact(PlayerData player) {
		if (_mode == Mode.Manual) {
			StartCoroutine(ChangeSpeed(player, _speedChange, _speedDurationSec));
		}
	}

	public override void OnInteractEnter(PlayerData player) {
		switch (_mode) {
			case Mode.Automatic:
			StartCoroutine(ChangeSpeed(player, _speedChange, _speedDurationSec));
			Debug.Log($"SpeedChanger {_speedChange} entered by {player.name}.");
			break;
			case Mode.InputGoal:
			StartCoroutine(StartInputGoalMinigame(player));
			break;
			default:
			break;
		}
	}

	protected IEnumerator ChangeSpeed(PlayerData player, float speedChange, float speedChangeDurationSec = 0.0f, float speedTransitionDurationSec = 1.0f) {
		if (speedChangeDurationSec <= 0.0f) {
			yield return ChangeSpeedGradually(player, speedChange, speedTransitionDurationSec);
		}
		else {
			yield return ChangeSpeedGradually(player, _speedChange, speedTransitionDurationSec);
			yield return new WaitForSeconds(_speedDurationSec);
			yield return ChangeSpeedGradually(player, -_speedChange, speedTransitionDurationSec);
		}
	}

	private IEnumerator ChangeSpeedGradually(PlayerData player, float speedChange, float speedTransitionDurationSec = 1.0f) {
		float targetSpeed = player.Speed + speedChange;
		float elapsedTime = 0.0f;

		while (elapsedTime < speedTransitionDurationSec) {
			float t = elapsedTime / speedTransitionDurationSec;
			float easeOutCubic = 1.0f - pow(1.0f - t, 3.0f);

			player.Speed = lerp(player.Speed, targetSpeed, easeOutCubic);
			elapsedTime += Time.deltaTime;

			yield return null;
		}

		player.Speed = targetSpeed;
	}

	private IEnumerator StartInputGoalMinigame(PlayerData player) {
		player.Events.OnDeath += (_, _) => {
			Debug.Log($"SpeedChanger input goal minigame cancelled. Player died.");
			player.RemoveState(PlayerState.OnInputGoalMinigame);
			StopAllCoroutines();
		};

		yield return ChangeSpeed(player, _speedChangeOnEnter, speedTransitionDurationSec: 0.1f);

		player.AddState(PlayerState.OnInputGoalMinigame);

		float elapsed = 0.0f;
		int inputCount = 0;

		while (inputCount < _inputsNeeded) {
			if (player.Input.InteractWasPressedThisFrame) {
				inputCount += 1;
				Debug.Log($"SpeedChanger input received. Inputs left: {_inputsNeeded - inputCount}.");
			}

			elapsed += Time.deltaTime;

			if (elapsed >= _inputGoalDurationSec) {
				Debug.Log($"SpeedChanger input goal timed out.");
				player.RemoveState(PlayerState.OnInputGoalMinigame);
				yield break;
			}

			yield return null;
		}

		Debug.Log($"SpeedChanger input goal completed. Speed change: {_speedChange}.");

		player.RemoveState(PlayerState.OnInputGoalMinigame);
		yield return ChangeSpeed(player, _speedChange, _speedDurationSec);
	}

}
