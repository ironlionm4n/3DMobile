using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTracker : MonoBehaviour
{
    [Tooltip("Drag your player object here.")]
    [SerializeField]
    private Transform player;

    [Tooltip("Drag all of your Scriptable Objects for the sub-levels here.")]
    [SerializeField]
    private LevelContainerScriptableObject[] levels;

    //This is a vairable to track the current sub-level the player is in
    private LevelContainerScriptableObject currentLevel;

    [Tooltip("What is the name of your primary scene?")]
    [SerializeField]
    private string baseLevelName = "BaseLevel";

    [Tooltip("How many sub-scenes do you have in the Z direction?")]
    [SerializeField]
    private int numberOfColumns = 0;

    [Tooltip("How many sub-scenes do you have in the X direction?")]
    [SerializeField]
    private int numberOfRows = 0;

    [Tooltip("The rate in seconds that we will check what scenes need to be loaded/unloaded.")]
    [SerializeField]
    private float tickTime = 0.3f;

    [Tooltip("How many sub-scenes should be loaded next to each side of our current one.")] //For Example 2 will give us 2 to the left, 2 to the right 2 above and 2 below our current sub-level
    [SerializeField]
    private int megaGridSize = 2;

    void Start()
    {
        currentLevel = levels[0];  //Set the first sub level to be the one at the origin
        UpdateLevels(Vector2.zero); //Trigger the UpdateLevels method to load/unloaded the needed sub-levels
        InvokeRepeating("CheckPosition", 0f, tickTime);  //Since we don't need to check our position every frame we will only check it based on the provided TickTime
    }

    void CheckPosition()  //Check to see if the Player moved to the top/bottom/left or right then call the UpdateLevels method and let it know what direction the player is headed
    {
        if(player.position.z > currentLevel.mySceneWorldLocation.z + currentLevel.mySceneSize/2) 
        {
            UpdateLevels(currentLevel.mySceneGridLocation + Vector2.right);
        }
        else if (player.position.z < currentLevel.mySceneWorldLocation.z - currentLevel.mySceneSize / 2)
        {
            UpdateLevels(currentLevel.mySceneGridLocation + Vector2.left);
        }
        if (player.position.x > currentLevel.mySceneWorldLocation.x + currentLevel.mySceneSize / 2)
        {
            UpdateLevels(currentLevel.mySceneGridLocation + Vector2.up);
        }
        else if (player.position.x < currentLevel.mySceneWorldLocation.x - currentLevel.mySceneSize / 2)
        {
            UpdateLevels(currentLevel.mySceneGridLocation + Vector2.down);
        }
    }

    void UpdateLevels(Vector2 newScene)
    {
        if (newScene.x >= 0 && newScene.x < numberOfColumns) //Make sure the new scenes X position is less than the number of collumns and higher than 0
        {
            if (newScene.y >= 0 && newScene.y < numberOfRows) //Make sure the new scenes Z position is less than the number of rows and higher than 0
            {
                foreach (LevelContainerScriptableObject level in levels)  //look through the scriptable objects
                {
                    if (level.mySceneGridLocation == newScene) //if the new scenes grid number matches the one passed in to the method then:
                    {
                        currentLevel = level;  //set the current level to match the found scriptable object
                    }
                    if (IsLevelValid(level, newScene))  //check if the scenes name is a valid one in teh build settings list
                    {
                        if (!IsScene_CurrentlyLoaded(level.mySceneName)) //make sure the scene is not already loaded
                        {
                            SceneManager.LoadSceneAsync(level.mySceneName, LoadSceneMode.Additive); //load the new scene into our open level
                        }
                    }
                    
                    else
                    {
                        if (IsScene_CurrentlyLoaded(level.mySceneName)) //if the level is already loaded but not needed
                        {
                            StartCoroutine(UnloadLevel(SceneIndexFromName(level.mySceneName))); //unload the level after getting the build number
                        }
                    }
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(baseLevelName));  //set the active scene back to the base scene
                }
            }
        }
    }

    bool IsLevelValid(LevelContainerScriptableObject level, Vector2 newScene)  //check to see if we want to load the scene
    {
        if (level.mySceneGridLocation.x >= newScene.x -megaGridSize && level.mySceneGridLocation.x <= newScene.x + megaGridSize)  //check if the scene location is within the grid size we want in the x direction
        {
            if (level.mySceneGridLocation.y >= newScene.y -megaGridSize && level.mySceneGridLocation.y <= newScene.y + megaGridSize)  //check if the scene location is within the grid size we want in the z direction
            {
                return true;  //tell the asking method that we do want to load this scene
            }
        }
            return false;  //if it is not within the grid size tell the asking method we want to unload the scene
    }

    IEnumerator UnloadLevel(int levelBuildIndex) //we will asynch unload the scene here
    {
        yield return SceneManager.UnloadSceneAsync(levelBuildIndex, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects); //unload teh level and wait for the operation to finish
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(baseLevelName)); //set the active scene back to the base scene
    }

    private static string NameFromIndex(int BuildIndex)  //get the scene name from a provided build index
    {
        string path = SceneUtility.GetScenePathByBuildIndex(BuildIndex);  //find the path name from the provided index, remove the / and . discard the location information and find the scene name from the path
        int slash = path.LastIndexOf('/');  
        string name = path.Substring(slash + 1);
        int dot = name.LastIndexOf('.');
        return name.Substring(0, dot);
    }

    private int SceneIndexFromName(string sceneName)  //we will find the scene index from the provided name
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)  //look through the scenes in the build list
        {
            string testedScreen = NameFromIndex(i);  //load the scene name from the current index we are testing
            if (testedScreen == sceneName)  //if this matches the provided name
                return i; //return the index
        }
        return -1; //if the scene is not found return -1 
    }

    bool IsScene_CurrentlyLoaded(string sceneName)  //see if the scene is currently loaded
    {
        for (int i = 0; i < SceneManager.sceneCount; ++i)  //get the length of the build list and cycle through it
        {
            Scene scene = SceneManager.GetSceneAt(i);  //get the scene name at the current index we are testing
            if (scene.name == sceneName)  //if the names match
            {

                return true;  //tell the asking method this scene is loaded
            }
        }
        return false; //tell the asking method that this scene is not loaded
    }
}
