using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Animation of the turning of the Mabz logo
/// </summary>
public class LogoAnimation : MonoBehaviour
{

	/// <summary>
	/// Prefab of a 1/9 piece of the logo
	/// </summary>
	public GameObject logoPiecePrefab;

	/// <summary>
	/// Array of 9 sprites that make up the logo
	/// Goes from top left across to top right,
	/// down and so on
	/// </summary>
	public Sprite[] spritePieces;

	/// <summary>
	/// Transforms of the 9 pieces plus the 1 buffer piece
	/// </summary>
	private Transform[] logoPieces;

	/// <summary>
	/// Sprite renderers of the logo pieces
	/// </summary>
	private SpriteRenderer[] logoPieceSpriteRenderers;

	/// <summary>
	/// Parent of the piece movement
	/// </summary>
	private GameObject moveParent;

	/// <summary>
	/// Moves applied to the logo pieces
	/// </summary>
	private MoveType[] moves =
	{
		MoveType.TopRight,
		MoveType.RightUp,
		MoveType.TopRight,
		MoveType.RightUp,
		MoveType.LeftUp,
		MoveType.MiddleLeft,
		MoveType.CenterUp,
		MoveType.TopRight,
		MoveType.BottomLeft,
		MoveType.RightUp,
		MoveType.CenterDown
	};

	/// <summary>
	/// Ordered list of move types by row/column and move direction
	/// </summary>
	private enum MoveType
	{
		TopRight,
		TopLeft,
		MiddleRight,
		MiddleLeft,
		BottomRight,
		BottomLeft,
		LeftUp,
		LeftDown,
		CenterUp,
		CenterDown,
		RightUp,
		RightDown
	};

	/// <summary>
	/// The current move iteration
	/// </summary>
	private int moveIndex;

	/// <summary>
	/// Time for piece to move
	/// </summary>
	private static float moveTime = .4f;

	/// <summary>
	/// Time in between moves
	/// </summary>
	private static float delayTime = .1f;

	/// <summary>
	/// Time waiting after all moves
	/// </summary>
	private static float waitTime = 2f;

	/// <summary>
	/// Timer for the animation
	/// </summary>
	private float timer;

	/// <summary>
	/// Whether all move animations are complete
	/// </summary>
	private bool isMoveAnimation;

	/// <summary>
	/// Scale of the piece dimensions compared to whole image
	/// </summary>
	private static float pieceScale = 1f / 3f;

	/// <summary>
	/// The piece index to set the buffer piece sprite
	/// </summary>
	private static int[] moveTypeBufferIndex =
	{
		2, 0, 5, 3, 8, 6, 0, 6, 1, 7, 2, 8
	};

	/// <summary>
	/// Position of the buffer piece before move in order of move type
	/// </summary>
	private static Vector3[] moveTypeBufferPosition =
	{
		new Vector3(-2, 1) * pieceScale,
		new Vector3(2, 1) * pieceScale,
		new Vector3(-2, 0) * pieceScale,
		new Vector3(2, 0) * pieceScale,
		new Vector3(-2, -1) * pieceScale,
		new Vector3(2, -1) * pieceScale,
		new Vector3(-1, -2) * pieceScale,
		new Vector3(-1, 2) * pieceScale,
		new Vector3(0, -2) * pieceScale,
		new Vector3(0, 2) * pieceScale,
		new Vector3(1, -2) * pieceScale,
		new Vector3(1, 2) * pieceScale

	};

	/// <summary>
	/// Indicies of the moving pieces by move type
	/// </summary>
	private static int[,] moveTypeParentPieces = new int[,]
	{
		{ 0, 1, 2, 9 },
		{ 0, 1, 2, 9 },
		{ 3, 4, 5, 9 },
		{ 3, 4, 5, 9 },
		{ 6, 7, 8, 9 },
		{ 6, 7, 8, 9 },
		{ 0, 3, 6, 9 },
		{ 0, 3, 6, 9 },
		{ 1, 4, 7, 9 },
		{ 1, 4, 7, 9 },
		{ 2, 5, 8, 9 },
		{ 2, 5, 8, 9 }
	};

