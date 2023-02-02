using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detects collision of the controller ray to a sticker or button
/// </summary>
public class RayDetector : MonoBehaviour
{

	/// <summary>
	/// Length of the ray projected from the controller
	/// </summary>
	private static float rayLength = 10;

	/// <summary>
	/// Gets the index of the sticker the ray is pointing at.
	/// If no sticker is pointed at, then -1 is returned
	/// </summary>
	/// <returns>Index of sticker pointed at, otherwise -1</returns>
	public int GetRaySticker()
	{
		RaycastHit hit;

		// If the raycast hits a game object
		if (Physics.Raycast(transform.position, transform.forward, out hit, rayLength))
		{

			// Game object of the hit raycast
			GameObject stickerHit = hit.transform.gameObject;
			try
			{

				// Get the sticker value by the sticker game object name
				int stickerIndex = int.Parse(stickerHit.name.Substring(7));
				return stickerIndex;
			}
			catch
			{

				// Failure to parse indicates hit object is not a sticker
				return -1;
			}

		}

		// No game object was hit
		return -1;
	}

	/// <summary>
	/// Gets the type of button hit
	/// </summary>
	/// <returns>
	/// 1: for solve cube button
	/// 2: for scramble cube button
	/// 3: for toggle color show button
	/// 4: for toggle show turn button
	/// 0: non-button hit
	/// -1: no game object hit
	/// </returns>
	public int GetRayButton()
	{
		RaycastHit hit;


		// If the raycast hits a game object
		if (Physics.Raycast(transform.position, transform.forward, out hit, rayLength))
		{

			// Game object of the hit raycast
			GameObject buttonHit = hit.transform.gameObject;

			// Return integer for button type
			switch (buttonHit.name)
			{
				case "SolveCube":
					return 1;
				case "ScrambleCube":
					return 2;
				case "ColorsCube":
					return 3;
				case "TurnsCube":
					return 4;
			}

			// If non game object hit return 0
			return 0;
		}

		// If no game object hit return -1
		return -1;
	}

}
