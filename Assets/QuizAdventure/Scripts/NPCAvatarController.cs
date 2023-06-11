using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(MovableObjectMotor))]     //requires this object to also have a MovableObjectMotor script on it
[RequireComponent(typeof(NavMeshAgent))]           //requires this object to also have a NavMeshAgent script on it
[RequireComponent(typeof(Rigidbody))]              //requires this object to also have a Rigidbody script on it

public class NPCAvatarController : MonoBehaviour {
    
    [SerializeField]
    [Tooltip("How far can the NPC walk from their current location at a time?")]
    float walkRadius = 30f;  //the furthest this NPC can walk from its last location

    [SerializeField]
    [Tooltip("How long in seconds does this NPC pause while deciding on its next state?")]
    Vector2 decisionPauseTime;  //What is the min and max seconds the NPC will pause while deciding on its next state?

    [SerializeField]
    [Tooltip("What does this NPC say to the player when they collide?")]
    string characterLine;  // What does this NPC say to the player when they collide?

    [SerializeField]
    [Tooltip("Put the Canvas on the NPC object here")]
    Canvas nPCCanvas; // The Canvas attatched to this object that displays the dialogue.

    [SerializeField]
    [Tooltip("Put the Text object in the NPC canvas here")]
    Text nPCTextBox;  //the Text box that will display the NPC line
    [SerializeField]
    [Tooltip("If you are using Text Mesh Pro Put the TMP object in the NPC canvas here")]
    TextMeshProUGUI textMeshProText;

    [SerializeField]
    [Tooltip("If the NPC wanders beyond this distance from its starting location it will return to the starting location")]
    float distanceFromStart  = 100f;  //how far from the starting position can this NPC wander

    AIState currentAIState;           //stores the curent state of the NPC
    Vector3 startLocation;            //stores the starting location of the NPC
    Transform playerTransform;        //reference to the playerTransform in the scene Set when the NPC collides with the Player
    Animator myAnimator;              //reference to the Move script on this object Set in Start
    MovableObjectMotor objectMotor;   //reference to the Move script on this object Set in Start
    bool bSelectingPosition = false;  //are we currently chosing a position?
    bool bIsDeciding = false;         //are we currently chosing what state to be in?

    // Use this for initialization
    void Start () {
        currentAIState = AIState.IDLE;                          //Make sure the NPC starts in the Idle state
        myAnimator = this.GetComponent<Animator>();             //Store a reference to the Animator on this object            
        objectMotor = this.GetComponent<MovableObjectMotor>();  //Store a reference to the MovableObjectMotor on this object    
        startLocation = this.transform.position;                //Store the location the NPC is at the start of the level
        
        if(nPCTextBox != null)
        {
            nPCTextBox.text = characterLine;                       //Make the Text box display the character line
        }
        else if (textMeshProText != null)
        {
            textMeshProText.text = characterLine;
        }
        
        nPCCanvas.enabled = false;                             //Disable the UI so it is not displayed before the Player finds and triggers it.


    }
	
	// Update is called once per frame
	void Update () {
        switch (currentAIState)  //look at what State we are currently in and call the appropriate method
        {
            case AIState.WALK:
                Walk();
                break;
            case AIState.RUN:
                Run();
                break;
            case AIState.IDLE:
                Idle();
                break;
            case AIState.SIT:
                Sit();
                break;
            case AIState.WAVE:
                Wave();
                break;
            case AIState.DECIDING:
                WhatDoIDo();
                break;
            default:
                Idle();
                break;
        }
	}

    void Run()
    {
        if (objectMotor.agent.remainingDistance <= objectMotor.agent.stoppingDistance && !objectMotor.agent.isStopped)  //if the objectMotor says the NPC is within the stopping distance allowed and the NPC is not already stopped
        {
            currentAIState = AIState.DECIDING;  //set the current state to be Deciding
        }
    }

    void Walk()
    {
        //TODO  we can change to a walking animation if the speed falls below a threshold
        //This can be added at a later time as a challenge to the student
    }

