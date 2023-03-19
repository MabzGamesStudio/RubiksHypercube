using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the multiple scenes in the game
/// </summary>
public class MultiSceneManager : MonoBehaviour
{

	/// <summary>
	/// Scene index for the logo scene
	/// </summary>
	private static int logoScene = 0;

	/// <summary>
	/// Scene index for the cube scene
	/// </summary>
	private static int cubeScene = 1;

	/// <summary>
	/// Scene index for the hypercube scene
	/// </summary>
	private static int hypercubeScene = 2;

	/// <summary>
	/// Scene index for the credits scene
	/// </summary>
	private static int creditsScene = 3;

	/// <summary>
	/// Index of the last cube scene type when selecting credits
	/// </summary>
	public static int lastCubeScene;

	/// <summary>
	/// The current scene
	/// </summary>
	public static int currentScene = logoScene;

	/// <summary>
	/// Swaps the cube and hypercube scenes
	/// </summary>
	public static void SwitchCubeScene()
	{
		if (currentScene == cubeScene)
		{
			lastCubeScene = hypercubeScene;
			currentScene = hypercubeScene;
			SceneManager.LoadScene(hypercubeScene);
		}
		else
		{
			lastCubeScene = cubeScene;
			currentScene = cubeScene;
			SceneManager.LoadScene(cubeScene);
		}
	}

	/// <summary>
	/// Sets the scene to the credits scene
	/// </summary>
	public static void CreditsScene()
	{
		currentScene = creditsScene;
		SceneManager.LoadScene(creditsScene);
	}

	/// <summary>
	/// Sets the scene previous from the credits scene
	/// </summary>
	public static void BackFromCredits()
	{
		currentScene = lastCubeScene;
		SceneManager.LoadScene(lastCubeScene);
	}

	/// <summary>
	/// Starts the game at the logo scene
	/// </summary>
	public static void StartGame()
	{
		currentScene = cubeScene;
		lastCubeScene = cubeScene;
		SceneManager.LoadScene(cubeScene);
	}

}