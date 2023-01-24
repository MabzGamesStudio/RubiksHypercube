using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CubeButton : MonoBehaviour
{

	public MeshRenderer meshRenderer;

	public TextMeshProUGUI textMesh;

	public Material regularColor;
	public Material highlightColor;

	private bool timerOn;
	private float timerValue;

	private bool toggleSelection;

	public ButtonType buttonType;

	public enum ButtonType
	{
		Solve,
		Scramble,
		ToggleColors,
		ShowTurning
	};

	private string[] buttonStrings =
	{
		"Solve Cube", "Solve Cube",
		"Scramble", "Scramble",
		"Hide Colors", "Show Colors",
		"Show Turns", "Hide Turns"
	};

	public void HighlightButton()
	{
		timerOn = true;
		timerValue = .2f;
		meshRenderer.material = highlightColor;
	}

	public void SetButtonToggle(bool isOn)
	{
		toggleSelection = isOn;
		if (isOn)
		{
			textMesh.text = buttonStrings[2 * (int)buttonType];
		}
		else
		{
			textMesh.text = buttonStrings[2 * (int)buttonType + 1];
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (timerOn)
		{
			timerValue -= Time.deltaTime;
			if (timerValue <= 0f)
			{
				meshRenderer.material = regularColor;
				toggleSelection = !toggleSelection;
				if (toggleSelection)
				{
					textMesh.text = buttonStrings[2 * (int)buttonType];
				}
				else
				{
					textMesh.text = buttonStrings[2 * (int)buttonType + 1];
				}
				timerOn = false;
			}
		}
	}
}
