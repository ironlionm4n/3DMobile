using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAvatarController : MonoBehaviour {
    private InputManager inputManager;         //a reference to the inputManager in the scene  Set in the Start method
    private Camera mainCam;                    //a reference to the MainCamera in the scene  Set in the Start method
    private MovableObjectMotor objectMotor;    //a reference to the MovableObjectMotor on this object  Set in the Start method
    private LayerMask walkableMask;            //what Layer are we going to be casting a ray against

    [SerializeField]
    [Tooltip("How many seconds will we give the Player to read the NPCs text when they bump into them")]
    float timeToPause = 0.5f;   //How many seconds will we give the Player to read the NPCs text


    // Use this for initialization
    void Start () {
        mainCam = Camera.main;                                //get a reference to the MainCamera in the scene
        objectMotor = GetComponent<MovableObjectMotor>();     //get a reference to the MovableObjectMotor in the scene
        walkableMask = 1 << 8;                                //Make sure the Layer Mask is set to Layer 8 which should always be "EnvironmentWalkable" 

        inputManager = GameObject.Find("InputManagerObj").GetComponent<InputManager>();    //get a reference to the InputManager in the scene

        inputManager.ClickDownEvent += OnClickDown;           //register to the ClickDownEvent on the InputManager so we get notified when the player clicks the LeftMouseButton
        inputManager.EscEvent += OnEscPressed;                //register to the EscEvent on the InputManager so we get notified when the player presses the Esc key or clicks the Home icon in the GUI
    }
    public void OnEscPressed(object source, EventArgs e)     //when the Esc key event is fired
    {
        objectMotor.MoveObjectTo(transform.position);        //set our desired location on the NavMesh to the current location of the Player (a redundant call to make sure we stop moving)
    }

    public void OnClickDown(object source, EventArgs e)    //when the LeftMouseButton pressed event is fired
    {
        /*EventSystem et = EventSystem.current;
        
        if (et.IsPointerOverGameObject() || et.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;
        if (et.currentSelectedGameObject != null) return;*/
        RaycastHit hit;                                                    // store the Raycast hit
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);           //cast a ray from the MainCamera through the click position 
        
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, walkableMask))    //cast a ray using the LayerMask
        {
            
            objectMotor.MoveObjectTo(hit.point);                          //tell the objectMotor on this object the location of the click and start moving there
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "NPC" || other.gameObject.tag == "POI")  //if we have crossed a trigger collider belonging to a NPC or POI
        {
            objectMotor.Stop();                                             //tell the objectMotor to stop all movement
            transform.LookAt(other.transform);                              //rotate the player to face the NPC or POI
            StartCoroutine(PauseCharacter());                               //start pausing the players movement so they have some time to read the info from the NPC or POI
        }
    }

    IEnumerator PauseCharacter()
    {
        
        yield return new WaitForSeconds(timeToPause);   //wait for x seconds so player can read the info from the NPC or POI

        objectMotor.Go();                      //tell the objectMotor it's ok to start moving again

    }

    
}
