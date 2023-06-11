using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PortalExit : MonoBehaviour {
    [SerializeField]
    [Tooltip("Add the Animator from the Fade Panel here.  it is under the Main Cameras Canvas.")]
    private Animator fadePanelAnimator;

    [Tooltip("What is Build Number of the next scene?")]
    [SerializeField]
    private int nextSceneNumber = 0;

    [Tooltip("How many seconds is your fade out animation?  Mostly this should stay at 1.")]
    [SerializeField]
    private float secondsOfFade = 1f;
    
    // ********* Method is called when an object intersects this objects trigger collider *********
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")  // Check if colliding object is the Player
        {
            EnterPortal();                    //Call the Enter Portal method
        }
    }


    // *********Method is called when Player enters the portal trigger **********
    private void EnterPortal()
    {
        InputManager.bCanPlayerMove = false;                  // Turn off player control
        if(fadePanelAnimator != null)
        {
            fadePanelAnimator.SetTrigger("FadeTransitionOut");    // Start the fade out animation
        }
        
        StartCoroutine(WaitForFade());                        // start waiting for the fade
    }


    // *********Wait for Fade animation **********
    private IEnumerator WaitForFade()
    {
        yield return new WaitForSeconds(secondsOfFade);    //Wait for the specified time to allow the animation to play

        SceneManager.LoadScene(nextSceneNumber);          // Load the specified next scene
    }

}
