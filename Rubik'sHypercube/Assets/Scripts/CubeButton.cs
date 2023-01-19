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
		"Show Colors", "Hide Colors",
		"Show Turns", "Hide Turns"
	};

	public void HighlightButton()
	{
		timerOn = true;
		timerValue = .2f;
		meshRenderer.material = highlightColor;
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
