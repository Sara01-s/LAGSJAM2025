using UnityEngine;
using UnityEngine.UIElements;

public class RockBehavior : MonoBehaviour {
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	Rigidbody2D _body;
	void Awake() {
		_body = GetComponent<Rigidbody2D>();
	}
	public void Update() {
		if (CheckCollision()) {
			print("collision");
			DestroyCollision();
		}
	}
	void OnCollisionEnter2D(Collision2D collision) {
		print("collision");
		if (collision.gameObject.CompareTag("Ground")) {
			DestroyCollision();
		}
	}
	private void DestroyCollision() {
		_body.linearVelocity = Vector2.zero;
		//destroy animation then destroy
		Destroy(gameObject);
	}
	public void SetInitialVelocity(Vector2 velocity) {
		_body.linearVelocity = velocity;
	}
	private bool CheckCollision() {
		return Physics2D.OverlapCircle(transform.position, transform.localScale.x * 0.5f, layerMask: 1 << 3);
	}
}
