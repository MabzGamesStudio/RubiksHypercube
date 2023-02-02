using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Text that displays the index moves throughout the solve and its SHA256
/// </summary>
public class SolveText : MonoBehaviour
{

	/// <summary>
	/// The moves in the solve by sticker index value
	/// </summary>
	private List<int> moveIndexes;

	/// <summary>
	/// Text mesh that displays the information
	/// </summary>
	private TextMeshProUGUI textMesh;


	/// <summary>
	/// Initializes the moves list and text mesh
	/// </summary>
	void Start()
	{
		moveIndexes = new List<int>();
		textMesh = GetComponentInChildren<TextMeshProUGUI>();
	}

	/// <summary>
	/// Resets the list of moves
	/// </summary>
	public void ClearMoves()
	{
		moveIndexes = new List<int>();
	}

	/// <summary>
	/// Adds the new turn sticker index to the moves list
	/// </summary>
	/// <param name="newNumber">Index of the sticker turned clockwise</param>
	public void AddMove(int newNumber)
	{
		moveIndexes.Add(newNumber);
	}

	/// <summary>
	/// Hide the moves text
	/// </summary>
	public void HideMoves()
	{
		textMesh.text = "";
	}

	/// <summary>
	/// Displays the moves and SHA256 in the text mesh
	/// </summary>
	public void ShowMoves()
	{

		// Adds each turn index separated by a space
		string solveText = "";
		foreach (int number in moveIndexes)
		{
			solveText += number + " ";
		}

		// Adds the SHA256 value of the moves text list
		solveText += "\nSHA256: " + ScrambleText.SHA256(solveText);

		// Sets the text mesh to the constructed text
		textMesh.text = solveText;
	}

}
