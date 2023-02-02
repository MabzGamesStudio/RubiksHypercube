using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Displays debug text to text mesh while in VR
/// </summary>
public class DebugText : MonoBehaviour
{

	/// <summary>
	/// Text mesh to display the debug text
	/// </summary>
	public static TextMeshProUGUI textMesh;

	/// <summary>
	/// Max number of characters displayed int he text at one time
	/// </summary>
	private static int maxCharacters = 1000;

	/// <summary>
	/// Log a line of the given text
	/// </summary>
	/// <param name="text">string value of the text</param>
	public static void Log(string text)
	{
		string combinedText = text + "\n" + textMesh.text;
		textMesh.text = combinedText.Substring(0, Mathf.Min(combinedText.Length, maxCharacters));
	}

	/// <summary>
	/// Log a line of the given text
	/// </summary>
	/// <param name="text">int value of the text</param>
	public static void Log(int text)
	{
		string combinedText = text.ToString() + "\n" + textMesh.text;
		textMesh.text = combinedText.Substring(0, Mathf.Min(combinedText.Length, maxCharacters));
	}

	/// <summary>
	/// Log a line of the given text
	/// </summary>
	/// <param name="text">float value of the text</param>
	public static void Log(float text)
	{
		string combinedText = text.ToString() + "\n" + textMesh.text;
		textMesh.text = combinedText.Substring(0, Mathf.Min(combinedText.Length, maxCharacters));
	}

	/// <summary>
	/// Log a line of the given text
	/// </summary>
	/// <param name="text">string[] value of the text</param>
	public static void Log(float[] text)
	{

		// Log each value separated by a comma and enclose in brackets
		string arrayText = "[ ";
		foreach (float number in text)
		{
			arrayText += number.ToString() + ", ";
		}
		arrayText += "]";
		string combinedText = arrayText + "\n" + textMesh.text;
		textMesh.text = combinedText.Substring(0, Mathf.Min(combinedText.Length, maxCharacters));
	}

	/// <summary>
	/// Log a line of the given text
	/// </summary>
	/// <param name="text">int[] value of the text</param>
	public static void Log(string[] text)
	{

		// Log each value separated by a comma and enclose in brackets
		string arrayText = "[ ";
		foreach (string number in text)
		{
			arrayText += number.ToString() + ", ";
		}
		arrayText += "]";
		string combinedText = arrayText + "\n" + textMesh.text;
		textMesh.text = combinedText.Substring(0, Mathf.Min(combinedText.Length, maxCharacters));
	}

	/// <summary>
	/// Log a line of the given text
	/// </summary>
	/// <param name="text">float[] value of the text</param>
	public static void Log(int[] text)
	{

		// Log each value separated by a comma and enclose in brackets
		string arrayText = "[ ";
		foreach (int number in text)
		{
			arrayText += number.ToString() + ", ";
		}
		arrayText += "]";
		string combinedText = arrayText + "\n" + textMesh.text;
		textMesh.text = combinedText.Substring(0, Mathf.Min(combinedText.Length, maxCharacters));
	}

}
