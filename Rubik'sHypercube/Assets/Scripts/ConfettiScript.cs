using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfettiScript : MonoBehaviour
{

	private ParticleSystem ps;

	// Start is called before the first frame update
	void Start()
	{
		ps = GetComponent<ParticleSystem>();
	}

	public void PartyTime()
	{
		ParticleSystem.MainModule main = ps.main;
		main.startDelay = 0;

		ps.Stop();
		ps.Clear();

		ps.Play();
	}
}
