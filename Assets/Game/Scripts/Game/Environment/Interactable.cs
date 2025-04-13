using UnityEngine;

public abstract class Interactable : MonoBehaviour {
	public virtual void Interact(PlayerData player) { }
	public virtual void OnInteractEnter(PlayerData player) { }
	public virtual void OnInteractExit(PlayerData player) { }
	public virtual void OnInteractStay(PlayerData player) { }
}
