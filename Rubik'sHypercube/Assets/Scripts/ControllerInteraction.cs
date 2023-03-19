using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Handles controller input and activates their resulting actions
/// </summary>
public class ControllerInteraction : MonoBehaviour
{

	/// <summary>
	/// Script to display text of sticker moves in scramble
	/// </summary>
	public ScrambleText scrambleText;

	/// <summary>
	/// Script to detect the collision of the control ray with game objects
	/// </summary>
	public RayDetector rayDetector;

	/// <summary>
	/// Transform of the controller position in 3D space
	/// </summary>
	public Transform controllerPosition;

	/// <summary>
	/// Script for the movement of the hypercube turning and display
	/// </summary>
	public HypercubeTurner hypercubeTurner;

	/// <summary>
	/// Script for the movement of the hypercube turning and display
	/// </summary>
	public CubeTurner cubeTurner;

	/// <summary>
	/// Whether this script is using the hypercube turner instead of
	/// the cube turner
	/// </summary>
	public bool usingHypercubeTurner;

	/// <summary>
	/// Scripts of the buttons: solve cube, scramble cube, toggle color show,
	/// toggle show turns, toggle cube type, and credits scene
	/// </summary>
	public CubeButton[] buttons;

	/// <summary>
	/// Script for the solve time display
	/// </summary>
	public CubeTimer cubeTimer;

	/// <summary>
	/// Script to display text of sticker moves in solve
	/// </summary>
	public SolveText solveText;

	/// <summary>
	/// Whether this controller is the right controller
	/// </summary>
	public bool rightController;

	/// <summary>
	/// Input device of the controller
	/// </summary>
	private InputDevice controllerDevice;

	/// <summary>
	/// Transform parent of the main hyperfaces of the hypercube
	/// </summary>
	private Transform mainSectionTransform;

	/// <summary>
	/// Transform parent of the outside hyperface of the hypercube
	/// </summary>
	private Transform outsideSectionTransform;

	/// <summary>
	/// Whether the cube has been turned in the solve yet
	/// </summary>
	private bool cubeMoved;

	/// <summary>
	/// Whether the trigger has been reset since the first frame of its trigger
	/// </summary>
	private bool triggerReset = true;

	/// <summary>
	/// Whether the grip has been reset since the first frame of its trigger
	/// </summary>
	private bool gripReset = true;

	/// <summary>
	/// Whether the primary button has been reset since the first frame of its trigger
	/// </summary>
	private bool primaryButtonReset = true;

	/// <summary>
	/// Whether the secondary button has been reset since the first frame of its trigger
	/// </summary>
	private bool secondaryButtonReset = true;

	/// <summary>
	/// Whether the stick click has been reset since the first frame of its trigger
	/// </summary>
	private bool rollerClickReset = true;

	/// <summary>
	/// Whether the stick move has been reset since the first frame of its trigger
	/// </summary>
	private bool stickReset = true;

	/// <summary>
	/// Start x position of the controller before being turned by grip
	/// </summary>
	private float controllerXStartPosition;

	/// <summary>
	/// Base x rotation of the hypercube before geing turned by grip
	/// </summary>
	private static float sectionBaseXRotation;

	/// <summary>
	/// Sensitivity of the rotation of the hypercube
	/// </summary>
	private static float spinSensitivity = 150f;

	/// <summary>
	/// Initializes the controller device to the left or right
	/// controller respectively
	/// </summary>
	void Start()
	{

		// Devices list and characteristic initialized
		List<InputDevice> devices = new List<InputDevice>();
		InputDeviceCharacteristics characteristic;

		// Whether this script refers to the left or right controller
		if (rightController)
		{
			characteristic = InputDeviceCharacteristics.Right;
		}
		else
		{
			characteristic = InputDeviceCharacteristics.Left;
		}

		// Gets the controller device from the devices list
		InputDevices.GetDevicesWithCharacteristics(characteristic, devices);
		controllerDevice = devices[0];
	}

	/// <summary>
	/// Checks each of the controller inputs for response
	/// </summary>
	void Update()
	{
		CheckTrigger();
		CheckPrimaryButton();
		CheckSecondaryButton();
		CheckStickClick();
		CheckStickPushed();
		CheckGrip();
	}

	/// <summary>
	/// Checks whether the trigger button has been triggered
	/// </summary>
	private void CheckTrigger()
	{

		// Gets the trigger input value
		controllerDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);

		// If the value is under 5% it has been released and is reset
		if (triggerValue < .05f)
		{
			triggerReset = true;
		}

