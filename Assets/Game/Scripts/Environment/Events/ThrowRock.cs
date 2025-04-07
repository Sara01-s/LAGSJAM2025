using System;
using UnityEngine;

public class ThrowRock : Manipulable {
	public GameObject prefab;
	private bool active = false;
	public GameObject arrow;
	public float velocity = 1;
	private GameObject savedArrow;
	public SpriteRenderer sprite;
	private float angleZ = 0;
	private float limit = 180;
	private float add = 0;
	private Collider2D _trigger;
	public override void Awake() {
		base.Awake();
	}
	public override void Interact(PlayerData player) {
		SummonRock();
	}
	public override void Manipulate(PlayerData player, float value) {
		print(value);
		add = value * 0.25f;
	}
	public override void OnTriggerEnter2D(Collider2D trigger) {
		base.OnTriggerEnter2D(trigger);
		if (trigger.CompareTag("Player")) {
			_trigger = trigger;
			active = true;
			savedArrow = Instantiate(arrow, trigger.transform.position, Quaternion.identity);
			savedArrow.transform.SetParent(trigger.transform);
			savedArrow.GetComponent<PointerCurve>().Draw(0, 10, velocity, 9.8f);
		}
	}
	public override void OnTriggerExit2D(Collider2D trigger) {
		base.OnTriggerExit2D(trigger);
		if (trigger.CompareTag("Player")) {
			active = false;
			Destroy(savedArrow);
		}
	}
	public void Update() {
		if (active & add != 0) {
			angleZ = Math.Max(Math.Min(angleZ + add, limit), 0);
			savedArrow.GetComponent<PointerCurve>().Draw(angleZ, 10, velocity, 9.8f);
			//savedArrow.transform.eulerAngles = new Vector3(0, 0, angleZ);
		}
	}
	private void SummonRock() {
		Vector2 direction = new Vector2(Mathf.Cos(angleZ * Mathf.Deg2Rad), Mathf.Sin(angleZ * Mathf.Deg2Rad));
		GameObject rock = Instantiate(prefab, _trigger.transform.position + new Vector3(0, 0.5f), transform.rotation);
		rock.transform.SetParent(_trigger.transform);
		Rigidbody2D body = rock.GetComponent<Rigidbody2D>();
		body.linearVelocity = direction * velocity;
	}
}
