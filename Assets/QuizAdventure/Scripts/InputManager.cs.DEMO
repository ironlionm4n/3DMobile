using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputManager : MonoBehaviour {
    // *********  Event and Handler for when the player clicks a mouse button *******
    public delegate void ClickDownHandler(object source, EventArgs args);
    public event ClickDownHandler ClickDownEvent;

    // ********* Event and Handler for when the player presses ESC **********
    public delegate void EscHandler(object source, EventArgs args);
    public event EscHandler EscEvent;

    [HideInInspector]
    static public bool bCanPlayerMove = true;  // Can the player move? T or F

    private void Awake()
    {
        InputManager.bCanPlayerMove = true;  // Make sure the player can move
    }

    // Update is called once per frame
    void Update () {
        
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)  //Check if the player is allowed to move and if the player has clicked the Left Mouse Button or pressed the mobile screen
        {
            if (bCanPlayerMove)
            {
                OnClickDown();  // Call Click Event
            }
            
        }
        if (Input.GetKeyDown(KeyCode.Escape)) //Check if the player is allowed to move and if the player has clicked the ESC key
        {
            if (bCanPlayerMove)
            {
                OnEscPressed();  // Call ESC Pressed Event
            }
            
        }
	}
     // Used if the player clicks on the Home icon in the GUI
    public void EscapeGUIPressed()  
    {
        OnEscPressed();  // Call ESC Pressed Event
    }

    // *********** Event ***********
    protected virtual void OnEscPressed()
    {
        if (EscEvent != null)
        {
            EscEvent(this, EventArgs.Empty);
        }
    }

    // *********** Event ***********
    protected virtual void OnClickDown()
    {
        if(ClickDownEvent != null)
        {
            ClickDownEvent(this, EventArgs.Empty);
        }
    }

    
}
