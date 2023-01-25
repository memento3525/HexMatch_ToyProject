using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFramerateSetter : MonoBehaviour
{
	public int targetFramerate = 60;

	private void Start()
	{
		QualitySettings.vSyncCount = 0; // OFF
		Application.targetFrameRate = targetFramerate;
	}
}
