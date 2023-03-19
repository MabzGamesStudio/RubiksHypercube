using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Handles the movement, turning, and coloring of the hypercube
/// Image for calculating the sticker indexes:
/// https://docs.google.com/drawings/d/1h2QFtHRlkxURK6rBMxNY1jmVToYkR3r181nzOgWkgTo/edit?usp=sharing
/// The creation of the cycles were generated by the Mathematica code located in
/// the "Mathematica" folder
/// IMPORTANT:
/// The hypersticker indexes start at 1 because Mathematica starts at index 1
/// </summary>
public class HypercubeTurner : MonoBehaviour
{

	/// <summary>
	/// Prefab of the center hyperface
	/// </summary>
	public GameObject insidePrefab;

	/// <summary>
	/// Prefab of the distored hyperfaces adjacent to the center hyperface
	/// </summary>
	public GameObject distortedPrefab;

	/// <summary>
	/// Prefab of the axis of rotation in turning the hypercube
	/// </summary>
	public GameObject turnAxisPrefab;

	/// <summary>
	/// Colors of the stickers in order of the hyperfaces: white, pink, purple,
	/// green, blue, red, orange, yellow
	/// </summary>
	public Material[] colors;

	/// <summary>
	/// Colors of the stickers when selected in order of the hyperfaces:
	/// white, pink, purple, green, blue, red, orange, yellow
	/// </summary>
	public Material[] colorsSelected;

	/// <summary>
	/// Script to activate the confetti on solve
	/// </summary>
	public ConfettiScript confetti;

	/// <summary>
	/// Text script to display the sticker moves in the scramble
	/// </summary>
	public ScrambleText scrambleText;

	/// <summary>
	/// Text script to display the sticker moves in the solve
	/// </summary>
	public SolveText solveText;

	/// <summary>
	/// Script of the button that toggles whether the sticker colors are showing
	/// </summary>
	public CubeButton showColorsButton;

	/// <summary>
	/// Script of the solve timer
	/// </summary>
	public CubeTimer timer;

	/// <summary>
	/// Axis of rotation around the line formed from the center to the noted
	/// sticker piece with the following axis indexes:
	/// 0: FaceTop, 1: FaceRight, 2: FaceFront, 3: EdgeTopFront, 4: EdgeTopRight,
	/// 5: EdgeTopBack, 6: EdgeTopLeft, 7: EdgeFrontRight, 8: EdgeFrontLeft,
	/// 9: CornerTopFrontRight,  10: CornerTopFrontLeft, 11: CornerTopBackLeft,
	/// 12: CornerTopBackRight
	/// </summary>
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

	/// <summary>
	/// Sticker type in reference to the hyperface
	/// </summary>
	private enum StickerType
	{
		Center,
		Middle,
		Edge,
		Corner
	}

	/// <summary>
	/// Type of rotation in direction and amount of rotation
	/// </summary>
	public enum TurnType
	{
		Clockwise,
		HalfTurn,
		CounterClockwise,
		None
	}

	/// <summary>
	/// Perspective of view of the cube
	/// </summary>
	public enum Perspective
	{
		Top,
		Normal,
		Bottom
	};

	/// <summary>
	/// Whether the sticker colors of the cube are showing
	/// </summary>
	public bool showingColors = true;

	/// <summary>
	/// Array of hyperfaces
	/// </summary>
	private GameObject[] cubes;

	/// <summary>
	/// Array of sticker hypercubies
	/// </summary>
	private GameObject[] cubies;

	/// <summary>
	/// Mesh renderers of the hypercubies
	/// </summary>
	private MeshRenderer[] cubiesMeshRenderers;

	/// <summary>
	/// Transform parents of the stickers moved in turn animation
	/// </summary>
	private Transform[] hypercubeTurnStickerParents;

	/// <summary>
	/// Transforms of the stickers moved in turn animation
	/// </summary>
	private Transform[] hypercubeTurnStickerChildren;

