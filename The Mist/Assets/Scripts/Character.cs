using System;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Character : MonoBehaviour
{
	[Header("Character Information")] 
	public CharacterData Data;
	private string Name
	{
		get
		{
			return Data != null ? Data.Name : "Sercan Altun";
		}
	}
	private Resource Ability
	{
		get
		{
			return Data != null ? Data.Ability : Resource.Food;
		}
	}
	
	[SerializeField] private Mission currentMission;
	public bool MissionIsNull
	{
		get { return currentMission == null; }
	}

	[Header("Dialog Box UI")] 
	[SerializeField] private GameObject CTNDialogCanvas;
	[SerializeField] private RectTransform IMGDialogBoxBackground;	
	[SerializeField] private Text TXTTitle;
	[SerializeField] private Text TXTDescription;
	[SerializeField] private Text TXTFoodValue;
	[SerializeField] private Text TXTWaterValue;
	[SerializeField] private Text TXTFaithValue;
	[SerializeField] private Text TXTOrderValue;
	[SerializeField] private Text TXTDayTime;
	[SerializeField] private GameObject PNLEmpty;	
	[HideInInspector] public bool showDialogBox;

	[Header("Info Box UI")]
	[SerializeField] private Text TXTName;
	[SerializeField] private Image IMGSkill;
	
	[Header("Mission UI")]
	[SerializeField] private Text TXTRemainingDays;	

	[Header("Other")] 
	public LookDirection _lookDirection;
	
	[HideInInspector] public int remainingDay;
	
	private float animationSpeed;
	private SpriteRenderer _spriteRenderer;
	private Animator _animator;
	private bool doNotDecreaseRemainingTimeOnThisCycle;

	public void OnNewCycle()
	{
		if (Sun.IsMorning && !doNotDecreaseRemainingTimeOnThisCycle)
		{
			if (remainingDay > 0) remainingDay--;
		}
		else if(!Sun.IsMorning)
		{
			GameManager.Instance.UpdateResource(Resource.Food, GetDailyConsumeModifier());
			GameManager.Instance.UpdateResource(Resource.Faith, GetDailyConsumeModifier());
			GameManager.Instance.UpdateResource(Resource.Order, GetDailyConsumeModifier());
		}
		
		if(doNotDecreaseRemainingTimeOnThisCycle) doNotDecreaseRemainingTimeOnThisCycle = false;
		
		if (MissionIsNull && remainingDay == 0)
		{
			SetMission(GameManager.Instance.RandomMission());
		}
		else if (!MissionIsNull && remainingDay == 0)
		{
			TakeRewards();
		}
		
		UpdateRemainingDays();
		
		AudioManager.Instance.Play("Click2");
	}

	private void TakeRewards()
	{
		GameManager.Instance.UpdateResource(Resource.Food, currentMission.Food * RewardModifier());
		GameManager.Instance.UpdateResource(Resource.Water, currentMission.Water * RewardModifier());
		GameManager.Instance.UpdateResource(Resource.Faith, currentMission.Faith * RewardModifier());
		GameManager.Instance.UpdateResource(Resource.Order, currentMission.Order * RewardModifier());
		
		SetMission(GameManager.Instance.RandomMission());
		
		_spriteRenderer.color = Colors.Random(0,3);
	}

	public void GiveAnswerToMission(bool answer)
	{
		if (!Sun.ReadyToNextDay) return;
		
		if (answer)
		{
			remainingDay = currentMission.Duration;
			_spriteRenderer.color = Colors.Grey4;
			UpdateRemainingDays();
			doNotDecreaseRemainingTimeOnThisCycle = true;
		}
		else
		{
			GameManager.Instance.UpdateResource(Resource.Order,-5);
			currentMission = null;
			remainingDay = 0;
		}

		UpdateDialogBoxView(false);
		Sun.NextCycle();
		GameManager.decideCount++;
	}
	
	public void UpdateDialogBoxView(bool show)
	{
		TXTName.text = Name;
		IMGSkill.sprite = GameManager.Instance.GetResourceSprite(Ability);
		PNLEmpty.SetActive(remainingDay > 0 || currentMission == null);
		
		showDialogBox = show;
		CTNDialogCanvas.SetActive(showDialogBox);
	}

	public void UpdateRemainingDays()
	{
		TXTRemainingDays.transform.parent.gameObject.SetActive(remainingDay > 0);
		TXTRemainingDays.text = string.Format("{0} Days", remainingDay);
	}
	
	private void Awake()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_animator = GetComponent<Animator>();
		_spriteRenderer.color = Colors.Random(0,3);
	}
	
	public void UpdatePose()
	{
		animationSpeed = Random.Range(1.0F, 1.5F);
		_animator.speed = animationSpeed;
		
		if (_lookDirection.Equals(LookDirection.Left))
		{
			IMGDialogBoxBackground.localScale = new Vector3(-1,1,1);
			CTNDialogCanvas.transform.localPosition = new Vector3(-1.55F, 2.9F, 0);
			_spriteRenderer.flipX = true;
		}
		else
		{
			IMGDialogBoxBackground.localScale = Vector3.one;
			CTNDialogCanvas.transform.localPosition = new Vector3(1.55F, 2.9F, 0);
			_spriteRenderer.flipX = false;
		}
	}

	public void SetMission(Mission mission)
	{
		currentMission = mission;
		TXTTitle.text = currentMission.Title;
		TXTDescription.text = currentMission.Description;
		remainingDay = -1;
		
		TXTDayTime.text = string.Format("{0} Days", currentMission.Duration);
		
		var foodValue = currentMission.Food;
		TXTFoodValue.text = foodValue.ToString();
		TXTFoodValue.transform.parent.gameObject.SetActive(foodValue != 0);
		
		var waterValue = currentMission.Water;
		TXTWaterValue.text = waterValue.ToString();
		TXTWaterValue.transform.parent.gameObject.SetActive(waterValue != 0);
		
		var faithValue = currentMission.Faith;
		TXTFaithValue.text = faithValue.ToString();
		TXTFaithValue.transform.parent.gameObject.SetActive(faithValue != 0);
		
		var orderValue = currentMission.Order;
		TXTOrderValue.text = orderValue.ToString();
		TXTOrderValue.transform.parent.gameObject.SetActive(orderValue != 0);
		
	}
	
	private void OnMouseDown()
	{
		if (remainingDay <= 0)
		{
			GameManager.CloseAllDialogues();
			UpdateDialogBoxView(true);
			AudioManager.Instance.Play("Click");
		}
	}

	private float GetDailyConsumeModifier()
	{
		var faith = GameManager.Instance.faith;

		if (faith > 0 && faith <= 33)
		{
			return -1.5F;
		}
		
		if (faith > 33 && faith <= 66)
		{
			return -1.0F;
		}
		
		return -0.5F;
	}

	private float RewardModifier()
	{
		var order = GameManager.Instance.order;
		
		if (order > 0 && order <= 33)
		{
			return 0.5F;
		}
		
		if (order > 33 && order <= 66)
		{
			return 1.0F;
		}
		
		return 1.5F;
	}
}


