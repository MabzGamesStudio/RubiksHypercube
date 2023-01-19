using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayDetector : MonoBehaviour
{

	private float rayLength = 10;

	public int GetRaySticker()
	{
		RaycastHit hit;

		if (Physics.Raycast(transform.position, transform.forward, out hit, rayLength))
		{
			GameObject stickerHit = hit.transform.gameObject;
			try
			{
				int stickerIndex = int.Parse(stickerHit.name.Substring(7));
				return stickerIndex;
			}
			catch
			{
				return 0;
			}

		}
		return -1;
	}

	public int GetRayButton()
	{
		RaycastHit hit;

		if (Physics.Raycast(transform.position, transform.forward, out hit, rayLength))
		{
			GameObject buttonHit = hit.transform.gameObject;
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
			return 0;
		}
		return -1;
	}
}
