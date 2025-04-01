using UnityEngine;

public class SenseTogglable : MonoBehaviour {
	[SerializeField] private WorldData _world;

	[Tooltip("This gameobject will be active only in the specified state.")]
	[field: SerializeField] public PlayerState State { get; private set; }

	private void OnEnable() => _world.AddSenseTogglable(this);
	private void OnDestroy() => _world.RemoveSenseTogglable(this);
}
