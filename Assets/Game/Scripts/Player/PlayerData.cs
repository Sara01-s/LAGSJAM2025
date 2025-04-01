using System;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

[Flags]
public enum PlayerState {
	CanSee = 1 << 0,
	CanHear = 1 << 1,
	CanSmell = 1 << 2,
	CanTouch = 1 << 3,
	CanTaste = 1 << 4,
}

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerData : ScriptableObject {
	[Header("Movement")]
	[Min(0.1f)] public float Speed = 5.0f;
	[Min(0.1f)] public float JumpHeight = 4.0f;
	[Min(0.1f)] public float SecondsToJumpPeak = 0.5f;
	[Min(0.1f)] public float SecondsToLand = 0.5f;
	[Min(0.1f)] public float MaxYSpeed = 24.2f;
	[HideInInspector] public Vector2 Position;
	[HideInInspector] public float HorizontalVelocity;

	[Header("Collision")]
	public Vector2 GroundCheckBoxOffset = new(0.0f, -0.5f);
	public Vector2 GroundCheckBox = new(0.5f, 0.5f);
	public LayerMask GroundLayer;
	public bool IsGrounded;

	[Header("Player State")]
	public PlayerState InitialState = PlayerState.CanSee | PlayerState.CanSmell | PlayerState.CanTouch | PlayerState.CanHear;
	public PlayerState State;
	public bool IsFrozen = false;
	public float MaxHealth = 100.0f;
	public float Health = 100.0f;

}
