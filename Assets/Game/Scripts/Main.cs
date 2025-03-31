using UnityEngine;

public class Main : MonoBehaviour {

	private void Awake() {
		Services.Instance.RegisterService<IAudioService, AudioPlayer>();
	}

	private void Start() {
		var audioService = Services.Instance.GetService<IAudioService>();
		audioService.PlaySound("sfx_test");
	}

	private void OnDestroy() {
		Services.Instance.Dispose();
	}

}
