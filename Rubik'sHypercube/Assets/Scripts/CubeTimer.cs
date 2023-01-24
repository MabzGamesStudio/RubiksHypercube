using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CubeTimer : MonoBehaviour
{

	private TextMeshProUGUI textMesh;

	private bool blindMode;

	private float time;

	private float memorizeTime;

	private bool timerOn;

	private bool finishTime;

	private bool hideTimer;

	// Start is called before the first frame update
	void Start()
	{
		textMesh = GetComponentInChildren<TextMeshProUGUI>();
	}

	private void UpdateTimerTextMesh()
	{
		string timeFormatted = FloatToTime(time);
		string memorizeTimeFormatted = FloatToTime(memorizeTime);
		string solveTimeFormatted = FloatToTime(time - memorizeTime);

		if (finishTime && blindMode)
		{
			textMesh.text =
				"Memorize Time:\n" + memorizeTimeFormatted + "\n\n" +
				"Solve Time:\n" + solveTimeFormatted + "\n\n" +
				"Total Time:\n" + timeFormatted;
		}
		else if (finishTime && !blindMode)
		{
			textMesh.text =
				"Finish Time:\n" + timeFormatted;
		}
		else if (!finishTime && blindMode)
		{
			textMesh.text = "<color=\"white\">" + timeFormatted + "\n" +
				"<color=\"grey\"><size=40%>" + memorizeTimeFormatted;
		}
		else if (!finishTime && !blindMode)
		{
			textMesh.text = timeFormatted;
		}
	}

	private void Update()
	{
		if (timerOn && !hideTimer)
		{
			time += Time.deltaTime;
			UpdateTimerTextMesh();
		}
		if (hideTimer)
		{
			textMesh.text = "";
		}
	}

	private string FloatToTime(float time)
	{
		string timeFormatted = "";
		int hours = (int)(time / 3600);
		int minutes = (int)(time / 60) % 60;
		float seconds = time % 60;
		if (time > 3600f)
		{
			timeFormatted += hours + ":";
		}
		if (time > 60f)
		{
			timeFormatted += minutes + ":";
		}
		timeFormatted += seconds.ToString("F2");

		return timeFormatted;
	}

	public void HideTimer()
	{
		hideTimer = true;
	}

	public void StartTimer()
	{
		timerOn = true;
		finishTime = false;
		time = 0f;
		hideTimer = false;
	}

	public void EnableBlindMode()
	{
		blindMode = true;
		memorizeTime = time;
	}

	public void DisableBlindMode()
	{
		blindMode = false;
	}

	public void StopTimer()
	{
		timerOn = false;
		finishTime = true;
		UpdateTimerTextMesh();
	}
}
