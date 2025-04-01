using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldData", menuName = "ScriptableObjects/WorldData", order = 3)]
public class WorldData : ScriptableObject {
	private readonly HashSet<SenseTogglable> _senseTogglables = new();

	public void AddSenseTogglable(SenseTogglable senseTogglable) {
		_senseTogglables.Add(senseTogglable);
	}

	public void RemoveSenseTogglable(SenseTogglable senseTogglable) {
		_senseTogglables.Remove(senseTogglable);
	}

	public HashSet<SenseTogglable> GetSenseTogglables() {
		return _senseTogglables;
	}
}
