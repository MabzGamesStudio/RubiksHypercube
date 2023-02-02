using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Display timer when solving the cube
/// </summary>
public class CubeTimer : MonoBehaviour
{

	/// <summary>
	/// Text mesh for the display time
	/// </summary>
	private TextMeshProUGUI textMesh;

	/// <summary>
	/// Whether the cube is currently being solved blind
	/// </summary>
	private bool blindMode;

	/// <summary>
	/// Time since start of solve
	/// </summary>
	private float time;

	/// <summary>
	/// Time spent analyzing cube before turning used for blind mode
	/// </summary>
	private float memorizeTime;

	/// <summary>
	/// Whether the timer is giong
	/// </summary>
	private bool timerOn;

	/// <summary>
	/// Whether the cube is complete for special final display
	/// </summary>
	private bool finishTime;

	/// <summary>
	/// Whether the timer is hidden
	/// </summary>
	private bool hideTimer;

	/// <summary>
	/// Initialize the text mesh
	/// </summary>
	void Start()
	{
		textMesh = GetComponentInChildren<TextMeshProUGUI>();
	}

	/// <summary>
	/// Update the timer text
	/// </summary>
	private void Update()
	{

		// If the timer is going then increment time and update text value
		if (timerOn && !hideTimer)
		{
			time += Time.deltaTime;
			UpdateTimerTextMesh();
		}

		// Hide text when timer is hidden
		if (hideTimer)
		{
			textMesh.text = "";
		}
	}

	/// <summary>
	/// Hides the timer
	/// </summary>
	public void HideTimer()
	{
		hideTimer = true;
	}

	/// <summary>
	/// Starts the timer display
	/// </summary>
	public void StartTimer()
	{
		timerOn = true;
		finishTime = false;
		time = 0f;
		hideTimer = false;
	}

	/// <summary>
	/// Sets blind mode on and updates new memorize time
	/// </summary>
	public void EnableBlindMode()
	{
		blindMode = true;
		memorizeTime = time;
	}

	/// <summary>
	/// Sets blind mode off
	/// </summary>
	public void DisableBlindMode()
	{
		blindMode = false;
	}

	/// <summary>
	/// Stops the time and updates to finish time display
	/// </summary>
	public void StopTimer()
	{
		timerOn = false;
		finishTime = true;
		UpdateTimerTextMesh();
	}

	/// <summary>
	/// Updates the display of the timer based on mode and timer value
	/// </summary>
	private void UpdateTimerTextMesh()
	{

		// The formatted time for each solve segment in hh:mm:ss.xx
		string timeFormatted = FloatToTime(time);
		string memorizeTimeFormatted = FloatToTime(memorizeTime);
		string solveTimeFormatted = FloatToTime(time - memorizeTime);

		// Display memorize, solve, and total time when blind mode finished
		if (finishTime && blindMode)
		{
			textMesh.text =
				"Memorize Time:\n" + memorizeTimeFormatted + "\n\n" +
				"Solve Time:\n" + solveTimeFormatted + "\n\n" +
				"Total Time:\n" + timeFormatted;
		}

		// Display total time when finished
		else if (finishTime && !blindMode)
		{
			textMesh.text =
				"Finish Time:\n" + timeFormatted;
		}

		// Display memorize time and total time while blind solving
		else if (!finishTime && blindMode)
		{
			textMesh.text = "<color=\"white\">" + timeFormatted + "\n" +
				"<color=\"grey\"><size=40%>" + memorizeTimeFormatted;
		}

		// Display time while solving
		else if (!finishTime && !blindMode)
		{
			textMesh.text = timeFormatted;
		}
	}

	/// <summary>
	/// Formats the given time into hh:mm:ss.xx
	/// Hours and/or minutes may not be shown if value is 0
	/// </summary>
	/// <param name="time">Time in seconds</param>
	/// <returns>Formatted time in hh:mm:ss.xx</returns>
	private string FloatToTime(float time)
	{
		string timeFormatted = "";

		// Hours, minutes, and seconds of time
		int hours = (int)(time / 3600);
		int minutes = (int)(time / 60) % 60;
		float seconds = time % 60;

		// If time exceeds an hour display hours
		if (time > 3600f)
		{
			timeFormatted += hours + ":";
		}

		// If time exceeds a minute display minutes
		if (time > 60f)
		{
			timeFormatted += minutes + ":";
		}

		// Return formatted time with added seconds
		// with 2 decimal place precision
		return timeFormatted + seconds.ToString("F2");
	}

}