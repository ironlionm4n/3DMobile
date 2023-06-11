using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

/* THIS SCRIPT SHOULD BE PLACED ON THE FADE PANEL OBJECT FOUND UNDER THE MAIN CAMERAS CANVAS */
public class EscUIController : MonoBehaviour {
    private InputManager inputManager;
    private Animator myAnimator;


    // Use this for initialization.
    void Start () {
        inputManager = GameObject.Find("InputManagerObj").GetComponent<InputManager>();  // Get a reference to the InputManager in the scene
        inputManager.EscEvent += OnEscPressed;      // Register for an Event callback
        myAnimator = GetComponent<Animator>();      // get reference to this objects Animator
    }
	
	/*When Escape is pressed disable player movement and fade in the pause menu*/
    public void OnEscPressed(object source, EventArgs e)
    {
        DisablePlayerMovement();
        myAnimator.SetTrigger("FadeIn");
    }

    /*Plays a fade to black animation for the Fade Panel*/
    public void QuitGameFade()
    {
        myAnimator.SetTrigger("FadeQuit");
    }

    /*Exits the application*/
    public void QuitGame()
    {
        Application.Quit();
        
    }

    /*Plays a fade to black animation for the Fade Panel*/
    public void ReturnToGame()
    {
        myAnimator.SetTrigger("FadeOut");
    }

    /*Used to turn on player movement after cutscenes or when pause screen is inactive*/
    public void EnablePlayerMovement()
    {
        InputManager.bCanPlayerMove = true;
    }

    /*Used to turn off player movement in cutscenes or when pause screen is active*/
    public void DisablePlayerMovement()
    {
        InputManager.bCanPlayerMove = false;
    }
}
