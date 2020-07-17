///-----------------------------------------------------------------
/// Author : #DEVELOPER_NAME#
/// Date : #DATE#
///-----------------------------------------------------------------

using Cinemachine;
using System;
using UnityEngine;
using static Com.Github.Knose1.Common.Twinning.Curve.Cube;

namespace Com.Github.Knose1
{
	[RequireComponent(typeof(Rigidbody))]
	public partial class Cube : MonoBehaviour
	{
		public string inputVertical   = "Vertical";
		public string inputHorizontal = "Horizontal";

		/// <summary>
		/// Time to move in seconds
		/// </summary>
		public float movingTime = 1;
		public float moveDistance = 3;
		public float moveHeight = 2;
		public float moveSpeedWhenFalling;
		public float idleTime = 1;
		public float idleAnimationTime = 1;
		public float raycastSize = 0.2f;
		public float raycastCheckIsGroundSize = 0.2f;
		public LayerMask layerMask;
		public LayerMask spikeLayerMask;
		public Rigidbody cube = null;
		public Transform virtualCamera = null;
		public Transform spawnPoint = null;
		public Animator damageAnimator = null;
		public string damageParameter = "damage";
		public TexteurSetteur texteurSetteur = null;

		private bool isQuaternionIdentity = true;

		private float startTimestamp = 0;

		private Vector3 move = Vector2.zero;

		private float angle = 0;
		private Vector3 direction;
		private Vector3 middleVertexDirection;
		private Vector3 middleVertex;

		private Vector3 startPos;
		private Quaternion startRotation;

		private Action doAction;
		private Rigidbody rb;

		private void Start()
		{
			rb = GetComponent<Rigidbody>();
			SetModeVoid();
		}

		private void Update()
		{
			if (rb.position.y < -150)
			{
				virtualCamera.GetComponent<CinemachineVirtualCamera>().enabled = false;
				Vector3 pos = virtualCamera.position;
				pos.y += spawnPoint.position.y - rb.position.y;

				rb.position = spawnPoint.position;
				virtualCamera.position = pos;
				virtualCamera.GetComponent<CinemachineVirtualCamera>().enabled = true;

				doAction = DoActionVoid;
				return;
			}

			move.x = -Input.GetAxis(inputHorizontal);
			move.z = -Input.GetAxis(inputVertical);
			move = Vector3.ClampMagnitude(move, 1);

			bool isHitSpike = Physics.Raycast(rb.position, Vector3.down, out RaycastHit hit, raycastCheckIsGroundSize, spikeLayerMask);
			Spike spike = null;
			if (isHitSpike && (spike = hit.transform.GetComponent<Spike>()))
			{
				spike.Effect(rb);
				doAction = DoActionVoid;
				isQuaternionIdentity = false;
				damageAnimator.SetTrigger(damageParameter);
				texteurSetteur.Text2IsBetter();
			}

			if (doAction != DoActionVoid)
			{
				doAction();
				return;
			}
			
			if (move != Vector3.zero)
			{
				//VC pos
				Vector3 vcPos = virtualCamera.position;
				vcPos.y = rb.position.y;

				//Direction
				direction = vcPos - rb.position;
				direction = Quaternion.LookRotation(move, Vector3.up) * direction;
				direction = direction.normalized;

				Vector3 cubeLocalScale = cube.transform.localScale;

				direction.x *= cubeLocalScale.x + 1;
				direction.y *= cubeLocalScale.y + 1;
				direction.z *= cubeLocalScale.z + 1;

				direction.x = Mathf.Clamp(direction.x, -cubeLocalScale.x, cubeLocalScale.x);
				direction.y = Mathf.Clamp(direction.y, -cubeLocalScale.y, cubeLocalScale.y);
				direction.z = Mathf.Clamp(direction.z, -cubeLocalScale.z, cubeLocalScale.z);

				direction /= 2;
			}

			Debug.DrawRay(rb.position, Vector3.down * raycastCheckIsGroundSize, Color.cyan);
			bool isHit = Physics.Raycast(rb.position, Vector3.down, raycastCheckIsGroundSize, layerMask);
			if (isHit)
			{
				doAction();

				if (move != Vector3.zero)
				{
					SetModeNormal();
				}
				return;
			}

			rb.velocity = Mathf.Clamp(rb.velocity.y, -20, 20) * Vector3.up + direction * move.magnitude * moveSpeedWhenFalling;

		}

