using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Script to toggle button and display selection feedback
/// </summary>
public class CubeButton : MonoBehaviour
{

	/// <summary>
	/// Mesh renderer of the button
	/// </summary>
	public MeshRenderer meshRenderer;

	/// <summary>
	/// Text mesh of the button
	/// </summary>
	public TextMeshProUGUI textMesh;

	/// <summary>
	/// Color material when button is not selected
	/// </summary>
	public Material regularColor;

	/// <summary>
	/// Color material when button is selected
	/// </summary>
	public Material highlightColor;

	/// <summary>
	/// Whether the timer is on
	/// </summary>
	private bool timerOn;

	/// <summary>
	/// Value of the timer
	/// </summary>
	private float timerValue;

	/// <summary>
	/// Length of time the timer lasts for
	/// </summary>
	private static float waitTime = .2f;

	/// <summary>
	/// Whether the button is toggled
	/// </summary>
	private bool toggleSelection;

	/// <summary>
	/// The type of this button
	/// </summary>
	public ButtonType buttonType;

	/// <summary>
	/// Types of all buttons in the game
	/// </summary>
	public enum ButtonType
	{
		Solve,
		Scramble,
		ToggleColors,
		ShowTurning,
		To4DCube,
		Credits,
		CreditsBack,
		To3DCube
	};

	/// <summary>
	/// Texts the button toggles between
	/// </summary>
	private string[] buttonStrings =
	{
		"Solve Cube", "Solve Cube",
		"Scramble", "Scramble",
		"Hide Colors", "Show Colors",
		"Show Turns", "Hide Turns",
		"4D Cube", "4D Cube",
		"Credits", "Credits",
		"Back", "Back",
		"3D Cube", "3DCube"

	};

	/// <summary>
	/// Updates the timer and resets the highlight if the wait time has passed
	/// </summary>
	private void Update()
	{
		if (timerOn)
		{

			// Update timer value
			timerValue -= Time.deltaTime;

			// If the full time has passed
			if (timerValue <= 0f)
			{

				// Reset the highlight color and toggle text
				meshRenderer.material = regularColor;
				toggleSelection = !toggleSelection;

				// Enum integer value dictates which button type and text value
				if (toggleSelection)
				{
					textMesh.text = buttonStrings[2 * (int)buttonType];
				}
				else
				{
					textMesh.text = buttonStrings[2 * (int)buttonType + 1];
				}

				// Turn off the timer
				timerOn = false;
			}
		}
	}

	/// <summary>
	/// Selects the button and highlights it
	/// </summary>
	public void HighlightButton()
	{
		timerOn = true;
		timerValue = waitTime;
		meshRenderer.material = highlightColor;
	}

	/// <summary>
	/// Sets the toggle display of the button
	/// </summary>
	/// <param name="isOn">Whether to display the button toggle on</param>
	public void SetButtonToggle(bool isOn)
	{
		toggleSelection = isOn;

		// Enum integer value dictates which button type and text value
		if (isOn)
		{
			textMesh.text = buttonStrings[2 * (int)buttonType];
		}
		else
		{
			textMesh.text = buttonStrings[2 * (int)buttonType + 1];
		}
	}

}
