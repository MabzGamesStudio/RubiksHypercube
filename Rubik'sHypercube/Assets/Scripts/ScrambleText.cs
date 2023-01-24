using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System;
using TMPro;

public class ScrambleText : MonoBehaviour
{

	private string scrambleText;

	private TextMeshProUGUI textMesh;

	private void Start()
	{
		textMesh = GetComponentInChildren<TextMeshProUGUI>();
	}

	public void SetText(string scrambleText)
	{
		this.scrambleText = scrambleText + "\nSHA256: " + SHA256(scrambleText);
	}

	public void HideText()
	{
		textMesh.text = "";
	}

	public void ShowText()
	{
		textMesh.text = scrambleText;
	}

	public static string SHA256(string text)
	{
		byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(text);
		SHA256CryptoServiceProvider sha224 = new SHA256CryptoServiceProvider();
		byte[] hash = sha224.ComputeHash(inputBytes);
		return BitConverter.ToString(hash).Replace("-", "").ToLower();
	}
}
