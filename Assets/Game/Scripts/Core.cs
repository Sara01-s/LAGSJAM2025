using UnityEngine;

public class Core : MonoBehaviour {
	private void Awake() {
		DontDestroyOnLoad(gameObject);

		name = name[..4];
		Services.Instance.RegisterService<IAudioService, AudioPlayer>();
	}

	private void OnDestroy() {
		Services.Instance.Dispose();
	}
}
