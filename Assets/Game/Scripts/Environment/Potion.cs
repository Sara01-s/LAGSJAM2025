using UnityEngine;

public class Potion : Interactable {
	[SerializeField] private float _healthModifier;
	[SerializeField] private PlayerState _drinkableState;

	public override void Interact(PlayerData player) {
		if (player.Health == player.MaxHealth && _healthModifier > 0.0f) {
			Services.Instance.GetService<IAudioService>().PlaySound("sfx_ph_error", volume: 0.5f);
			return;
		}

		if (_healthModifier < 0.0f || !player.State.HasFlag(_drinkableState)) {
			// We sent the inverse of the health modifier because hurt system expect only positive values.
			player.Events.OnPlayerHurt?.Invoke(new DamageInfo() { DamageAmount = -_healthModifier });
			return;
		}

		// Heal.
		float previousHealth = player.Health;
		player.Health += _healthModifier;
		player.Events.OnHealthChanged?.Invoke(player.Health, previousHealth);
	}
}
