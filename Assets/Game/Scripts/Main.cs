using UnityEngine;
using UnityEngine.Assertions;

public static class Main {
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void SpawnCore() {
		var core = Resources.Load<GameObject>("Core");

		Object.Instantiate(core);
		Assert.IsNotNull(core, "Core prefab not found in Resources folder.");
	}
}