	/// <summary>
	/// Base positions of each of the 9 pieces
	/// </summary>
	private static Vector3[] basePiecePositions =
	{
		new Vector3(-pieceScale, pieceScale),
		new Vector3(0, pieceScale),
		new Vector3(pieceScale, pieceScale),
		new Vector3(-pieceScale, 0),
		new Vector3(0, 0),
		new Vector3(pieceScale, 0),
		new Vector3(-pieceScale, -pieceScale),
		new Vector3(0, -pieceScale),
		new Vector3(pieceScale, -pieceScale)
	};

	/// <summary>
	/// Cycles of pieces in order of move type
	/// </summary>
	private static int[,] moveCycles =
	{
		{ 0, 1, 2 },
		{ 0, 2, 1 },
		{ 3, 4, 5 },
		{ 3, 5, 4 },
		{ 6, 7, 8 },
		{ 6, 8, 7 },
		{ 0, 6, 3 },
		{ 0, 3, 6 },
		{ 1, 7, 4 },
		{ 1, 4, 7 },
		{ 2, 8, 5 },
		{ 2, 5, 8 },
	};

	/// <summary>
	/// These map the pieces starting position so that by the
	/// end of the scramble they solve to the logo
	/// </summary>
	private static int[] startPiecePositions =
	{
		1, 8, 6, 4, 3, 5, 2, 7, 0, 9
	};

	/// <summary>
	/// Initializes pieces positions and data
	/// </summary>
	private void Start()
	{

		// Logo pieces transfroms and sprite renderers
		logoPieces = new Transform[10];
		logoPieceSpriteRenderers = new SpriteRenderer[10];

		// Iterates over each piece and intializes data
		for (int i = 0; i < 10; i++)
		{

			// Creates the logo piece game object and initializes data
			GameObject logoPieceGameObject = Instantiate(logoPiecePrefab);
			logoPieces[i] = logoPieceGameObject.transform;
			logoPieces[i].parent = transform;
			logoPieceSpriteRenderers[i] = logoPieceGameObject.GetComponent<SpriteRenderer>();
			logoPieces[i].gameObject.name = "Piece" + i;
			logoPieces[i].localScale = new Vector3(pieceScale, pieceScale);

			// The buffer piece is set to a unique position and sprite
			if (i == 9)
			{
				SetBufferPiece();
			}

			// Regular pieces have their positions and pieces set
			else
			{
				logoPieceSpriteRenderers[i].sprite = spritePieces[startPiecePositions[i]];
				logoPieces[i].localPosition = basePiecePositions[i];
			}
		}

		// Move parent game object and data initialized
		moveParent = new GameObject();
		moveParent.name = "MoveParent";
		moveParent.transform.parent = transform;
		moveParent.transform.localPosition = Vector3.zero;

		// Pieces about to move are attached to move parent
		AttachToParent();

		// Movement animation is started
		isMoveAnimation = true;
	}

	/// <summary>
	/// Updates the animation, then waits, then transitions scene
	/// </summary>
	private void Update()
	{

		// Update the move pieces if in progression of animation
		if (isMoveAnimation)
		{
			UpdateMoveAnimation();
		}

		// Wait for elapsed time, then start game with start scene
		else
		{
			timer += Time.deltaTime;
			if (timer >= waitTime)
			{
				MultiSceneManager.StartGame();
			}
		}
	}

