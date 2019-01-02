using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Character", menuName = "The Mist/Character")]
public class CharacterData : ScriptableObject
{
	public string Name;

	public Resource Ability;
}
