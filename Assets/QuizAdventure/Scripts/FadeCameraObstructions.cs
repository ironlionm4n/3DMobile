using System.Collections;
using UnityEngine;

public class FadeCameraObstructions : MonoBehaviour {
    
    GameObject player;     // the Players GameObject
    GameObject hitObject;  // the object between the Camera and Player
    Camera mainCam;        // the Main Camera in the scene
    Material temp;         // a temp material for the shader change

    [Tooltip("This should be Mobile/Diffuse")]
    [SerializeField]
    Shader originalShader; // the hitObjects original shader  This should be Mobile/Diffuse

    [Tooltip("This should be Mobile/Transparent/Mobile Transparent")]
    [SerializeField]
    Shader fadeShader;    // the shader we will use to fade out the hitObject This should be Mobile/Transparent/Mobile Transparent

    [Tooltip("How fast should the hitObject fade out and in?")]
    [SerializeField]
    float fadeSpeed;  //How fast should the hitObject fade out and in

    [Tooltip("How transparent should the hitObject get?")]
    [SerializeField]
    float targetAlpha;  // the target alpha for the fade

    

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");  //Find reference to the Player
        mainCam = Camera.main;
        
    }

    //Coroutine to fade the object from solid to transparent
    IEnumerator FadeMaterialOut(Material objMat)
    {        
            while (objMat.shader == fadeShader && objMat.color.a > targetAlpha)  //while the hitObject has the fadeShader applied and the alpha color is above the target alpha threshold 
            {
                Color colorOfMat = objMat.color;   //temp var to hold the current color of the material
                objMat.color = new Vector4(0, 0, 0, colorOfMat.a - Time.deltaTime * fadeSpeed);  //decrease the alpha of the color based on the time difference from the last frame time the fadeSpeed var
                yield return new WaitForFixedUpdate();  //wait until the next Fixed Update cycle
            }    
    }


    //Coroutine to fade the object from transparent to solid
    IEnumerator FadeMaterialIn(Material objMat)
    {  
        while (objMat.shader == fadeShader && objMat.color.a < 1f && hitObject != null)
        {
            Color colorOfMat = objMat.color;  //temp var to hold the current color of the material
            objMat.color = new Vector4(0, 0, 0, colorOfMat.a + Time.deltaTime * fadeSpeed);  //increase the alpha of the color based on the time difference from the last frame time the fadeSpeed var
            yield return new WaitForFixedUpdate();   //wait until the next Fixed Update cycle
        }
        if(objMat.shader == fadeShader && objMat.color.a >= 1f && hitObject!=null)  // if we have reached full opacity
        {
            objMat.shader = originalShader;  //set the shader back to the original shader
            hitObject = null;                //set the hitObject back to null to prepair for teh next object
        }
        else
        {
            objMat.shader = originalShader; // if all other cases fail just set the material back to the original shader
        }        
    }

	// Update is called once per frame
	void Update () {
        RaycastHit hit;  // store the raycast hit
        if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, 100))  //raycast from the camera to the player and log the hit
        {
            if(hit.collider.gameObject.tag != "Player" && hit.collider.gameObject.tag != "NPC" && hit.transform.gameObject.layer == LayerMask.NameToLayer("EnviromentIgnoreRaycast"))  //if the raycast hits something that 
            {
                if (hitObject != hit.collider.gameObject)  // only run if the hitObject is not already stored
                {   if(temp != null && temp.shader != originalShader) //check if we've already changed the shader and make sure there is a game object in the temp var
                    {
                        StopCoroutine("FadeMaterialOut");       //stop fading the material out
                        StartCoroutine(FadeMaterialIn(temp));   //start fading the material in
                    }                    
                    hitObject = hit.collider.gameObject;                  //set the hitObject var to be the object hit by the raycast
                    temp = hitObject.GetComponent<Renderer>().material;   //get a reference to the material
                    temp.shader = fadeShader;                             //set the shader to the fadeShader
                    temp.color = new Vector4(0, 0, 0, 1f);                //make sure the new material is set to be fully solid
                    StartCoroutine(FadeMaterialOut(temp));                //start fading the material out        
                }               
            }
            else   
            {
                if(hitObject!= null)  //make sure we have a reference
                {
                    StartCoroutine(FadeMaterialIn(temp)); //if all other checks fail make sure we fade the material in
                }
            }            
        }
    }
}
