using UnityEngine;

public class Potion : Interactable {
	[SerializeField] private float _healthModifier;
	[SerializeField] private PlayerState _drinkableState;
	[SerializeField] private bool _destroyOnConsume;

	public override void Interact(PlayerData player) {
		if (player.Health == player.MaxHealth && _healthModifier > 0.0f) {
			Services.Instance.GetService<IAudioService>().PlaySound("sfx_ph_error", volume: 0.7f);
			return;
		}

		if (_healthModifier < 0.0f || !player.State.HasFlag(_drinkableState)) {
			// We send the inverse of the health modifier because hurt system expects only positive values.
			player.Events.OnPlayerHurt?.Invoke(new DamageInfo() { DamageAmount = -_healthModifier });

			if (_destroyOnConsume) {
				Destroy(gameObject);
			}
			return;
		}

		// Heal.
		float previousHealth = player.Health;
		player.Health += _healthModifier;
		player.Events.OnHealthChanged?.Invoke(player.Health, previousHealth);

		if (_destroyOnConsume) {
			Destroy(gameObject);
		}
	}
}