	/// <summary>
	/// Array of gameobjects of the turn axis for the turn animation
	/// </summary>
	private GameObject[] turnAxis;

	/// <summary>
	/// Colors of the stickers of the hypercube
	/// </summary>
	private int[] cubeColors;

	/// <summary>
	/// Game object parent of the central 7 hyperfaces
	/// </summary>
	private GameObject mainSection;

	/// <summary>
	/// Whether the cube is currently in the turn animation
	/// </summary>
	private bool isTurning;

	/// <summary>
	/// Timer progression of the turn animation
	/// </summary>
	private float turnTimer = 0f;

	/// <summary>
	/// Current turn axis being rotated around
	/// </summary>
	private TurnAxis currentTurnAxis;

	/// <summary>
	/// Current sticker move
	/// </summary>
	private int currentTurnSticker;

	/// <summary>
	/// Current rotation degrees around turn axis
	/// </summary>
	private float currentTurnDegrees;

	/// <summary>
	/// Current type of turn being performed
	/// </summary>
	private TurnType currentTurnType;

	/// <summary>
	/// Time elapsed over turn animation
	/// </summary>
	private static float turnTime = .5f;

	/// <summary>
	/// Time elapsed over sticker select animation
	/// </summary>
	private static float switchTime = .2f;

	/// <summary>
	/// Distance between the hyperfaces
	/// </summary>
	private static float hyperfaceDistance = 8f;

	/// <summary>
	/// Whether to use the turn animation
	/// </summary>
	private bool showTurn = true;

	/// <summary>
	/// Number of sticker turns in scramble of hypercube
	/// </summary>
	private static int scrambleMoves = 100;

	/// <summary>
	/// Scaling factor of the size of the hypercube
	/// </summary>
	private static float hypercubeScale = 0.04f;

	/// <summary>
	/// Position of the hypercube in space
	/// </summary>
	private static Vector3 hypercubePosition = new Vector3(0f, 1.1f, 1.2f);

	/// <summary>
	/// Offset of the perspective shift of the hypercube
	/// </summary>
	private static float perspectiveOffset = 1f;

	/// <summary>
	/// Whether the confetti solve animation is currently in progress
	/// </summary>
	private bool solveAnimation;

	/// <summary>
	/// Time elapsed during the solve
	/// </summary>
	private float solveAnimationTime;

	/// <summary>
	/// Whether the cube has been scrambled
	/// </summary>
	private bool cubeScrambled;

	/// <summary>
	/// Names of the hyperfaces in order
	/// </summary>
	private static string[] sideNames = new string[]
	{
		"Inside", "Front", "Back", "Top", "Bottom", "Right", "Left", "Outside"
	};

	/// <summary>
	/// Euler angles of rotation of each hyperface in order
	/// </summary>
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

	/// <summary>
	/// Positions of the hyperfaces in space relative to center hyperface
	/// </summary>
	private Vector3[] cubePositions = new Vector3[]
	{
		new Vector3(0, 0, 0),
		new Vector3(0, 0, -hyperfaceDistance),
		new Vector3(0, 0, hyperfaceDistance),
		new Vector3(0, hyperfaceDistance, 0),
		new Vector3(0, -hyperfaceDistance, 0),
		new Vector3(hyperfaceDistance, 0, 0),
		new Vector3(-hyperfaceDistance, 0, 0),
		new Vector3(3 * hyperfaceDistance, 0, 0)
	};

	/// <summary>
	/// This maps the order of the sticker based on its creation to the sticker
	/// index used universally
	/// Sticker creation is different due to the rotations of the hyperfaces
	/// which change the relative sticker index positions
	/// </summary>
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

	/// <summary>
	/// Initializes hypercube, hyperfaces, stickers, turn axis, and button
	/// that displays sticker colors to showing
	/// </summary>
	void Start()
	{
		SetupHypercubeStructure();
		AssignCubies();
		SetupTurnAxis();
		showColorsButton.SetButtonToggle(true);
	}

