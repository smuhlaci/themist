using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterSorter : MonoBehaviour
{

	[SerializeField] private float YPosition;

	[FormerlySerializedAs("XPositionss")] [SerializeField]
	private List<float> XPositions;

	private void OnEnable()
	{
		EventManager.StartListening("New Cycle", OnNewCycle);
	}

	private void OnDisable()
	{
		EventManager.StopListening("New Cycle", OnNewCycle);
	}

	private void OnNewCycle()
	{
		var chars = transform.GetComponentsInChildren<Character>().ToList();

		for (var i = 0; i < chars.Count; i++)
		{
			if (XPositions[i] > 0)
			{
				chars[i]._lookDirection = LookDirection.Left;
				chars[i].UpdatePose();
			}

			chars[i].transform.position = new Vector3(XPositions[i], YPosition, 0);
		}
	}
}