///-----------------------------------------------------------------
/// Author : #DEVELOPER_NAME#
/// Date : #DATE#
///-----------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace Com.Github.Knose1 {
	public class TexteurSetteur : MonoBehaviour {
		public Text text1 = null;
		public Text text2 = null;

		public void Text2IsBetter()
		{
			text1.gameObject.SetActive(false);
			text2.gameObject.SetActive(true);
		}
	}
}