	/// <summary>
	/// Updates the data and animations of the turn or solve
	/// </summary>
	private void Update()
	{
		UpdateSolveAnimation();
		UpdateHypercubeTurn();
	}

	/// <summary>
	/// Updates the animation of the solve animation
	/// </summary>
	private void UpdateSolveAnimation()
	{

		// If solve animation in progress, elapse time
		if (solveAnimation)
		{
			solveAnimationTime -= Time.deltaTime;

			// If time has elapsed, the animation has finished
			if (solveAnimationTime <= 0)
			{
				solveAnimation = false;
			}
		}
	}

	/// <summary>
	/// Updates the animatino and data of the hypercube turn
	/// </summary>
	private void UpdateHypercubeTurn()
	{

		// Update turn animation if not in solve animation
		if (isTurning && !solveAnimation)
		{

			// Percent progress through turn animation from 0 to 1
			float percent;

			// Update time progression
			turnTimer += Time.deltaTime;

			// Animation progress depends on whether the turn animation is
			// in progress or the simple color switch is in progress
			if (showTurn && currentTurnAxis != TurnAxis.None)
			{
				percent = turnTimer / turnTime;
			}
			else
			{
				percent = turnTimer / switchTime;
			}

			// If the animation is currently in progress
			if (percent <= 1f)
			{

				// If displaying turn animation update the turn rotation
				if (currentTurnAxis != TurnAxis.None && showTurn)
				{
					turnAxis[(int)currentTurnAxis].transform.localEulerAngles = new Vector3(0, currentTurnDegrees * percent);
				}

				// Highlight turn sticker color, if not showing colors make sure
				// the highlight color is grayed out
				if (showingColors)
				{
					cubiesMeshRenderers[currentTurnSticker - 1].material = colorsSelected[cubeColors[currentTurnSticker - 1]];
				}
				else
				{
					cubiesMeshRenderers[currentTurnSticker - 1].material = colorsSelected[8];
				}

				// Highlight stickers on face if middle sticker or entire hyperface
				// if center sticker selected
				CheckColorCentralizerTurn(true);
				CheckColorFaceTurn(true);
			}

			// If the animation time has elapsed
			else
			{

				// Reset stickers rotation, position, and transform parents
				if (currentTurnAxis != TurnAxis.None)
				{
					turnAxis[(int)currentTurnAxis].transform.localEulerAngles = Vector3.zero;
					UpdateStickersParents(turnAxis[(int)currentTurnAxis], true);
				}

				// Un-highlight turn sticker color, if not showing colors make sure
				// the highlight color is grayed out
				if (showingColors)
				{
					cubiesMeshRenderers[currentTurnSticker - 1].material = colors[cubeColors[currentTurnSticker - 1]];
				}
				else
				{
					cubiesMeshRenderers[currentTurnSticker - 1].material = colors[8];
				}

				// Un-highlight stickers on face if middle sticker or entire hyperface
				// if center sticker selected
				CheckColorCentralizerTurn(false);
				CheckColorFaceTurn(false);

				// Update all sticker colors based on the sticker turn
				UpdateStickers(currentTurnSticker, currentTurnType);

				// Check if cube is solved
				if (IsSolved())
				{
					// Starts the solve animation
					timer.StopTimer();
					confetti.PartyTime();

					// Cube has been solved and is no longer scrambled
					cubeScrambled = false;

					// Show the solve and scramble moves
					scrambleText.ShowText();
					solveText.ShowMoves();

					// Cube stickers are set to show
					showColorsButton.SetButtonToggle(true);

					// Show sticker colors and activate solve animation
					solveAnimation = true;
					solveAnimationTime = 5.5f;
					if (!showingColors)
					{
						ToggleColorShow();
					}
				}

				// Cube is no longer turning
				isTurning = false;
			}
		}
	}

