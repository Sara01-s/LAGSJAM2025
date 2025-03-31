using UnityEngine;

public class Hola : MonoBehaviour {

	void Start() {
		Services.Instance.GetService<IAudioService>().PlaySound("sfx_test");
	}
}
