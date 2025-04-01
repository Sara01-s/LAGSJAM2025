using UnityEngine;
using System;

[CreateAssetMenu(fileName = "PlayerEvents", menuName = "ScriptableObjects/PlayerEvents", order = 2)]
public class PlayerEvents : ScriptableObject {
	public struct Inputs {
		// Movement.
		public Action<float> OnHorizontalHeld;
		public Action OnHorizontalReleased;
		public Action OnJumpPressed;
		public Action OnJumpInputHeld;
		public bool IsJumpInputHeld;
		public Action OnJumpReleased;

		// Senses.
		public Action OnSightPressed;
		public Action OnSmellPressed;
		public Action OnTastePressed;
		public Action OnTouchPressed;
		public Action OnHearingPressed;

		// Interaction.
		public Action OnInteractPressed;

		// Navigation.
		public Action OnEscInputPressed;
	}

	public Inputs Input;
	public Action OnPlayerLand;
	public Action OnPlayerDeath;
	public Action OnPlayerRespawn;
	public Action<DamageInfo> OnPlayerHurt;
	public Action<PlayerState> OnPlayerStateChanged;
}
