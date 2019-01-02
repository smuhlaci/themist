using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stars : MonoBehaviour
{

	[SerializeField] private float starsUpdateTime = 1.5F;
	private float starsUpdateCooldown = 0.0F;
	private List<SpriteRenderer> stars = new List<SpriteRenderer>();
	
	private void Start () {
		for (int i = 0; i < transform.childCount; i++)
		{
			var star = transform.GetChild(i).GetComponent<SpriteRenderer>();
			if (star != null)
				stars.Add(star);
			else Debug.LogErrorFormat("SpriteRenderer couldn't found in Star({0})", i);
		}
	}
	
	private void Update ()
	{
		if (starsUpdateCooldown <= 0)
		{
			foreach (var star in stars)
			{
				star.color = Colors.Random(4, 0);
			}

			starsUpdateCooldown = starsUpdateTime;
		}
		else starsUpdateCooldown -= Time.deltaTime;
	}
}
