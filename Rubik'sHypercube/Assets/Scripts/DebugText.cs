using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugText : MonoBehaviour
{

	private static TextMeshProUGUI textMesh;

	private static int maxCharacters = 1000;

	// Start is called before the first frame update
	void Start()
	{
		textMesh = GetComponent<TextMeshProUGUI>();
	}


	public static void Log(string text)
	{
		string combinedText = text + "\n" + textMesh.text;
		textMesh.text = combinedText.Substring(0, Mathf.Min(combinedText.Length, maxCharacters));
	}

	public static void Log(int text)
	{
		string combinedText = text.ToString() + "\n" + textMesh.text;
		textMesh.text = combinedText.Substring(0, Mathf.Min(combinedText.Length, maxCharacters));
	}

	public static void Log(float text)
	{
		string combinedText = text.ToString() + "\n" + textMesh.text;
		textMesh.text = combinedText.Substring(0, Mathf.Min(combinedText.Length, maxCharacters));
	}

	public static void Log(float[] text)
	{
		string arrayText = "[ ";
		foreach (float number in text)
		{
			arrayText += number.ToString() + ", ";
		}
		arrayText += "]";
		string combinedText = arrayText + "\n" + textMesh.text;
		textMesh.text = combinedText.Substring(0, Mathf.Min(combinedText.Length, maxCharacters));
	}

	public static void Log(string[] text)
	{
		string arrayText = "[ ";
		foreach (string number in text)
		{
			arrayText += number.ToString() + ", ";
		}
		arrayText += "]";
		string combinedText = arrayText + "\n" + textMesh.text;
		textMesh.text = combinedText.Substring(0, Mathf.Min(combinedText.Length, maxCharacters));
	}

	public static void Log(int[] text)
	{
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
