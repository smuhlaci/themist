using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
	[SerializeField] private Sprite sun;
	[SerializeField] private Sprite moon;

	public static bool ReadyToNextDay;
	public static bool IsMorning;
	private SpriteRenderer _spriteRenderer;
	private static Animator _animator;
	private static bool autoCycle;
	
	private void Start()
	{
		IsMorning = true;
		_spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
		_animator = GetComponent<Animator>();
		ReadyToNextDay = true;
	}

	public static void NextCycle()
	{
		if(!autoCycle)
		if (!ReadyToNextDay) return;

		ReadyToNextDay = false;
		autoCycle = false;
		
		_animator.SetTrigger("DayCycle");
		EventManager.TriggerEvent("New Cycle");
		GameManager.Instance.NewCycle();
	}

	public void setReadyToNextDay()
	{
		ReadyToNextDay = true;
		if(autoCycle) NextCycle();
	}

	public static void NextCycleWhenReady()
	{
		autoCycle = true;
	}
	
	public void OnNewCycle()
	{
		IsMorning = !IsMorning;
		_spriteRenderer.sprite = IsMorning ? sun : moon;
	}
}
