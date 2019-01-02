using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
	public string name;
	
	public AudioClip clip;

	[Range(0F,1F)]
	public float volume;
	[Range(0.1F, 3.0F)]
	public float pitch;

	public bool loop;

	[HideInInspector]
	public AudioSource source;
}