	/// <summary>
	/// Updates the movement of the pieces
	/// </summary>
	private void UpdateMoveAnimation()
	{

		// Update time change
		timer += Time.deltaTime;

		// If within move time
		if (timer <= moveTime)
		{

			// Progress through move from 0 to 1
			float progress = timer / moveTime;

			// If moving right, set move parent position to the right
			if (moves[moveIndex] == MoveType.TopRight ||
				moves[moveIndex] == MoveType.MiddleRight ||
				moves[moveIndex] == MoveType.BottomRight)
			{
				moveParent.transform.localPosition = new Vector3(progress * pieceScale, 0);
			}

			// If moving left, set move parent position to the left
			else if (moves[moveIndex] == MoveType.TopLeft ||
				moves[moveIndex] == MoveType.MiddleLeft ||
				moves[moveIndex] == MoveType.BottomLeft)
			{
				moveParent.transform.localPosition = new Vector3(-progress * pieceScale, 0);
			}

			// If moving up, set move parent position up
			else if (moves[moveIndex] == MoveType.LeftUp ||
				moves[moveIndex] == MoveType.CenterUp ||
				moves[moveIndex] == MoveType.RightUp)
			{
				moveParent.transform.localPosition = new Vector3(0, progress * pieceScale);
			}

			// If moving down, set move parent position down
			else if (moves[moveIndex] == MoveType.LeftDown ||
				moves[moveIndex] == MoveType.CenterDown ||
				moves[moveIndex] == MoveType.RightDown)
			{
				moveParent.transform.localPosition = new Vector3(0, -progress * pieceScale);
			}
		}

		// If within move delay time, reset pieces to base positions
		else if (timer <= moveTime + delayTime)
		{
			moveParent.transform.localPosition = moveParent.transform.localPosition.normalized * pieceScale;
		}

		// After move and delay time have elapsed, iterate to next move or finish animation
		else
		{

			// Update the pieces in the move cycle
			UpdatePiecesCycle();

			// Iterate to next move
			moveIndex++;

			// Reset pieces to base parent and position
			DetachFromParent();

			// If all moves complete finish move animation
			if (moveIndex >= moves.Length)
			{
				isMoveAnimation = false;
			}

			// Update buffer piece and pieces to attach to move parent
			else
			{
				SetBufferPiece();
				AttachToParent();
			}

			// Reset timer for next move
			timer -= moveTime + delayTime;
		}
	}

	/// <summary>
	/// Updates the pieces based on move cycle
	/// </summary>
	private void UpdatePiecesCycle()
	{

		// Gets the current cycle from the list of cycles
		int[] cycle = {
			moveCycles[(int)moves[moveIndex], 0],
			moveCycles[(int)moves[moveIndex], 1],
			moveCycles[(int)moves[moveIndex], 2]
		};

		// Cycles the sprites of the 3 moved pieces
		Sprite startSprite = logoPieceSpriteRenderers[cycle[0]].sprite;
		logoPieceSpriteRenderers[cycle[0]].sprite = logoPieceSpriteRenderers[cycle[2]].sprite;
		logoPieceSpriteRenderers[cycle[2]].sprite = logoPieceSpriteRenderers[cycle[1]].sprite;
		logoPieceSpriteRenderers[cycle[1]].sprite = startSprite;
	}

	/// <summary>
	/// Attaches the moving pieces to the move parent
	/// </summary>
	private void AttachToParent()
	{
		for (int i = 0; i < 4; i++)
		{
			logoPieces[moveTypeParentPieces[(int)moves[moveIndex], i]].parent = moveParent.transform;
		}
	}

	/// <summary>
	/// Detaches the moving pieces from the parent back to main logo parent
	/// </summary>
	private void DetachFromParent()
	{
		Transform[] detatchPieces = moveParent.GetComponentsInChildren<Transform>();
		moveParent.transform.localPosition = Vector3.zero;
		foreach (Transform piece in detatchPieces)
		{
			piece.parent = transform;
		}

	}

	/// <summary>
	/// Sets the position and sprite of the buffer piece
	/// </summary>
	private void SetBufferPiece()
	{

		// Gets the current type of move
		int moveType = (int)moves[moveIndex];

		// Sets the buffer position
		logoPieces[9].localPosition = moveTypeBufferPosition[moveType];

		// Sets the buffer sprite
		int bufferIndex = moveTypeBufferIndex[moveType];
		logoPieceSpriteRenderers[9].sprite = logoPieceSpriteRenderers[bufferIndex].sprite;
	}

}