using UnityEngine;

public class Main : MonoBehaviour {
	private void Awake() {
		Services.Instance.RegisterService<IAudioService, AudioPlayer>();
	}

	private void OnDestroy() {
		Services.Instance.Dispose();
	}
}
