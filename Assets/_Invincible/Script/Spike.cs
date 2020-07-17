///-----------------------------------------------------------------
/// Author : #DEVELOPER_NAME#
/// Date : #DATE#
///-----------------------------------------------------------------

using UnityEngine;

namespace Com.Github.Knose1 {
	public class Spike : MonoBehaviour {
		public float force;

		public void Effect(Rigidbody player)
		{
			player.velocity = new Vector3(player.velocity.x, force + Mathf.Abs(player.velocity.y), player.velocity.z);
		}
	}
}