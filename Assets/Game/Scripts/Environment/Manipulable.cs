using static Unity.Mathematics.math;
using System.Collections;
using UnityEngine;

// TODO - If the player exists to fast the scale down coroutine will not be called.
[RequireComponent(typeof(BoxCollider2D))]
public abstract class Manipulable : Interactable {
	public abstract void Manipulate(PlayerData player, float value);

}
