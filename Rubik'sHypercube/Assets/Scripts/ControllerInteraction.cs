using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ControllerInteraction : MonoBehaviour
{

	private InputDevice controllerDevice;

	public RayDetector rayDetector;

	public Transform controllerPosition;

	public HypercubeTurner hypercubeTurner;

	private Transform mainSectionTransform;
	private Transform outsideSectionTransform;

	public CubeButton[] buttons;

	public bool rightController;

	private bool triggerReset = true;
	private bool gripReset = true;
	private bool primaryButtonReset = true;
	private bool secondaryButtonReset = true;
	private bool rollerClickReset = true;
	private bool stickReset = true;

	private float controllerXStartPosition;
	private static float sectionBaseXRotation;
	private float spinSensitivity = 100f;

	// Start is called before the first frame update
	void Start()
	{
		List<InputDevice> devices = new List<InputDevice>();

		InputDeviceCharacteristics characteristic;
		if (rightController)
		{
			characteristic = InputDeviceCharacteristics.Right;
		}
		else
		{
			characteristic = InputDeviceCharacteristics.Left;
		}

		InputDevices.GetDevicesWithCharacteristics(characteristic, devices);
		controllerDevice = devices[0];
	}

	// Update is called once per frame
	void Update()
	{
		controllerDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);

		if (triggerValue < .05f)
		{
			triggerReset = true;
		}

		if (triggerReset && triggerValue > .1f)
		{
			triggerReset = false;
			int stickerClicked = rayDetector.GetRaySticker();
			if (stickerClicked > 0)
			{
				int moveHyperfaceSticker = (stickerClicked - 1) / 27 * 27 + 14;
				hypercubeTurner.Turn(moveHyperfaceSticker, HypercubeTurner.TurnType.None);
			}

			int buttonClicked = rayDetector.GetRayButton();
			if (buttonClicked > 0)
			{
				switch (buttonClicked)
				{
					case 1:
						hypercubeTurner.SetStartState();
						buttons[0].HighlightButton();
						break;
					case 2:
						hypercubeTurner.Scramble();
						buttons[1].HighlightButton();
						break;
					case 3:
						hypercubeTurner.ToggleColorShow();
						buttons[2].HighlightButton();
						break;
					case 4:
						hypercubeTurner.ToggleShowTurn();
						buttons[3].HighlightButton();
						break;
				}
			}
		}

		controllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonPressed);

		if (!primaryButtonPressed)
		{
			primaryButtonReset = true;
		}

		if (primaryButtonPressed && primaryButtonReset)
		{
			primaryButtonReset = false;
			int stickerClicked = rayDetector.GetRaySticker();
			if (stickerClicked > 0)
			{
				hypercubeTurner.Turn(stickerClicked, HypercubeTurner.TurnType.Clockwise);
			}
		}

		controllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonPressed);

		if (!secondaryButtonPressed)
		{
			secondaryButtonReset = true;
		}

		if (secondaryButtonPressed && secondaryButtonReset)
		{
			secondaryButtonReset = false;
			int stickerClicked = rayDetector.GetRaySticker();
			if (stickerClicked > 0)
			{
				hypercubeTurner.Turn(stickerClicked, HypercubeTurner.TurnType.CounterClockwise);
			}
		}

		controllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool rollerClicked);

		if (!rollerClicked)
		{
			rollerClickReset = true;
		}

		if (rollerClicked && rollerClickReset)
		{
			rollerClickReset = false;
			int stickerClicked = rayDetector.GetRaySticker();
			if (stickerClicked > 0)
			{
				hypercubeTurner.Turn(stickerClicked, HypercubeTurner.TurnType.HalfTurn);
			}
		}

		controllerDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue);


		if (gripValue < .05f)
		{
			if (!gripReset)
			{
				sectionBaseXRotation += controllerXStartPosition - controllerPosition.position.x;
			}
			gripReset = true;
		}

		if (gripReset && gripValue > .1f)
		{
			gripReset = false;
			if (mainSectionTransform == null)
			{
				mainSectionTransform = hypercubeTurner.gameObject.transform.Find("MainSection");
			}
			if (outsideSectionTransform == null)
			{
				outsideSectionTransform = hypercubeTurner.gameObject.transform.Find("OutsideCube");
			}
			controllerXStartPosition = controllerPosition.position.x;
		}

		if (gripValue > .1f)
		{
			float newPositionX = controllerPosition.position.x;
			Quaternion sectionRotation = Quaternion.identity;

			sectionRotation.eulerAngles = spinSensitivity *
				new Vector3(0, sectionBaseXRotation + controllerXStartPosition - newPositionX, 0);
			mainSectionTransform.localRotation = sectionRotation;

			sectionRotation.eulerAngles = -sectionRotation.eulerAngles;
			outsideSectionTransform.localRotation = sectionRotation;

		}

		controllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 stickValue);

		if (-0.1f < stickValue.y && stickValue.y < 0.1f)
		{
			if (!stickReset)
			{
				hypercubeTurner.ChangePerspective(HypercubeTurner.Perspective.Normal);
			}
			stickReset = true;
		}

		if (stickValue.y > 0.5f && stickReset)
		{
			stickReset = false;
			hypercubeTurner.ChangePerspective(HypercubeTurner.Perspective.Top);
		}
		else if (stickValue.y < -0.5f && stickReset)
		{
			stickReset = false;
			hypercubeTurner.ChangePerspective(HypercubeTurner.Perspective.Bottom);
		}


	}
}
