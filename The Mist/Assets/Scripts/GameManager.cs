using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
	private static GameManager _instance;

	public static GameManager Instance { get { return _instance; } }
	
	[Header("Resources")]
	[SerializeField] private float food;
	[SerializeField] private float water;
	[SerializeField] public float faith;
	[SerializeField] public float order;
	[SerializeField] private int maxValue;
	[SerializeField] private int starterFollowerCount;

	[Header("Datas")] 
	[SerializeField] private List<Mission> Missions;
	[SerializeField] private List<CharacterData> CharacterDatas;
	[SerializeField] private GameObject followerPrefab;
		
	[Header("UI")] 
	[SerializeField] private Image IMGFoodBar;
	[SerializeField] private Image IMGWaterBar;
	[SerializeField] private Image IMGFaithBar;
	[SerializeField] private Image IMGOrderBar;
	[SerializeField] private Text TXTDay;
	[SerializeField] private Text TXTGameOver;
	
	
	[Space(10)]
	public List<Sprite> ResourceSprites;
		
	public static List<Character> _characters = new List<Character>();
	public static int decideCount = 0;
	private static int cycleCount = 1;

	public void UpdateResource(Resource resourceType, float value, bool setToValue = false)
	{	
		switch (resourceType)
		{
			case Resource.Food:
				food = setToValue ? value : food + value;
				Mathf.Clamp(food, 0, maxValue);
				IMGFoodBar.fillAmount = (float) food / maxValue;
				break;
			case Resource.Water:
				water = setToValue ? value : water + value;				
				Mathf.Clamp(water, 0, maxValue);
				IMGWaterBar.fillAmount = (float) water / maxValue;
				break;
			case Resource.Faith:
				faith = setToValue ? value : faith + value;
				Mathf.Clamp(faith, 0, maxValue);
				IMGFaithBar.fillAmount = (float) faith / maxValue;
				break;
			case Resource.Order:
				order = setToValue ? value : order + value;
				Mathf.Clamp(order, 0, maxValue);
				IMGOrderBar.fillAmount = (float) order / maxValue;
				break;
			default:
				throw new ArgumentOutOfRangeException("resourceType", resourceType, null);
		}
	}

	public static void CloseAllDialogues()
	{
		foreach (Character character in _characters)
		{
			character.UpdateDialogBoxView(false);
		}
	}

	public Sprite GetResourceSprite(Resource resource)
	{
		return ResourceSprites[(int) resource];
	}
	
	private void Awake()
	{
		cycleCount = 1;
		decideCount = 0;
		
		if (_instance != null && _instance != this)
		{
			Destroy(gameObject);
		} else {
			_instance = this;
		}
	}

	private void Start()
	{
		SpawnFollowersAtStart();
		
		UpdateFollowers();
		
		EventManager.TriggerEvent("New Cycle");
	}

	private void Update()
	{
		UpdateResource(Resource.Food, 0);
		UpdateResource(Resource.Water, 0);
		UpdateResource(Resource.Faith, 0);
		UpdateResource(Resource.Order, 0);
	}

	private void SpawnFollowersAtStart()
	{
		if(followerPrefab == null) Debug.LogError("Follower prefab couldn't find.");

		var parentGameobject = GameObject.FindWithTag("Respawn").transform;
		for (var i = 0; i < starterFollowerCount; i++)
		{
			var character = Instantiate(followerPrefab, parentGameobject).GetComponent<Character>();
			character.Data = randomCharacterData();
			character.SetMission(RandomMission());
		}
		
		UpdateFollowers();
		
	}
	
	private void UpdateFollowers()
	{
		_characters.Clear();
		var chars = GameObject.FindGameObjectsWithTag("Player");
		foreach (var character in chars)
		{
			var characterComponent = character.GetComponent<Character>(); 
			if(characterComponent != null)
				_characters.Add(characterComponent);
			else 
				Debug.LogErrorFormat("{0}'s tag is Player but it hasn't got Character component.", character.name);
		}
	}

	public void NewCycle()
	{
		var chars = GameObject.FindGameObjectsWithTag("Player");
		var followersOnMission = 0;
		if(chars.Length == 0 || food <= 0 || faith <= 0 || water <= 0 || order <= 0) GameOver();

		var charComponents = chars[0].transform.parent.GetComponentsInChildren<Character>();
		foreach (var character in charComponents)
		{
			character.OnNewCycle();
			if (character.remainingDay > 0) followersOnMission++;
		}

		if (followersOnMission == chars.Length)
		{
			Sun.NextCycleWhenReady();
		}
		
		cycleCount++;

		TXTDay.text = string.Format("Day {0}", (cycleCount+1) / 2);
	}

	public Mission RandomMission()
	{
		return Missions[Random.Range(0, Missions.Count)];
	}

	private CharacterData randomCharacterData()
	{
		return CharacterDatas[Random.Range(0, CharacterDatas.Count)];
	}
	
	private void GameOver()
	{
		AudioManager.Instance.Play("GameOver");
		var gameOverInfo = string.Format(
			"You have survived for <size=119>{0}</size> days.\nYou've made <size=119>{1}</size> decisions.\n\nNow you're alone out in the mist.\nYou sacrificed yourself for your people."
			, (cycleCount + 1) / 2, decideCount);

		TXTGameOver.text = gameOverInfo;
		TXTGameOver.transform.parent.gameObject.SetActive(true);
	}

	public void PlaySound(string name)
	{
		AudioManager.Instance.Play(name);
	} 
}

public enum Resource
{
	Food = 0,
	Water,
	Faith,
	Order
}

public enum LookDirection
{
	Right = 0,
	Left
}

public static class Colors
{
	public static Color Grey1 = new Color32(60,55,61,255);
	public static Color Grey2 = new Color32(95,96,100,255);
	public static Color Grey3 = new Color32(126,127,131,255);
	public static Color Grey4 = new Color32(160,159,164,255);
	public static Color Grey5 = new Color32(208,208,208,255);
	public static Color Grey6 = new Color32(224,224,224,255);

	public static readonly Color[] colors = {Grey1,Grey2,Grey3,Grey4,Grey5,Grey6};

	public static Color Random(int min, int max)
	{
		var randomColorIndex = UnityEngine.Random.Range(min, colors.Length - max);
		return colors[randomColorIndex];
	}
}

