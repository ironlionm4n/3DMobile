using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class QuizMaster : MonoBehaviour {
    
    PointOfInterest[] pOIs;        //an array of POIs  This is set in the Start method 
    PointOfInterest chosenPOI;     //a randomly chosen POI from the POIs array
    bool bPlayerFoundInfo = false; //has the player traveled to the chosen POI yet
    GameObject portal;             // a reference to the end level Portal object  This is set in the Start method

    [SerializeField]
    [Tooltip("Put the Text from the matching Panel here")]
    Text introTextBox, questionTextBox, responseTextBox;   //The text objects from the Panels in the Quizmaster Canvas

    [SerializeField]
    [Tooltip("How many seconds will we give the Player to read the Quizmasters text")]
    float timeToPause = 0.5f;   //How many seconds will we give the Player to read the Quizmasters text

    [SerializeField]
    [Tooltip("Put the Canvas that is on the Quizmaster here")]
    Canvas pOICanvas;  //The Canvas on the Quizmaster

    [SerializeField]
    [Tooltip("Put the Panel from the Canvas here")]
    GameObject introPanel, questionPanel, responsePanel;  //The Panel objects from the Quizmaster Canvas

    [SerializeField]
    [Tooltip("Put the 4 button texts from the Question Panel in Canvas here")]
    Text[] answerButtonTextFields;   //The buttons in the Question Panel in the Quizmaster Canvas

    [SerializeField]
    [Tooltip("What is the response the Quzmaster will give?")]
    string responseForCorrectAnswer, responseForWrongAnswer;    //The responses for correct and incorrect answers

    private bool bIsPortalOpen = false;  //Has the Player already won?

    // Use this for initialization
    void Start () {
        bIsPortalOpen = false;
        pOIs = FindObjectsOfType<PointOfInterest>();                  //Find all the POIs in the level and add them to an array
        chosenPOI = pOIs[UnityEngine.Random.Range(0, pOIs.Length)];   //make a random choice in the array and assign that POI to the chosenPOI var
        chosenPOI.UITriggeredEvent += POITriggered;                   //assign our POITriggered method to the Event on the chosen POI
        /*This is the intro text for the Quizmaster that will display when the Player first meets them*/
        introTextBox.text = "Hi I'm Quizmaster Steve. \n Go to the " + chosenPOI.pOIName + " and learn about it.  Then come back and answer a question about the " + chosenPOI.pOIName + " correctly and I'll teleport you to a new location!";
        SetupQuestionUI();                                            //Set up the Question UI
        portal = GameObject.Find("ExitPortal");                       //Get a reference to the Portal object in the level
        if(portal != null)
        {
            portal.SetActive(false);                                      //Deactivate the Portal object
        }
        
    }

    void SetupQuestionUI()  //we will cycle through the availible answers on the POI and add the answer text to a random button in this objects canvas  This will make each playthrough have a different order of answers so the Player cannot memorize the answer location
    {
        List<string> answerList = new List<string>();                        //Create a list for the possible answers from the chosenPOI and add each possible answer
        answerList.Add(chosenPOI.answerA);                                   
        answerList.Add(chosenPOI.answerB);
        answerList.Add(chosenPOI.answerC);
        answerList.Add(chosenPOI.correctAnswer);
        for (int i = 0; i < answerButtonTextFields.Length; i++)             //Cycle through the list
        {
            int randomNum = UnityEngine.Random.Range(0, answerList.Count);  //get a random number from 0 to the final availible answer in the list
            answerButtonTextFields[i].text = answerList[randomNum];         //add the text from the list at the random location to the next answerButtonTextField
            answerList.Remove(answerList[randomNum]);                       //remove the used answer from the list
        }
        questionTextBox.text = chosenPOI.quizQuestion;                      //assign the question from the POI to the questionText in the Quizmaster Canvas
        pOICanvas.enabled = false;                                          //disable the Canvas
    }

    public void CheckAnswer(int answer)
    {
        EnablePanels(false, false, true, true);                                     //Enable the response text panel
        if (answerButtonTextFields[answer].text == chosenPOI.correctAnswer) //check to see if the button clicked contains the correct answer
        {
            responseTextBox.text = responseForCorrectAnswer;                //display the Quizmasters congratulations text
            TurnOnPortal();                                                 //enable the Portal object
        }

        else                                                                //if the answer is wrong
        {
            responseTextBox.text = responseForWrongAnswer;                  //display the Quizmasters try again text
        }
       
    }

    public void EnablePanels(bool iPanel = false, bool qPanel = false, bool rPanel = false, bool canvasSwitch = false)
    {
        pOICanvas.enabled = canvasSwitch;                                            //enable the Quizmaster Canvas
        introPanel.SetActive(iPanel);                                        //Disable the intro text panel
        questionPanel.SetActive(qPanel);                                     //Disable the question text panel
        responsePanel.SetActive(rPanel);                                     //Enable the response text panel
        StartCoroutine(PauseForText(timeToPause));                                      //start a pause where the player cannot click to move so they have time to read the text
    }

    IEnumerator PauseForText(float pauseTime)
    {
        InputManager.bCanPlayerMove = false;                                //disable the players ability to move
        yield return new WaitForSeconds(pauseTime);                       //wait for the required seconds
        if(questionPanel.activeSelf == false)
        {
            InputManager.bCanPlayerMove = true;                                 //enable the players ability to move  
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")                              //if the player is colliding with the Quizmasters trigger collider
        {
            if (!bIsPortalOpen)
            {
                
                if (!bPlayerFoundInfo)                                         //if the player has not actually traveled to the chosenPOI
                {
                    EnablePanels(true, false, false, true);
                                                   //Enable the intro text panel 
                                               //Disable the question text panel
                                                //Disable the response text panel
                    
                }

                else                                                           //if the player has actually traveled to the chosenPOI
                {
                    InputManager.bCanPlayerMove = false;
                    EnablePanels(false, true, false, true);
                                                   //Disable the intro text panel
                                                 //Enable the question text panel
                                                //Disable the response text panel
                }
            }
            else
            {
                EnablePanels(false, false, true, true);
                responseTextBox.text = responseForCorrectAnswer;
            }
            
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")                              //if the player is no longer colliding with the Quizmasters trigger collider
        {
            EnablePanels();
                                              //Disable the intro text panel
                                           //Disable the question text panel
                                            //Disable the response text panel
                                                 //Disable the canvas
        }
    }

    public void POITriggered(object source, EventArgs e)
    {
        bPlayerFoundInfo = true;                                           //when the Event on the chosenPOI is fired set the bPlayerFoundInfo to true so the Quizmaster will ask a question the next time the player talks to them
    }

    private void TurnOnPortal()
    {
        bIsPortalOpen = true;
        if (portal != null)
        {
            portal.SetActive(true);                                            //set the Portal object to active so the player can travel to the next level
        }
           
    }
}