		// If the value exceeds 10% and is reset the trigger has been pressed
		if (triggerReset && triggerValue > .1f)
		{

			// Trigger is no longer reset
			triggerReset = false;

			// Sends sticker to hypercube turner if clicked
			int stickerClicked = rayDetector.GetRaySticker();

			// Controller aimed at sticker if value is above 0
			if (stickerClicked > 0)
			{
				if (usingHypercubeTurner)
				{

					// Gets the center sticker of the hyperface
					int moveHyperfaceSticker = (stickerClicked - 1) / 27 * 27 + 14;
					hypercubeTurner.Turn(moveHyperfaceSticker, HypercubeTurner.TurnType.None);
				}
				else
				{

					// Turns the center sticker of the hyperface
					cubeTurner.Turn(stickerClicked, CubeTurner.TurnType.FaceCentralizer);
				}
			}

			// Sends button response if any buttons clicked
			int buttonClicked = rayDetector.GetRayButton();

			// Controller aimed at button if value is above 0
			if (buttonClicked > 0)
			{
				switch (buttonClicked)
				{

					// Solve hypercube button clicked:
					// Shows button click animation
					// Hides scramble and solve text, and hides timer
					case 1:
						if (usingHypercubeTurner)
						{
							hypercubeTurner.SetStartState();
						}
						else
						{
							cubeTurner.SetStartState();
						}
						buttons[0].HighlightButton();
						cubeTimer.HideTimer();
						scrambleText.HideText();
						solveText.HideMoves();
						solveText.ClearMoves();
						break;

					// Scramble hypercube button clicked
					// Hides scramble and solve text, and starts timer
					// Shows button click animation
					// Cube moved variable resets
					case 2:
						if (usingHypercubeTurner)
						{
							scrambleText.SetText(hypercubeTurner.Scramble());
						}
						else
						{
							scrambleText.SetText(cubeTurner.Scramble());
						}
						scrambleText.HideText();
						buttons[1].HighlightButton();
						cubeTimer.StartTimer();
						solveText.HideMoves();
						solveText.ClearMoves();
						cubeMoved = false;
						break;

					// Toggle show sticker colors button clicked
					// Shows button click animation
					// Toggles show color variable
					// Changes blind mode variable and button text
					case 3:
						if (usingHypercubeTurner)
						{
							hypercubeTurner.ToggleColorShow();
						}
						else
						{
							cubeTurner.ToggleColorShow();
						}
						buttons[2].HighlightButton();
						bool showingColors;
						if (usingHypercubeTurner)
						{
							showingColors = hypercubeTurner.showingColors;
						}
						else
						{
							showingColors = cubeTurner.showingColors;
						}
						if (showingColors)
						{
							cubeTimer.DisableBlindMode();
						}
						else if (!cubeMoved)
						{
							cubeTimer.EnableBlindMode();
						}
						break;

					// Toggle show turns button clicked
					// Toggles show turns variable
					case 4:
						if (usingHypercubeTurner)
						{
							hypercubeTurner.ToggleShowTurn();
						}
						else
						{
							cubeTurner.ToggleShowTurn();
						}
						buttons[3].HighlightButton();
						break;

					// Switch from 3D to 4D cube scene
					case 5:
						System.Threading.Thread.Sleep(100);
						MultiSceneManager.SwitchCubeScene();
						break;

					// Switch to credits scene
					case 6:
						MultiSceneManager.CreditsScene();
						break;

					// Switch back from credits scene
					case 7:
						MultiSceneManager.BackFromCredits();
						break;

					// Switch from 4D to 3D cube scene
					case 8:
						System.Threading.Thread.Sleep(100);
						MultiSceneManager.SwitchCubeScene();
						break;
				}
			}
		}
	}

	/// <summary>
	/// Checks whether the primary button has been triggered
	/// </summary>
	private void CheckPrimaryButton()
	{

		// Gets the primary button input value
		controllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonPressed);

		// If the button released, the button is reset
		if (!primaryButtonPressed)
		{
			primaryButtonReset = true;
		}

		// If the button is pressed and is reset the primary button activates
		if (primaryButtonPressed && primaryButtonReset)
		{

			// Primary button is no longer reset
			primaryButtonReset = false;

			// Sends sticker clicked to hypercube turner if clicked
			int stickerClicked = rayDetector.GetRaySticker();

			// Controller aimed at sticker if value is above 0
			if (stickerClicked > 0)
			{
				if (usingHypercubeTurner)
				{
					hypercubeTurner.Turn(stickerClicked, HypercubeTurner.TurnType.Clockwise);
				}
				else
				{
					cubeTurner.Turn(stickerClicked, CubeTurner.TurnType.Clockwise);
				}
				cubeMoved = true;
			}
		}
	}

	/// <summary>
	/// Checks whether the secondary button has been triggered
	/// </summary>
	private void CheckSecondaryButton()
	{

		// Gets the secondary button input value
		controllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonPressed);

		// If the button released, the button is reset
		if (!secondaryButtonPressed)
		{
			secondaryButtonReset = true;
		}

		// If the button is pressed and is reset the secondary button activates
		if (secondaryButtonPressed && secondaryButtonReset)
		{

			// Secondary button is no longer reset
			secondaryButtonReset = false;

			// Sends sticker clicked to hypercube turner if clicked
			int stickerClicked = rayDetector.GetRaySticker();

			// Controller aimed at sticker if value is above 0
			if (stickerClicked > 0)
			{
				if (usingHypercubeTurner)
				{
					hypercubeTurner.Turn(stickerClicked, HypercubeTurner.TurnType.CounterClockwise);
				}
				else
				{
					cubeTurner.Turn(stickerClicked, CubeTurner.TurnType.CounterClockwise);
				}
				cubeMoved = true;
			}
		}
	}

	/// <summary>
	/// Checks whether the stick button has been triggered
	/// </summary>
	private void CheckStickClick()
	{

		// Gets the stick button input value
		controllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool rollerClicked);

		// If the stick button released, the stick button is reset
		if (!rollerClicked)
		{
			rollerClickReset = true;
		}

		// If the stick button is pressed and is reset the stick button activates
		if (rollerClicked && rollerClickReset)
		{

			// Stick button is no longer reset
			rollerClickReset = false;

			// Sends sticker clicked to hypercube turner if clicked
			int stickerClicked = rayDetector.GetRaySticker();

			// Controller aimed at sticker if value is above 0
			if (stickerClicked > 0)
			{
				if (usingHypercubeTurner)
				{
					hypercubeTurner.Turn(stickerClicked, HypercubeTurner.TurnType.HalfTurn);
				}
				else
				{
					cubeTurner.Turn(stickerClicked, CubeTurner.TurnType.HalfTurn);
				}
				cubeMoved = true;
			}
		}
	}

	/// <summary>
	/// Checks whether the stick has been moved
	/// </summary>
	private void CheckStickPushed()
	{

		// Gets the stick position input value
		controllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 stickValue);

		// If the stick button is within the 10% released threshold, the stick button
		// is reset and the hypercube perspective goes to the front view
		if (-0.1f < stickValue.y && stickValue.y < 0.1f)
		{
			if (!stickReset)
			{
				if (usingHypercubeTurner)
				{
					hypercubeTurner.ChangePerspective(HypercubeTurner.Perspective.Normal);
				}
				else
				{
					cubeTurner.ChangePerspective(CubeTurner.Perspective.Normal);
				}
			}
			stickReset = true;
		}

		// If the stick button is within the 50% moved up threshold, the stick button
		// is activated and the hypercube perspective goes to the top view
		if (stickValue.y > 0.5f && stickReset)
		{
			stickReset = false;
			if (usingHypercubeTurner)
			{
				hypercubeTurner.ChangePerspective(HypercubeTurner.Perspective.Top);
			}
			else
			{
				cubeTurner.ChangePerspective(CubeTurner.Perspective.Top);
			}
		}

		// If the stick button is within the 50% moved down threshold, the stick button
		// is activated and the hypercube perspective goes to the bottom view
		else if (stickValue.y < -0.5f && stickReset)
		{
			stickReset = false;
			if (usingHypercubeTurner)
			{
				hypercubeTurner.ChangePerspective(HypercubeTurner.Perspective.Bottom);
			}
			else
			{
				cubeTurner.ChangePerspective(CubeTurner.Perspective.Bottom);
			}
		}
	}

	/// <summary>
	/// Checks whether the grip button has been triggered
	/// </summary>
	private void CheckGrip()
	{

		// Gets the grip button input value
		controllerDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue);

		// If the grip button is within the 5% released threshold, the grip button
		// is reset and the base rotation is reset to the current rotation
		if (gripValue < .05f)
		{
			if (!gripReset)
			{
				sectionBaseXRotation += controllerXStartPosition - controllerPosition.position.x;
			}
			gripReset = true;
		}

		// If the grip button is within the 10% pressed threshold, the grip button
		// is activated and the base rotation is 
		if (gripReset && gripValue > .1f)
		{

			// Grip button is no longer reset
			gripReset = false;

			// Main section and outside section transforms are initialized
			if (usingHypercubeTurner)
			{
				if (mainSectionTransform == null)
				{
					mainSectionTransform = hypercubeTurner.gameObject.transform.Find("MainSection");
				}
				if (outsideSectionTransform == null)
				{
					outsideSectionTransform = hypercubeTurner.gameObject.transform.Find("OutsideCube");
				}
			}
			else
			{
				if (mainSectionTransform == null)
				{
					mainSectionTransform = cubeTurner.gameObject.transform.Find("MainSection");
				}
			}


			// The controller x start position is initialized to the current controller
			// position in space
			controllerXStartPosition = controllerPosition.position.x;
		}

		// If the grip button is being held
		if (gripValue > .1f)
		{

			// The new x position of the controller in space is saved
			float newPositionX = controllerPosition.position.x;

			// The x new rotation is calculated by the base rotation plus th
			// change in controller x position, scaled by the spin sensitivity
			Quaternion sectionRotation = Quaternion.identity;
			sectionRotation.eulerAngles = spinSensitivity *
				new Vector3(
					0,
					sectionBaseXRotation + controllerXStartPosition - newPositionX,
					0);

			// Main section is rotated and outside section is rotated in the
			// opposite direction
			mainSectionTransform.localRotation = sectionRotation;
			if (usingHypercubeTurner)
			{
				sectionRotation.eulerAngles = -sectionRotation.eulerAngles;
				outsideSectionTransform.localRotation = sectionRotation;
			}
		}
	}

}
