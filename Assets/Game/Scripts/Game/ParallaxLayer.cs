using UnityEngine;

public class ParallaxLayer : MonoBehaviour {
	[SerializeField] private PlayerData _player;
	[SerializeField] private float _speedFactor = 1.0f;

	private void Update() {
		if (_player.IsFrozen) {
			return;
		}

		float speed = _player.Speed * _speedFactor;
		transform.position += speed * Time.deltaTime * transform.right;
	}
}
