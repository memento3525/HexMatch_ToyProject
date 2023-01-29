using UnityEngine;

namespace Mentum.Utility
{
	public class TargetFramerateSetter : MonoBehaviour
	{
		public int targetFramerate = 60;

		private void Start()
		{
			QualitySettings.vSyncCount = 0; // OFF
			Application.targetFrameRate = targetFramerate;
		}
	}
}