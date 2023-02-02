using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System;
using TMPro;

/// <summary>
/// Text that displays the scramble sticker index moves and its SHA256 value
/// </summary>
public class ScrambleText : MonoBehaviour
{

	/// <summary>
	/// Text of sticker integer move list and SHA256 value
	/// </summary>
	private string scrambleText;

	/// <summary>
	/// Text mesh that displays text
	/// </summary>
	private TextMeshProUGUI textMesh;

	/// <summary>
	/// Initialize text mesh
	/// </summary>
	private void Start()
	{
		textMesh = GetComponentInChildren<TextMeshProUGUI>();
	}

	/// <summary>
	/// Sets the moves sticker index list, then adds the SHA256 value
	/// </summary>
	/// <param name="scrambleText">
	/// Space separated list of sticker index clockwise moves
	/// </param>
	public void SetText(string scrambleText)
	{
		this.scrambleText = scrambleText + "\nSHA256: " + SHA256(scrambleText);
	}

	/// <summary>
	/// Hides the scramble text
	/// </summary>
	public void HideText()
	{
		textMesh.text = "";
	}

	/// <summary>
	/// Displays the scramble text string to the text mesh
	/// </summary>
	public void ShowText()
	{
		textMesh.text = scrambleText;
	}

	/// <summary>
	/// Converts the input text to the SHA256 hexadecimal value
	/// </summary>
	/// <param name="text">Text string treated as ASCII characters</param>
	/// <returns>Hexadecimal value of the SHA256 value</returns>
	public static string SHA256(string text)
	{
		byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(text);
		SHA256CryptoServiceProvider sha224 = new SHA256CryptoServiceProvider();
		byte[] hash = sha224.ComputeHash(inputBytes);
		return BitConverter.ToString(hash).Replace("-", "").ToLower();
	}
}
