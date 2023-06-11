using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PointOfInterest : MonoBehaviour {
    [Tooltip("What does the Quizmaster say when you first talk to them? (Write this in Notepad and copy/paste into this inspector field)")]
    public string flavorText;

    [Tooltip("What question will the Quizmaster ask about this Point Of Interest?")]
    public string quizQuestion;

    [Tooltip("What is a wrong answer to the Quiz Question?")]
    public string answerA, answerB, answerC;

    [Tooltip("What is the correct answer to the Quiz Question?")]
    public string correctAnswer;

    [Tooltip("What is the name of this Point Of Interest?")]
    public string pOIName;

    [SerializeField]
    [Tooltip("Add the POIs canvas here")]
    Canvas pOICanvas; // The Canvas attatched to this object that displays the dialogue.

    [SerializeField]
    [Tooltip("How long should the Player Controller be paused when this UI pops up?")]
    float timeToPause = 0.5f;

    //  ********Event Handler and event that will notify an object that this dialogue has been displayed********
    public delegate void UITriggeredHandler(object source, EventArgs args);  
    public event UITriggeredHandler UITriggeredEvent;

    private void Start()
    {
        pOICanvas.GetComponentInChildren<Text>().text = flavorText;
        pOICanvas.enabled = false; //Disable the UI so it is not displayed before the Player finds and triggers it.

    }

    // ******** This method will disable the Players ability to click to move for a set time when the UI is fist displayed ********
    IEnumerator PauseForText()
    {
        InputManager.bCanPlayerMove = false; //Let Input Manager know not to pay attention to click inputs
        yield return new WaitForSeconds(timeToPause);
        InputManager.bCanPlayerMove = true; //Let Input Manager know to now pay attention to click inputs
    }

    //******** This method is called when another object intersects this objects trigger collider ************
    private void OnTriggerEnter(Collider other)
    {
        
        if(other.gameObject.tag == "Player")  //check if the colliding object is the player
        {
            StartCoroutine(PauseForText());   // pause the player control so they can read the text
            pOICanvas.enabled = true;         // turn on the POI canvas to display the text
            OnUITriggered();                  // let any listeners know the UI was triggered
        }
        
    }

    // ******* This method is called when an object leaves this objects trigger collider ********
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")  //check if the colliding object is the player
        {
            pOICanvas.enabled = false;        //turn off the POI canvas
        }
    }

    // *********** Event ***********
    protected virtual void OnUITriggered()
    {
        if (UITriggeredEvent != null)
        {
            UITriggeredEvent(this, EventArgs.Empty);
        }
    }
}