	/// <summary>
	/// Changes the position of the hypercube with given a perspective change
	/// </summary>
	/// <param name="perspective">Direction of perspective</param>
	public void ChangePerspective(Perspective perspective)
	{
		switch (perspective)
		{

			// Move hypercube down when looking at top perspective
			case Perspective.Top:
				transform.parent.localPosition =
					new Vector3(
						hypercubePosition.x,
						hypercubePosition.y - perspectiveOffset,
						hypercubePosition.z);
				break;

			// Center hypercube when looking at front perspective
			case Perspective.Normal:
				transform.parent.localPosition =
					new Vector3(
						hypercubePosition.x,
						hypercubePosition.y,
						hypercubePosition.z);
				break;

			// Move hypercube up when looking at bottom perspective
			case Perspective.Bottom:
				transform.parent.localPosition =
					new Vector3(
						hypercubePosition.x,
						hypercubePosition.y + perspectiveOffset,
						hypercubePosition.z);
				break;
		}
	}

	/// <summary>
	/// Checks whether the hypercube is solved
	/// </summary>
	/// <returns></returns>
	public bool IsSolved()
	{

		// If cube has not been scrambled the cube is not considered solved
		if (!cubeScrambled)
		{
			return false;
		}

		// If any sticker colors differ on the same hyperface, it is not solved
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

		// If all 8 hyperfaces have the same sticker colors it is solved
		return true;
	}

	/// <summary>
	/// Toggles whether to show the sticker colors or gray them out
	/// </summary>
	public void ToggleColorShow()
	{

		// Update show color variable
		showingColors = !showingColors;

		// Set each sticker material to show color or gray out
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

	/// <summary>
	/// Toggles whether to show the turn animation and returns new value
	/// </summary>
	/// <returns></returns>
	public bool ToggleShowTurn()
	{
		showTurn = !showTurn;
		return showTurn;
	}

	/// <summary>
	/// Scrambles the cube and returns scramble moves
	/// Scramble moves example:
	/// "100 56 23 ... 178 67 "
	/// </summary>
	/// <returns>Space separated list of sticker moves</returns>
	public string Scramble()
	{

		// Starts scramble at solve state
		SetStartState();

		// Records each random sticker move
		string scrambleText = "";
		for (int i = 0; i < scrambleMoves; i++)
		{

			// Updates the scramble list and activates hypercube turn
			int randomNumber = UnityEngine.Random.Range(1, 216);
			scrambleText += randomNumber.ToString() + " ";
			UpdateStickers(randomNumber, TurnType.Clockwise);
		}

		// Cube has been scramblee and scrmable list is returned
		cubeScrambled = true;
		return scrambleText;
	}

	/// <summary>
	/// Sets the cube to the solved state
	/// </summary>
	public void SetStartState()
	{

		// Sets each sticker data to solve state and updates material to color
		// material or grayed out material
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 27; j++)
			{
				if (showingColors)
				{
					cubiesMeshRenderers[27 * i + j].material = colors[i];
				}
				else
				{
					cubiesMeshRenderers[27 * i + j].material = colors[8];
				}
				cubeColors[27 * i + j] = i;
			}
		}

