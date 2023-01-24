using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SolveText : MonoBehaviour
{

	private List<int> moveIndexes;

	private TextMeshProUGUI textMesh;

	// Start is called before the first frame update
	void Start()
	{
		moveIndexes = new List<int>();
		textMesh = GetComponentInChildren<TextMeshProUGUI>();
	}

	public void ClearMoves()
	{
		moveIndexes = new List<int>();
	}

	public void AddMove(int newNumber)
	{
		moveIndexes.Add(newNumber);
	}

	public void HideMoves()
	{
		textMesh.text = "";
	}

	public void ShowMoves()
	{
		string solveText = "";
		foreach (int number in moveIndexes)
		{
			solveText += number + " ";
		}
		solveText += "\nSHA256: " + ScrambleText.SHA256(solveText);
		textMesh.text = solveText;
	}
}
