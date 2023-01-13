using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HypercubeTurner : MonoBehaviour
{

	public GameObject insidePrefab;

	public GameObject distortedPrefab;

	public GameObject turnAxisPrefab;

	public Material[] colors;

	public Material[] colorsSelected;

	private GameObject[] cubes;

	private GameObject[] cubies;

	private MeshRenderer[] cubiesMeshRenderers;

	public enum TurnAxis
	{
		FaceTop,
		FaceRight,
		FaceFront,
		EdgeTopFront,
		EdgeTopRight,
		EdgeTopBack,
		EdgeTopLeft,
		EdgeFrontRight,
		EdgeFrontLeft,
		CornerTopFrontRight,
		CornerTopFrontLeft,
		CornerTopBackLeft,
		CornerTopBackRight,
		None
	}

	private enum StickerType
	{
		Center,
		Middle,
		Edge,
		Corner
	}

	// Turn axis index order:
	// 0: FaceTop, 1: FaceRight, 2: FaceFront, 3: EdgeTopFront, 4: EdgeTopRight, 5: EdgeTopBack,
	// 6: EdgeTopLeft, 7: EdgeFrontRight, 8: EdgeFrontLeft, 9: CornerTopFrontRight,
	// 10: CornerTopFrontLeft, 11: CornerTopBackLeft, 12: CornerTopBackRight


	private GameObject[] turnAxis;
	private List<int[]> turns;

	private int[] cubeColors;

	private GameObject mainSection;

	private bool showingColors = true;

	private bool isTurning;
	private float turnTimer = 0f;
	private TurnAxis currentTurnAxis;
	private int currentTurnSticker;
	private float currentTurnDegrees;
	private TurnType currentTurnType;

	private float turnTime = .1f;

	private float fadeStickerTime = .5f;

	public enum TurnType
	{
		Clockwise,
		HalfTurn,
		CounterClockwise,
		None
	}

	private string[] sideNames = new string[]
	{
		"Inside", "Front", "Back", "Top", "Bottom", "Right", "Left", "Outside"
	};

	/*
	 * 
	 * Cubies order:
	 * Bottom, Middle, Top
	 * Front, Middle, Back
	 * Left, Middle, Right
	 * 
	 */

	private Vector3[] cubesEulerAngles = new Vector3[]
	{
		new Vector3(0, 0, 0),
		new Vector3(-90, 0, 0),
		new Vector3(90, 0, 0),
		new Vector3(0, 0, 0),
		new Vector3(180, 0, 0),
		new Vector3(0, 0, -90),
		new Vector3(0, 0, 90),
		new Vector3(0, 0, 0)
	};

	private Vector3[] cubePositions = new Vector3[]
	{
		new Vector3(0, 0, 0),
		new Vector3(0, 0, -6),
		new Vector3(0, 0, 6),
		new Vector3(0, 6, 0),
		new Vector3(0, -6, 0),
		new Vector3(6, 0, 0),
		new Vector3(-6, 0, 0),
		new Vector3(12, 12, 12)
	};

	private int[] cubiesMapping = new int[]
	{

		// White
		0, 1, 2, 3, 4, 5, 6, 7, 8,
		9, 10, 11, 12, 13, 14, 15, 16, 17,
		18, 19, 20, 21, 22, 23, 24, 25, 26,

		// Pink
		33, 34, 35, 42, 43, 44, 51, 52, 53,
		30, 31, 32, 39, 40, 41, 48, 49, 50,
		27, 28, 29, 36, 37, 38, 45, 46, 47,

		// Purple
		72, 73, 74, 63, 64, 65, 54, 55, 56,
		75, 76, 77, 66, 67, 68, 57, 58, 59,
		78, 79, 80, 69, 70, 71, 60, 61, 62,
		
		// Green
		81, 82, 83, 84, 85, 86, 87, 88, 89,
		90, 91, 92, 93, 94, 95, 96, 97, 98,
		99, 100, 101, 102, 103, 104, 105, 106, 107,
		
		// Blue
		132, 133, 134, 129, 130, 131, 126, 127, 128,
		123, 124, 125, 120, 121, 122, 117, 118, 119,
		114, 115, 116, 111, 112, 113, 108, 109, 110,
		
		// Red
		153, 144, 135, 156, 147, 138, 159, 150, 141,
		154, 145, 136, 157, 148, 139, 160, 151, 142,
		155, 146, 137, 158, 149, 140, 161, 152, 143,
		
		// Orange
		164, 173, 182, 167, 176, 185, 170, 179, 188,
		163, 172, 181, 166, 175, 184, 169, 178, 187,
		162, 171, 180, 165, 174, 183, 168, 177, 186,
		
		// Yellow
		189, 190, 191, 192, 193, 194, 195, 196, 197,
		198, 199, 200, 201, 202, 203, 204, 205, 206,
		207, 208, 209, 210, 211, 212, 213, 214, 215,
	};

	// Start is called before the first frame update
	void Start()
	{
		SetupHypercubeStructure();
		AssignCubies();
		SetupTurnAxis();
	}

	// Update is called once per frame
	private void Update()
	{
		if (isTurning)
		{
			turnTimer += Time.deltaTime;
			float percent = turnTimer / turnTime;

			if (percent <= 1f)
			{
				if (currentTurnAxis != TurnAxis.None)
				{
					turnAxis[(int)currentTurnAxis].transform.localEulerAngles = new Vector3(0, currentTurnDegrees * percent);
				}
				cubiesMeshRenderers[currentTurnSticker - 1].material = colorsSelected[cubeColors[currentTurnSticker - 1]];
			}
			else
			{
				if (currentTurnAxis != TurnAxis.None)
				{
					turnAxis[(int)currentTurnAxis].transform.localEulerAngles = Vector3.zero;
					UpdateStickersParents(turnAxis[(int)currentTurnAxis], true);
				}
				UpdateStickers(currentTurnSticker, currentTurnType);
				cubiesMeshRenderers[currentTurnSticker - 1].material = colors[cubeColors[currentTurnSticker - 1]];
				isTurning = false;
			}

		}
	}

	public bool IsSolved()
	{
		for (int i = 0; i < 8; i++)
		{
			int cubeColor = cubeColors[i * 27];
			for (int j = i * 27; j < (i + 1) * 27; j++)
			{
				if (cubeColors[j] != cubeColor)
				{
					return false;
				}
			}
		}
		return true;
	}

	public void ToggleColorShow()
	{
		showingColors = !showingColors;
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 27; j++)
			{
				if (showingColors)
				{
					cubiesMeshRenderers[27 * i + j].material = colors[cubeColors[27 * i + j]];
				}
				else
				{
					cubiesMeshRenderers[27 * i + j].material = colors[8];
				}
			}
		}
	}

	// TODO
	public string Scramble()
	{
		return "";
	}

	public void SetStartState()
	{
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 27; j++)
			{
				cubiesMeshRenderers[27 * i + j].material = colors[i];
				cubeColors[27 * i + j] = i;
			}
		}
	}

	public void Turn(int stickerIndex, TurnType turnType)
	{

		// Gets the degrees to turn based on sticker and turn amount
		if (!isTurning)
		{
			turnTimer = 0f;
			float degrees = 0;
			StickerType stickerType = GetStickerTypeFromIndex(stickerIndex);
			switch (stickerType)
			{
				case StickerType.Edge:
					degrees = 180;
					break;
				case StickerType.Center:
					degrees = 0;
					break;
				case StickerType.Middle:
					switch (turnType)
					{
						case TurnType.Clockwise:
							degrees = 90;
							break;
						case TurnType.CounterClockwise:
							degrees = -90;
							break;
						case TurnType.HalfTurn:
							degrees = 180;
							break;
					}
					break;
				case StickerType.Corner:
					switch (turnType)
					{
						case TurnType.Clockwise:
							degrees = 120;
							break;
						case TurnType.CounterClockwise:
							degrees = -120;
							break;
						case TurnType.HalfTurn:
							degrees = 0;
							break;
					}
					break;
			}

			// Gets the axis to turn around
			TurnAxis stickerTurnAxis = GetAxisFromSticker(stickerIndex);

			// Reverses the direction of turn if opposite positive y-axis
			degrees *= ReverseTurnDirection(stickerIndex) ? -1 : 1;

			if (stickerTurnAxis != TurnAxis.None)
			{
				UpdateStickersParents(turnAxis[(int)stickerTurnAxis], true);
			}

			currentTurnAxis = stickerTurnAxis;
			currentTurnSticker = stickerIndex;
			currentTurnDegrees = degrees;
			currentTurnType = turnType;
			isTurning = true;
		}
	}

	private void SetupHypercubeStructure()
	{

		cubes = new GameObject[8];

		mainSection = new GameObject();
		mainSection.transform.parent = transform;
		mainSection.name = "MainSection";

		cubes[0] = Instantiate(insidePrefab);
		cubes[0].transform.parent = mainSection.transform;
		cubes[0].name = "InsideCube";
		cubes[7] = Instantiate(insidePrefab);
		cubes[7].transform.parent = transform;
		cubes[7].name = "OutsideCube";

		for (int i = 1; i < 7; i++)
		{
			cubes[i] = Instantiate(distortedPrefab);
			cubes[i].transform.parent = mainSection.transform;
			cubes[i].name = sideNames[i] + "Cube";
		}

		for (int i = 0; i < 8; i++)
		{
			Quaternion quaternionRotation = new Quaternion();
			quaternionRotation.eulerAngles = cubesEulerAngles[i];
			cubes[i].transform.localRotation = quaternionRotation;
			cubes[i].transform.localPosition = cubePositions[i];
		}

		float spacing = CubesMeshSettings.spacing;

		for (int i = 1; i <= 3; i++)
		{
			Transform rowInside = cubes[0].transform.Find("Row" + i);
			Transform rowOutside = cubes[7].transform.Find("Row" + i);
			rowInside.localPosition = Vector3.zero;
			rowOutside.localPosition = Vector3.zero;
			for (int j = 1; j <= 3; j++)
			{
				Transform columnInside = rowInside.Find("Column" + j);
				Transform columnOutside = rowOutside.Find("Column" + j);
				columnInside.localPosition = Vector3.zero;
				columnOutside.localPosition = Vector3.zero;
				for (int k = 1; k <= 3; k++)
				{
					Transform pieceInside = columnInside.Find("Cube" + k);
					pieceInside.localPosition = new Vector3((k - 2) * (1 + spacing), (i - 2) * (1 + spacing), (j - 2) * (1 + spacing));
					Transform pieceOutside = columnOutside.Find("Cube" + k);
					pieceOutside.localPosition = new Vector3((k - 2) * (1 + spacing), (i - 2) * (1 + spacing), (j - 2) * (1 + spacing));
				}
			}
		}

	}

	private void AssignCubies()
	{
		cubies = new GameObject[216];
		cubiesMeshRenderers = new MeshRenderer[216];
		cubeColors = new int[216];

		int indexCounter = 0;

		for (int i = 0; i < 8; i++)
		{
			GameObject cube = cubes[i];
			for (int j = 1; j <= 3; j++)
			{
				GameObject layer = cube.transform.Find("Row" + j).gameObject;
				for (int k = 1; k <= 3; k++)
				{
					GameObject row = layer.transform.Find("Column" + k).gameObject;
					for (int l = 1; l <= 3; l++)
					{
						cubies[cubiesMapping[indexCounter]] = row.transform.Find("Cube" + l).gameObject;
						indexCounter++;
					}
				}
			}
		}

		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 27; j++)
			{
				cubiesMeshRenderers[27 * i + j] = cubies[27 * i + j].GetComponent<MeshRenderer>();
				cubiesMeshRenderers[27 * i + j].material = colors[i];
				cubeColors[27 * i + j] = i;
			}
		}

	}

	private StickerType GetStickerTypeFromIndex(int index)
	{
		if (index % 27 == 13)
		{
			return StickerType.Center;
		}
		else if (
			index % 27 == 4 || index % 27 == 12 || index % 27 == 16 ||
			index % 27 == 10 || index % 14 == 22 || index % 22 == 22)
		{
			return StickerType.Middle;
		}
		else if (
			index % 27 == 0 || index % 27 == 6 || index % 27 == 8 || index % 27 == 2 ||
			index % 27 == 18 || index % 27 == 24 || index % 27 == 26 || index % 27 == 20)
		{
			return StickerType.Corner;
		}
		return StickerType.Edge;
	}

	// TODO
	private void UpdateStickersParents(GameObject axis, bool toAxis)
	{

	}

	private void UpdateStickers(int stickerIndex, TurnType turnType)
	{
		int[,] cycles = TurnCycles.GetCycles(stickerIndex, turnType);

		for (int i = 0; i < cycles.GetLength(0); i++)
		{

			// Identity
			if (cycles.GetLength(1) == 0)
			{
				break;
			}

			int endIndex = cycles.GetLength(1) - 1;
			int tempColor = cubeColors[cycles[i, endIndex] - 1];
			for (int j = endIndex; j > 0; j--)
			{
				cubeColors[cycles[i, j] - 1] = cubeColors[cycles[i, j - 1] - 1];
				if (showingColors)
				{
					cubiesMeshRenderers[cycles[i, j] - 1].material = colors[cubeColors[cycles[i, j] - 1]];
				}
			}
			cubeColors[cycles[i, 0] - 1] = tempColor;
			if (showingColors)
			{
				cubiesMeshRenderers[cycles[i, 0] - 1].material = colors[cubeColors[cycles[i, 0] - 1]];
			}
		}
	}

	private TurnAxis GetAxisFromSticker(int stickerIndex)
	{
		switch (stickerIndex)
		{
			case 1:
				return TurnAxis.CornerTopBackRight;
			case 2:
				return TurnAxis.EdgeTopBack;
			case 3:
				return TurnAxis.CornerTopBackLeft;
			case 4:
				return TurnAxis.EdgeTopRight;
			case 5:
				return TurnAxis.FaceTop;
			case 6:
				return TurnAxis.EdgeTopLeft;
			case 7:
				return TurnAxis.CornerTopFrontRight;
			case 8:
				return TurnAxis.EdgeTopFront;
			case 9:
				return TurnAxis.CornerTopFrontLeft;
			case 10:
				return TurnAxis.EdgeFrontLeft;
			case 11:
				return TurnAxis.FaceFront;
			case 12:
				return TurnAxis.EdgeFrontRight;
			case 13:
				return TurnAxis.FaceRight;
			case 15:
				return TurnAxis.FaceRight;
			case 16:
				return TurnAxis.EdgeFrontRight;
			case 17:
				return TurnAxis.FaceFront;
			case 18:
				return TurnAxis.EdgeFrontLeft;
			case 19:
				return TurnAxis.CornerTopFrontLeft;
			case 20:
				return TurnAxis.EdgeTopFront;
			case 21:
				return TurnAxis.CornerTopFrontRight;
			case 22:
				return TurnAxis.EdgeTopLeft;
			case 23:
				return TurnAxis.FaceTop;
			case 24:
				return TurnAxis.EdgeTopRight;
			case 25:
				return TurnAxis.CornerTopBackLeft;
			case 26:
				return TurnAxis.EdgeTopBack;
			case 27:
				return TurnAxis.CornerTopBackRight;
			case 44:
				return TurnAxis.FaceFront;
			case 65:
				return TurnAxis.FaceFront;
			case 86:
				return TurnAxis.FaceTop;
			case 131:
				return TurnAxis.FaceTop;
			case 148:
				return TurnAxis.FaceRight;
			case 177:
				return TurnAxis.FaceRight;
		}
		return TurnAxis.None;
	}

	private bool ReverseTurnDirection(int stickerIndex)
	{
		switch (stickerIndex)
		{
			case 1:
				return true;
			case 2:
				return true;
			case 3:
				return true;
			case 4:
				return true;
			case 5:
				return true;
			case 6:
				return true;
			case 7:
				return true;
			case 8:
				return true;
			case 9:
				return true;
			case 13:
				return true;
			case 16:
				return true;
			case 17:
				return true;
			case 18:
				return true;
			case 65:
				return true;
			case 86:
				return true;
			case 148:
				return true;
		}
		return false;
	}

	private void SetupTurnAxis()
	{
		Transform[] turns = new Transform[13];
		turnAxis = new GameObject[13];
		turnAxisPrefab = Instantiate(turnAxisPrefab);

		turns[0] = turnAxisPrefab.transform.Find("FaceTop");
		turns[1] = turnAxisPrefab.transform.Find("FaceRight");
		turns[2] = turnAxisPrefab.transform.Find("FaceFront");
		turns[3] = turnAxisPrefab.transform.Find("EdgeTopFront");
		turns[4] = turnAxisPrefab.transform.Find("EdgeTopRight");
		turns[5] = turnAxisPrefab.transform.Find("EdgeTopBack");
		turns[6] = turnAxisPrefab.transform.Find("EdgeTopLeft");
		turns[7] = turnAxisPrefab.transform.Find("EdgeFrontRight");
		turns[8] = turnAxisPrefab.transform.Find("EdgeFrontLeft");
		turns[9] = turnAxisPrefab.transform.Find("CornerTopFrontRight");
		turns[10] = turnAxisPrefab.transform.Find("CornerTopFrontLeft");
		turns[11] = turnAxisPrefab.transform.Find("CornerTopBackLeft");
		turns[12] = turnAxisPrefab.transform.Find("CornerTopBackRight");

		for (int i = 0; i < 13; i++)
		{
			turnAxis[i] = turns[i].Find("RotationParent").gameObject;
		}
	}

	public void Test(int i)
	{
		Debug.Log(i);
		cubiesMeshRenderers[i - 1].material = colors[0];
	}
}



[CustomEditor(typeof(HypercubeTurner))]
public class HypercubeTurnerEditor : Editor
{
	private HypercubeTurner hypercube;
	private int stickerIndexMove = 15;
	private HypercubeTurner.TurnType turnType = HypercubeTurner.TurnType.Clockwise;

	private void OnEnable()
	{
		hypercube = (HypercubeTurner)target;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		stickerIndexMove = EditorGUILayout.IntField("Sticker Turn Index", stickerIndexMove);
		turnType = (HypercubeTurner.TurnType)EditorGUILayout.EnumPopup("Turn Type", turnType);
		stickerIndexMove = Mathf.Min(Mathf.Max(1, stickerIndexMove), 216);
		if (GUILayout.Button("Move"))
		{
			hypercube.Turn(stickerIndexMove, turnType);
		}
		if (GUILayout.Button("Toggle Blind Mode"))
		{
			hypercube.ToggleColorShow();
		}
		if (GUILayout.Button("Set Start State"))
		{
			hypercube.SetStartState();
		}
		if (GUILayout.Button("Scramble"))
		{
			hypercube.Scramble();
		}
		if (GUILayout.Button("Test"))
		{
			hypercube.Test(stickerIndexMove);
		}
	}
}