    void Idle()
    {
        if (!bSelectingPosition && myAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Idle")  //If we are not already selecting a position and the Animator is already playing the Idle animation
        {
            
            SelectTargetLocation();  //Choose our next target location
        }
       
    }

    void Sit()
    {
        //TODO  we can change to a sitting animation
        //This can be added at a later time as a challenge to the student

    }

    void Wave()
    {
        transform.LookAt(playerTransform);  //Rotate to face the player location
        nPCCanvas.transform.LookAt(transform.position * 2 - Camera.main.transform.position);  //make sure the NPC Canvas is facing the Main Camera
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")              // Did we cross into the Players trigger collider?
        {
            objectMotor.Stop();                //Tell our objectMotor to stop all movement
            playerTransform = other.transform; //Store a reference to the object we collided with (should be the player)
            myAnimator.SetTrigger("Wave");     //Tell our Animator to start playing the Wave animation
            currentAIState = AIState.WAVE;     //set the current state to be Wave
            nPCCanvas.enabled = true;          //Enable our Canvas to the Player can read our line
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")              //If we leave the trigger collider around the player
        {  
            nPCCanvas.enabled = false;          //Disable our Canvas to hide our line
            myAnimator.SetTrigger("Wave");      //Tell our Animator to start playing the Wave animation again
            currentAIState = AIState.DECIDING;  //Set the current state to be Deciding
            objectMotor.Go();                   //Tell our objectMotor it is OK to start moving again
        }
    }

   void WhatDoIDo()
    {
        if(!bIsDeciding)                       //check to see if we are already making a decision
        {
            bIsDeciding = true;                //set to true so we do not keep trying to make a desision
            StartCoroutine(PauseCharacter());  //pause our NPC to give the illusion of thinking about their next move
        }
    }

    IEnumerator PauseCharacter()
    {
        float pauseTime = Random.Range(decisionPauseTime.x, decisionPauseTime.y);  //chose a random number from our decisionPauseTime range
        yield return new WaitForSeconds(pauseTime);                                //wait for the random seconds to give the illusion of making a decision
        bSelectingPosition = false;                                                //we are finished selecting a position 
        bIsDeciding = false;                                                       //we are finished making a decision
        
        currentAIState = AIState.IDLE;                                             //set the current state to be Idle
    }

    void SelectTargetLocation()
    {
        if (!nPCCanvas.enabled && myAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Idle_Wave")  //check if we are both hiding our canvas and that we are not currently playing the Wave animation
        {
            bSelectingPosition = true;                                                                   //toggle the bool so we know we are currently choosing a location to move to
            
            if(Vector3.Distance(this.transform.position, startLocation) > distanceFromStart)            //check to see if we've wandered too far from our starting location
            {
                objectMotor.MoveObjectTo(startLocation);                                                //if we have wandered to far set our next location to be where we started the level
            }

            else
            {
                Vector3 randomDirection = Random.insideUnitSphere * walkRadius;    //choose a random direction within the specified walkRadius
                randomDirection += transform.position;                             //add to our current location to get a vector of movement
                NavMeshHit hit;                                                    //store our NavMesh hit
                NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);   //Query the NavMesh to get a position on the NavMesh in the adjusted direction and within the max walkRadius
                Vector3 finalPosition = hit.position;                              //Store our final NavMesh position.  This can be used in extended logic if we want to track where the NPC has been before (not currently implemented)
                objectMotor.MoveObjectTo(finalPosition);                           //Tell our objectMotor to move us to the new chosen position
            }            
            currentAIState = AIState.RUN;                                          //set the current state to be Run
        }

        else
        {
            currentAIState = AIState.WAVE;  //set the current state to be Wave
        } 
    }
}

public enum AIState  //an enum that stores all the different states the NPC can be in
{
    WALK,
    RUN,
    IDLE,
    SIT,
    WAVE,
    DECIDING,
}