		// Cube is no longer scrambled
		cubeScrambled = false;
	}

	/// <summary>
	/// Activates turn of the hypercube
	/// Does not activate while in solve animation or already in turn animation
	/// </summary>
	/// <param name="stickerIndex">Sticker that has been selected</param>
	/// <param name="turnType">Type of turn applied to sticker</param>
	public void Turn(int stickerIndex, TurnType turnType)
	{

		// Does not activate while in turn animation
		if (!isTurning)
		{

			// Resets turn timer
			turnTimer = 0f;

			// Amount of degrees to rotate around
			float degrees = 0;

			// Type of sticker selected
			StickerType stickerType = GetStickerTypeFromIndex(stickerIndex);

			// Gets the turn degrees based on sticker type and type of turn
			switch (stickerType)
			{

				// Edge turn applies 180 degree turn
				case StickerType.Edge:
					degrees = 180;
					break;

				// Center turn does not rotate cube
				case StickerType.Center:
					degrees = 0;
					break;

				// Middle sticker turns clockwise 90 degrees, counterclockwise 90
				// degrees, or half turn 180 degrees
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

				// Corner sticker turns clockwise or counterclockwise 120 degrees
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

			// Reverses the direction of turn if opposite of positive y-axis
			// of the turn axis
			degrees *= ReverseTurnDirection(stickerIndex) ? -1 : 1;

			// If the sticker applies turn animation, then update stickers
			// parents that rotate around turn axis
			if (stickerTurnAxis != TurnAxis.None && showTurn)
			{
				UpdateStickersParents(turnAxis[(int)stickerTurnAxis], true);
			}

			// Updates the data for the current turn axis, sticker, degrees,
			// turn type, and turn begins
			currentTurnAxis = stickerTurnAxis;
			currentTurnSticker = stickerIndex;
			currentTurnDegrees = degrees;
			currentTurnType = turnType;
			isTurning = true;

			// These record the turns to the solve moves list, the moves are
			// recorded 1, 2, or 3 times because turns indicate only clockwise
			// moves

			// Add sinlge move if edge sticker or center
			if (stickerType == StickerType.Center || stickerType == StickerType.Edge)
			{
				solveText.AddMove(stickerIndex);
			}

			// In corner move repeat 1 for clockwise and 2 for counterclockwise
			else if (stickerType == StickerType.Corner)
			{
				if (turnType == TurnType.Clockwise)
				{
					solveText.AddMove(stickerIndex);
				}
				else if (turnType == TurnType.CounterClockwise)
				{
					solveText.AddMove(stickerIndex);
					solveText.AddMove(stickerIndex);
				}
			}

			// In middle move repeat 1 for clockwise, 2 for half turn,
			// and 3 for counterclockwise turn
			else if (stickerType == StickerType.Middle)
			{
				if (turnType == TurnType.Clockwise)
				{
					solveText.AddMove(stickerIndex);
				}
				else if (turnType == TurnType.HalfTurn)
				{
					solveText.AddMove(stickerIndex);
					solveText.AddMove(stickerIndex);
				}
				else if (turnType == TurnType.CounterClockwise)
				{
					solveText.AddMove(stickerIndex);
					solveText.AddMove(stickerIndex);
					solveText.AddMove(stickerIndex);
				}
			}
		}
	}

	/// <summary>
	/// Initializes the structure of the hypercube hyperfaces
	/// </summary>
	private void SetupHypercubeStructure()
	{

		// Hyperfaces game objects
		cubes = new GameObject[8];

		// Main section parent created
		mainSection = new GameObject();
		mainSection.transform.parent = transform;
		mainSection.name = "MainSection";

		// Inside and outside hyperfaces created
		cubes[0] = Instantiate(insidePrefab);
		cubes[0].transform.parent = mainSection.transform;
		cubes[0].name = "InsideCube";
		cubes[7] = Instantiate(insidePrefab);
		cubes[7].transform.parent = transform;
		cubes[7].name = "OutsideCube";

		// Hyperfaces adjacent to inside hyperface are created
		for (int i = 1; i < 7; i++)
		{
			cubes[i] = Instantiate(distortedPrefab);
			cubes[i].transform.parent = mainSection.transform;
			cubes[i].name = sideNames[i] + "Cube";
		}

		// Rotations are applied to hyperfaces adjacent to inside hyperface
		for (int i = 0; i < 8; i++)
		{
			Quaternion quaternionRotation = new Quaternion();
			quaternionRotation.eulerAngles = cubesEulerAngles[i];
			cubes[i].transform.localRotation = quaternionRotation;
			cubes[i].transform.localPosition = cubePositions[i];
		}

		// Spacing between the stickers
		float spacing = CubesMeshSettings.spacing;


		// Iterates over each row, then column, then layer to place and name
		// the stickers of the inside and outside hyperfaces
		// Row plane parallel to front face, column plane parallel to right face,
		// and layer plane parallel to top face
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

		// Scales and positions the entire hypercube
		transform.localScale = new Vector3(hypercubeScale, hypercubeScale, hypercubeScale);
		transform.parent.localPosition = hypercubePosition;
	}

	/// <summary>
	/// Assigns the stickers data and materials
	/// </summary>
	private void AssignCubies()
	{

		// Array of sticker game objects
		cubies = new GameObject[216];

		// Mesh renderers of the stickers
		cubiesMeshRenderers = new MeshRenderer[216];

		// Data for the sticker color
		cubeColors = new int[216];

		// Counter for the sticker index
		int indexCounter = 0;

		// Stickers and their parents for the pieces that are rotated in the
		// turn animations
		hypercubeTurnStickerParents = new Transform[81];
		hypercubeTurnStickerChildren = new Transform[81];

		// Counter for the sticker index of the turn animation stickers
		int transformParentsIndex = 0;

		// List of sticker indexes of the stickers that move in the turn animations
		// in order of index
		List<int> transformParentStickerIndicies = new List<int>()
		{

			// White hyperface stickers
			1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27,
			
			// Pink hyperface stickers
			34, 35, 36, 43, 44, 45, 52, 53, 54,
			
			// Purple hyperface stickers
			55, 56, 57, 64, 65, 66, 73, 74, 75,

			// Green hyperface stickers
			82, 83, 84, 85, 86, 87, 88, 89, 90,
			
			// Blue hyperface stickers
			127, 128, 129, 130, 131, 132, 133, 134, 135,
			
			// Red hyperface stickers
			136, 139, 142, 145, 148, 151, 154, 157, 160,
			
			// Orange hyperface stickers
			165, 168, 171, 174, 177, 180, 183, 186, 189
		};

		// Iterates over each sticker and initializes its color and index
		// Iterates over each hyperface, then layer, row, and column
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

						// Initializes its sticker data and name to its index
						cubies[cubiesMapping[indexCounter]] = row.transform.Find("Cube" + l).gameObject;
						cubies[cubiesMapping[indexCounter]].name = "Sticker" + (1 + cubiesMapping[indexCounter]);
						indexCounter++;

						// If the sticker is part of the list within the turn animation
						// then initialize its transform and parent data
						if (transformParentStickerIndicies.Contains(1 + cubiesMapping[indexCounter - 1]))
						{
							hypercubeTurnStickerParents[transformParentsIndex] = row.transform;
							hypercubeTurnStickerChildren[transformParentsIndex] = cubies[cubiesMapping[indexCounter - 1]].transform;
							transformParentsIndex++;
						}
					}
				}
			}
		}

		// Initializes the mesh renderers and color data of the stickers in index order
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

	/// <summary>
	/// Gets the type of sticker by the index range (1 - 216)
	/// </summary>
	/// <param name="index">Index of the sticker</param>
	/// <returns>Type of sticker</returns>
	private StickerType GetStickerTypeFromIndex(int index)
	{

		// Center sticker index
		if (index % 27 == 14)
		{
			return StickerType.Center;
		}

		// Middle sticker indexes
		else if (
			index % 27 == 5 || index % 27 == 11 || index % 27 == 13 ||
			index % 27 == 15 || index % 27 == 17 || index % 27 == 23)
		{
			return StickerType.Middle;
		}

		// Corner sticker indexes
		else if (
			index % 27 == 1 || index % 27 == 3 || index % 27 == 7 || index % 27 == 9 ||
			index % 27 == 19 || index % 27 == 21 || index % 27 == 25 || index % 27 == 0)
		{
			return StickerType.Corner;
		}

		// Otherwise sticker index is edge
		return StickerType.Edge;
	}

	/// <summary>
	/// Updates the parents of the turn animation stickers
	/// </summary>
	/// <param name="axis">Game object of the turn axis</param>
	/// <param name="toAxis">
	/// Whether to apply sticker parent to axis or original parent
	/// </param>
	private void UpdateStickersParents(GameObject axis, bool toAxis)
	{

		// Iterates over each sticker in list of turn animation stickers
		for (int i = 0; i < hypercubeTurnStickerParents.Length; i++)
		{

			// Sets parent to axis
			if (toAxis)
			{
				hypercubeTurnStickerChildren[i].parent = axis.transform;
			}

			// Sets parent to original parent
			else
			{
				hypercubeTurnStickerChildren[i].parent = hypercubeTurnStickerParents[i];
			}
		}
	}

	/// <summary>
	/// Updates the sticker colors based on the sticker turn
	/// </summary>
	/// <param name="stickerIndex">Index of the sticker turn selected</param>
	/// <param name="turnType">Type of turn applied to the sticker</param>
	private void UpdateStickers(int stickerIndex, TurnType turnType)
	{

		// Gets the cycle notation based on the sticker turn data
		// Example: {{1, 2}, {3, 4, 5}} swaps stickers with index 1 and 2
		// and color on index 3 goes to index 4, color index 4 goes to 5, and
		// color on index 5 goes to index 3
		int[,] cycles = TurnCycles.GetCycles(stickerIndex, turnType);

		// Iterates over each of the cycles
		for (int i = 0; i < cycles.GetLength(0); i++)
		{

			// Identity does not rotate any stickers
			if (cycles.GetLength(1) == 0)
			{
				break;
			}

			// Value of the last index in the cycle
			int endIndex = cycles.GetLength(1) - 1;

			// Saves the color of the sticker on the end index
			int tempColor = cubeColors[cycles[i, endIndex] - 1];

			// Sets each sticker color to the color of the sticker at the index
			// of the cycle before it, starting at the end of the cycle
			for (int j = endIndex; j > 0; j--)
			{

				// Updates sticker color value
				cubeColors[cycles[i, j] - 1] = cubeColors[cycles[i, j - 1] - 1];

				// Updates sticker visual color if showing colors
				if (showingColors)
				{
					cubiesMeshRenderers[cycles[i, j] - 1].material = colors[cubeColors[cycles[i, j] - 1]];
				}
			}

			// Updates the color of the cycle start index to the saved end index color
			// Updates sticker color value
			cubeColors[cycles[i, 0] - 1] = tempColor;

			// Updates sticker visual color if showing colors
			if (showingColors)
			{
				cubiesMeshRenderers[cycles[i, 0] - 1].material = colors[cubeColors[cycles[i, 0] - 1]];
			}
		}
	}

	/// <summary>
	/// Checks if middle sticker selected, which visually highlights whole face
	/// </summary>
	/// <param name="highlight">Whether to highlight or un-highlight face</param>
	private void CheckColorFaceTurn(bool highlight)
	{

		// Relative piece to inside stickers
		int relativePiece = currentTurnSticker % 27;

		// Index of the hyperface selected
		int cubeFace = (currentTurnSticker - 1) / 27;

		// Sticker indexes of each middle piece relative to inside hyperface
		List<int> faceIndicies = new List<int>() { 5, 11, 13, 15, 17, 23 };

		// Sticker indexes within each face of relative pieces of inside hyperface
		int[,] facePieces = new int[,]
		{
			{ 1, 2, 3, 4, 5, 6, 7, 8, 9},
			{ 1, 2, 3, 10, 11, 12, 19, 20, 21},
			{ 1, 4, 7, 10, 13, 16, 19, 22, 25},
			{ 3, 6, 9, 12, 15, 18, 21, 24, 27},
			{ 7, 8, 9, 16, 17, 18, 25, 26, 27},
			{ 19, 20, 21, 22, 23, 24, 25, 26, 27},
		};

		// If the middle sticker has been selected
		if (faceIndicies.Contains(relativePiece))
		{

			// Iterate over each of the 9 stickers on the face
			for (int i = 0; i < 9; i++)
			{

				// Index of sticker on face
				int highlightSticker = 27 * cubeFace + facePieces[faceIndicies.IndexOf(relativePiece), i] - 1;

				// Update sticker highlight
				UpdateStickerHighlight(highlightSticker, highlight);
			}
		}
	}

	/// <summary>
	/// Checks if center sticker selected, which visually highlights whole hyperface
	/// </summary>
	/// <param name="highlight">Whether to highlight or un-highlight hyperface</param>
	private void CheckColorCentralizerTurn(bool highlight)
	{

		// Relative piece to inside stickers
		int relativePiece = currentTurnSticker % 27;

		// Index of the hyperface selected
		int cubeFace = (currentTurnSticker - 1) / 27;

		// If the center sticker has been selected
		if (relativePiece == 14)
		{

			// Iterate over each of the 27 stickers on the hyperface
			for (int i = 0; i < 27; i++)
			{

				// Index of sticker on face
				int highlightSticker = 27 * cubeFace + i;

				// Update sticker highlight
				UpdateStickerHighlight(highlightSticker, highlight);
			}
		}
	}

	/// <summary>
	/// Updates the highlight of the given sticker adaptive to
	/// whether the sticker is grayed out
	/// </summary>
	/// <param name="stickerIndex">Index of the sticker</param>
	/// <param name="highlight">Whether to highlight the sticker</param>
	private void UpdateStickerHighlight(int stickerIndex, bool highlight)
	{

		// Highlight the sticker to color
		if (highlight && showingColors)
		{
			cubiesMeshRenderers[stickerIndex].material = colorsSelected[cubeColors[stickerIndex]];
		}

		// Highlight the sticker to grayed out highlight
		else if (highlight && !showingColors)
		{
			cubiesMeshRenderers[stickerIndex].material = colorsSelected[8];
		}

		// Un-highlight the sticker to color
		else if (!highlight && showingColors)
		{
			cubiesMeshRenderers[stickerIndex].material = colors[cubeColors[stickerIndex]];
		}

		// Un-highlight the sticker to grayed out highlight
		else if (!highlight && !showingColors)
		{
			cubiesMeshRenderers[stickerIndex].material = colors[8];
		}
	}

	/// <summary>
	/// Gets the axis of rotation based on the given sticker index
	/// </summary>
	/// <param name="stickerIndex">Sticker turn index</param>
	/// <returns></returns>
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
			case 0:
				return TurnAxis.CornerTopBackRight;
		}

		// If sticker is not within inside hypercube, no turn animation or axis
		return TurnAxis.None;
	}

	/// <summary>
	/// Returns whether the given sticker points in the negative direction of
	/// the turn axis
	/// See TurnAxis enum to see sticker pointing in positive direction
	/// </summary>
	/// <param name="stickerIndex">Index of the sticker</param>
	/// <returns>
	/// Whether the sticker points in the negative direction of the turn axis
	/// </returns>
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
		}
		return false;
	}

	/// <summary>
	/// Initializes the structure and data of the turn axis
	/// </summary>
	private void SetupTurnAxis()
	{

		// Turn axis transforms
		Transform[] turns = new Transform[13];

		// Turn axis game objects initialized
		turnAxis = new GameObject[13];
		turnAxisPrefab = Instantiate(turnAxisPrefab);

		// Turn axis parent is under the main section
		turnAxisPrefab.transform.parent = transform.Find("MainSection");
		turnAxisPrefab.transform.localPosition = Vector3.zero;
		turnAxisPrefab.name = "TurnAxis";

		// Initialize the transform data of the turns
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

		// Initialize the transforms of the turn axis to the turns parents
		for (int i = 0; i < 13; i++)
		{
			turnAxis[i] = turns[i].Find("RotationParent").gameObject;
		}
	}

}