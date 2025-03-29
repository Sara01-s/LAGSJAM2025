using UnityEngine;
using System;

[CreateAssetMenu(fileName = "PlayerEvents", menuName = "ScriptableObjects/PlayerEvents", order = 2)]
public class PlayerEvents : ScriptableObject {
	public struct Inputs {
		public Action OnMainInputHeld;
		public Action OnJumpPressed;
		public Action OnJumpInputHeld;
		public Action OnJumpReleased;
		public Action OnEscInputPressed;
		public Action<float> OnHorizontalHeld;
		public Action OnHorizontalReleased;
		public bool IsJumpInputHeld;
	}

	public Inputs Input;
	public Action OnPlayerLand;
	public Action OnPlayerDeath;
	public Action OnPlayerRespawn;
}
