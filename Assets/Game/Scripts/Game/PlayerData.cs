using static Unity.Mathematics.math;
using UnityEngine;
using System;

[Flags]
public enum PlayerState {
	WallGliding = 1 << 0,
	OnInputGoalMinigame = 1 << 1,
}

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerData : ScriptableObject {
	public PlayerState State;
	public void AddState(PlayerState state) => State |= state;
	public void RemoveState(PlayerState state) => State &= ~state;
	public bool HasState(PlayerState state) => (State & state) == state;

	/// <summary>
	/// Normalized Spline Position.
	/// `0.0f` = spline start, `1.0f` = spline end.
	/// </summary>
	public float Nsp;
	// <summary> Normal offset Y from the spline. </summary>
	public float NormalOffset;
	public float StartSpeed;
	public float RollAngles;
	public float GravityScale;
	public Vector2 WorldPosition;
	public bool IsFrozen;

	public float LastCheckpointNsp;
	public float LastCheckpointNormalOffset;

	public InputData Input;
	public EventData Events;

	public float Speed {
		get => _speed;
		set {
			bool isDifferentValue = abs(_speed - value) > EPSILON;

			if (isDifferentValue) {
				float oldSpeed = _speed;

				_speed = max(0.0f, value);
				Events.OnSpeedChanged?.Invoke(_speed, oldSpeed);
				Debug.Log($"Speed changed from {oldSpeed} to {_speed}");
			}
		}
	}

	private float _speed = 0.0f;

	[Serializable]
	public struct InputData {
		public bool Enabled;
		public bool IsMainInputHeld;
		public bool InteractWasPressedThisFrame;

		public Action OnInteractPressed;
	}

	public struct EventData {
		public Action<float /*newSpeed*/, float /*oldSpeed*/> OnSpeedChanged;
		public Action<float /*lastCheckpointNsp*/, float /*lastCheckpointNormalOffset*/> OnDeath;
		public Action OnRespawn;
	}

}
