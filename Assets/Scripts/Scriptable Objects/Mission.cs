using UnityEngine;

[CreateAssetMenu(fileName = "Mission", menuName = "The Mist/Mission")]
public class Mission : ScriptableObject
{
	[Header("Information")]
	public string Title;
	public string Description;
	[Header("Rewards")]
	public int Food;
	public int Water;
	public int Faith;
	public int Order;
	[Space(10)]
	public int Duration;
}
