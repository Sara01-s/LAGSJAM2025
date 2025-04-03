using UnityEngine;
using System.Collections;

public class Fireflies : MonoBehaviour {

	public float timer;
	private Collider2D lightHitbox;
	private SpriteRenderer sprite;
	private void Awake() {
		lightHitbox = GetComponentInChildren<Collider2D>();
		sprite = GetComponentInChildren<SpriteRenderer>();
	}
	private void OnTriggerEnter2D(Collider2D trigger) {
		print("something");
		if (trigger.CompareTag("Throwable")) {
			print("disperse");
			DisperseFireflies();
			StartCoroutine(RegroupFireflies());

		}
	}
	private void OnCollisionEnter2D(Collision2D collision) {
		if (collision.collider.CompareTag("Throwable")) {
			DisperseFireflies();
			StartCoroutine(RegroupFireflies());
		}
	}
	private void DisperseFireflies() {
		print("disperse");
		lightHitbox.enabled = false;
		var col = sprite.material.color;
		col.a = 0.2f;
		sprite.color = col;
	}
	private IEnumerator RegroupFireflies() {
		yield return new WaitForSeconds(timer);
		print("regroup");
		lightHitbox.enabled = true;
		var col = sprite.material.color;
		col.a = 1f;
		sprite.color = col;
	}

}
