using static Unity.Mathematics.math;
using UnityEngine;
using UnityEngine.Splines;
using EasyTransition;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour {
	[SerializeField] private PlayerData _player;
	[SerializeField] private SplineContainer _splinePath;
	[SerializeField] private float _normalOffset = 0.0f;
	[SerializeField] private Transform _floor, _ceil;
	[SerializeField] private TransitionSettings _deathTransition;

	private Rigidbody2D _body;
	private float _splineLength = 0.0f;
	private float _gravityDirection = -1.0f;
	private float _fallVelocity = 0.0f;

	private void Awake() {
		// Validar referencias
		if (_floor == null || _ceil == null) {
			Debug.LogError("Floor o Ceil no asignados en PlayerMovement!");
		}

		if (_splinePath == null || _splinePath.Spline == null) {
			Debug.LogError("SplinePath no asignado o inválido en PlayerMovement!");
		}

		_player.IsFrozen = false;
		_player.Speed = _player.StartSpeed;
		_player.LastCheckpointNsp = 0.0f;
		_player.Nsp = 0.0f;
		_body = GetComponent<Rigidbody2D>();
		_body.gravityScale = 0.0f;
		_splineLength = _splinePath.Spline.GetLength();

		// Inicializar _normalOffset en el centro del rango permitido
		if (_floor != null && _ceil != null) {
			_normalOffset = (_floor.localPosition.y + _ceil.localPosition.y) * 0.5f;
		}
	}

	private void OnEnable() {
		_player.Events.OnDeath += MovePlayerToRespawnCheckpoint;
	}

	private void OnDisable() {
		_player.Events.OnDeath -= MovePlayerToRespawnCheckpoint;
	}

	private void Update() {
		if (_player.IsFrozen) {
			return;
		}

		_gravityDirection = _player.Input.IsMainInputHeld ? 1.0f : -1.0f;
		_player.WorldPosition = transform.position;
	}

	private void FixedUpdate() {
		if (_player.IsFrozen) {
			return;
		}

		_player.Nsp += (_player.Speed * Time.fixedDeltaTime) / _splineLength;
		_player.Nsp %= 1.0f;

		Vector2 splinePos = _splinePath.Spline.EvaluatePosition(_player.Nsp).xy;
		Vector2 splineNormal = _splinePath.Spline.EvaluateUpVector(_player.Nsp).xy;

		float floorY = _floor.localPosition.y;
		float ceilY = _ceil.localPosition.y;

		_fallVelocity += _gravityDirection * _player.GravityScale * Time.fixedDeltaTime;
		_normalOffset += _fallVelocity * Time.fixedDeltaTime;
		_normalOffset = clamp(_normalOffset, floorY, ceilY);

		if (abs(_normalOffset - floorY) < 0.001f || abs(_normalOffset - ceilY) < 0.001f) {
			_fallVelocity = 0.0f;
		}

		Vector2 finalPosition = splinePos + splineNormal * _normalOffset;

		_body.position = finalPosition;
		_body.rotation = Mathf.Atan2(splineNormal.y, splineNormal.x) * Mathf.Rad2Deg;

		_player.RollAngles = _body.rotation;
		_player.NormalOffset = _normalOffset;
	}

	private void MovePlayerToRespawnCheckpoint(float lastCheckpointNsp, float lastCheckpointNormalOffset) {
		_player.Speed = 0.0f;
		_player.Input.Enabled = false;
		_player.IsFrozen = true;

		TransitionManager.Instance.TransitionWithoutSceneLoad(
			_deathTransition,
			onTransitionCutPointReached: () => {
				Vector2 respawnPos = _splinePath.Spline.EvaluatePosition(lastCheckpointNsp).xy;
				respawnPos.y = lastCheckpointNormalOffset;

				_body.position = respawnPos;

				_normalOffset = lastCheckpointNormalOffset;
				_player.Nsp = lastCheckpointNsp;
				_player.Speed = _player.StartSpeed;
				_player.NormalOffset = lastCheckpointNormalOffset;
			},
			onTransitionEnd: () => {
				_player.Input.Enabled = true;
				_player.IsFrozen = false;
				_player.Events.OnRespawn?.Invoke();
			}
		);
	}

	private void OnDrawGizmos() {
		if (_splinePath == null || _splinePath.Spline == null) {
			return;
		}

		// Dibujar posición en el spline
		Gizmos.color = Color.green;
		Vector3 splinePos = _splinePath.Spline.EvaluatePosition(_player.Nsp);
		Gizmos.DrawSphere(splinePos, 0.2f);

		// Dibujar dirección tangente
		Gizmos.color = Color.red;
		Vector3 tangentDir = normalize(_splinePath.Spline.EvaluateTangent(_player.Nsp));
		Gizmos.DrawLine(splinePos, splinePos + tangentDir * 4.0f);

		// Dibujar dirección normal
		Gizmos.color = Color.green;
		Vector3 splineUpDir = normalize(_splinePath.Spline.EvaluateUpVector(_player.Nsp));
		Gizmos.DrawLine(transform.position, transform.position + splineUpDir * 4.0f);
	}
}
