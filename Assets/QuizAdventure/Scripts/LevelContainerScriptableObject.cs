using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu] //this will automatically create a menu option for us in the Assets>Create dropdown

public class LevelContainerScriptableObject : ScriptableObject
{
    [Tooltip("Enter the name of the sub-scene you want to associate with this Scriptable Object.")]
    public string mySceneName;

    [Tooltip("Which Row and Column is this sub-scene in?")]
    public Vector2 mySceneGridLocation;

    [Tooltip("The position of the center of your sub-scene.")]
    public Vector3 mySceneWorldLocation;

    [Tooltip("What is the diameter or width of your sub-scene?")]
    public float mySceneSize;
}
