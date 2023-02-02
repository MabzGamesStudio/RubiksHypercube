using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for the execution of the confetti
/// </summary>
public class ConfettiScript : MonoBehaviour
{

	/// <summary>
	/// Particle system of the confetti
	/// </summary>
	private ParticleSystem ps;

	/// <summary>
	/// Initialize the particle system
	/// </summary>
	void Start()
	{
		ps = GetComponent<ParticleSystem>();
	}

	/// <summary>
	/// Restart the confetti animation, then play it
	/// </summary>
	public void PartyTime()
	{

		// Settings for the particle system
		ParticleSystem.MainModule main = ps.main;

		// Start the animation right away when activated
		main.startDelay = 0;
		ps.Stop();
		ps.Clear();
		ps.Play();
	}

}
