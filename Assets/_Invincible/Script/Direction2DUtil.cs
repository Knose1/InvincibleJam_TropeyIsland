
using UnityEngine;
///-----------------------------------------------------------------
/// Author : #DEVELOPER_NAME#
/// Date : #DATE#
///-----------------------------------------------------------------
namespace Com.Github.Knose1
{
	public static class Direction2DUtil
	{
		private static Quaternion _right = Quaternion.AngleAxis(90, Vector3.up);
		private static Quaternion _left = Quaternion.AngleAxis(-90, Vector3.up);
		private static Quaternion _back = Quaternion.AngleAxis(180, Vector3.up);

		public static Vector3 GetRight(Vector3 v) => _right * v;
		public static Vector3 GetLeft (Vector3 v) => _left  * v;
		public static Vector3 GetBack (Vector3 v) => _back  * v;
	}
}