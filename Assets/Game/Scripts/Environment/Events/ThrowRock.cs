using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class ThrowRock : Interactable {
	public GameObject prefab;
	private bool active = false;
	private PlayerData _player;
	public GameObject arrow;
	public float velocity = 1;
	private GameObject savedArrow;
	public SpriteRenderer sprite;
	public override void Awake() {
		base.Awake();

	}
	public override void Interact(PlayerData player) {
		SummonRock();
	}
	public override void OnTriggerEnter2D(Collider2D trigger) {
		base.OnTriggerEnter2D(trigger);
		if (trigger.CompareTag("Player")) {
			savedArrow = Instantiate(arrow, trigger.transform.position, Quaternion.identity);
			savedArrow.transform.SetParent(trigger.transform);

		}
	}
	public override void OnTriggerExit2D(Collider2D trigger) {
		base.OnTriggerExit2D(trigger);
		if (trigger.CompareTag("Player")) {
			Destroy(savedArrow);
		}
	}
	public void Update() {
		if (active & _player != null) {
			//actualizar el angulo de la "flecha" el punto alrededor del player
			//el angulo es la rotacion z del objeto (en grados)
		}
	}
	private void SummonRock() {
		float angleZ = savedArrow.transform.eulerAngles.z;
		Vector2 direction = new Vector2(Mathf.Cos(angleZ * Mathf.Deg2Rad), Mathf.Sin(angleZ * Mathf.Deg2Rad));
		GameObject rock = Instantiate(prefab, transform.position + new Vector3(0, 0.5f), transform.rotation);
		Rigidbody2D body = rock.GetComponent<Rigidbody2D>();
		body.linearVelocity = direction * velocity;
	}
}
