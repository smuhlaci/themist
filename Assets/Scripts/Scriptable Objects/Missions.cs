using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class Missions : MonoBehaviour 
{

}
#if UNITY_EDITOR

public class CreateNewData
{
    [MenuItem("The Mist/Create/Mission")]
    public static Mission CreateMission()
    {
        var asset = ScriptableObject.CreateInstance<Mission>();

        AssetDatabase.CreateAsset(asset, "Assets/Resources/Mission.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }
    
    [MenuItem("The Mist/Create/Character")]
    public static CharacterData CreateCharacterData()
    {
        var asset = ScriptableObject.CreateInstance<CharacterData>();

        AssetDatabase.CreateAsset(asset, "Assets/Resources/Character.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }
}
#endif