		private void SetModeVoid()
		{
			startTimestamp = Time.time;

			doAction = DoActionVoid;
		}


		private void DoActionVoid()
		{
			float lerp = (Time.time - startTimestamp) / idleTime;
			if (lerp >= 1 && !isQuaternionIdentity)
			{
				SetModeIdle();
			}
		}

		private void SetModeIdle()
		{
			startPos = rb.position;
			startRotation = cube.rotation;
			startTimestamp = Time.time;

			doAction = DoActionIdle;
		}

		private void DoActionIdle()
		{
			float lerp = (Time.time - startTimestamp) / idleAnimationTime;
			if (lerp > 1)
			{
				isQuaternionIdentity = true;
				SetModeVoid();
				return;
			}

			rb.MovePosition(startPos + Vector3.up * Mathf.Sin(lerp * 180 * Mathf.Deg2Rad) * 3);
			cube.MoveRotation(Quaternion.Lerp(startRotation, Quaternion.identity, lerp));
		}

		private void SetModeNormal()
		{
			if (doAction != DoActionVoid) return;
			//Obvious stuff
			startPos = rb.position;
			startRotation = cube.rotation;
			startTimestamp = Time.time;

			Debug.DrawRay(rb.position + cube.transform.localScale / 2, direction * raycastSize, Color.cyan, 5);
			if (Physics.BoxCast(rb.position, cube.transform.localScale / 2, direction, cube.rotation, raycastSize, layerMask))
			{
				return;
			}

			//DrawLine
			//Debug.DrawLine(startPos, vcPos, Color.red, movingTime);
			//Debug.DrawRay(transform.position, direction, Color.green, movingTime);
			//Debug.DrawRay(transform.position, move, Color.blue, movingTime);

			SetupMiddleVertex();
			doAction = DoActionNormal;
		}

		protected void DoActionNormal()
		{

			if (angle >= 90) return;

			float lerp = (Time.time - startTimestamp) / movingTime;
			if (lerp >= 1)
			{
				ExecuteRotation(90);

				startRotation = cube.rotation;

				isQuaternionIdentity = false;

				SetupMiddleVertex();
				SetModeVoid();
				return;
			}

			angle = 90 * new Sin().In(lerp);
			ExecuteRotation(angle);

		}

		/*///////////////////////////////////////*/
		private void SetupMiddleVertex()
		{
			middleVertexDirection = direction + cube.transform.localScale.y / 2 * Vector3.down;
			middleVertex = startPos + middleVertexDirection;

			angle = 0;

			Debug.DrawLine(middleVertex, startPos, Color.blue, 1);
		}

		private void ExecuteRotation(float angle)
		{
			cube.MoveRotation(Quaternion.AngleAxis(angle, Direction2DUtil.GetRight(direction)) * startRotation);

			Vector3 toUp = -middleVertexDirection.y * Vector3.up * Mathf.Sin((angle * 2) * Mathf.Deg2Rad) * moveHeight;
			Vector3 toForward = direction * 2 * Mathf.Sin((angle) * Mathf.Deg2Rad) * moveDistance;
			Vector3 pos = -middleVertexDirection.y * Vector3.up + toForward + toUp - direction;

			Debug.DrawRay(middleVertex, toUp, Color.red);
			Debug.DrawRay(middleVertex, toForward, Color.green);

			if (Physics.BoxCast(rb.position, cube.transform.localScale / 2, pos, cube.rotation, pos.magnitude / 2, layerMask))
			{
				return;
			}

			rb.MovePosition(pos + middleVertex);
		}
	}
}