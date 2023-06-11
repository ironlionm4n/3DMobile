using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseCamera : MonoBehaviour {
    
    [SerializeField]
    [Tooltip("Which Transform are we following?")]
    Transform targetTransform;             // Which Transform are we following?

    [Tooltip("How far behind the target should we be?  Negative numbers will place us in front of the target")]
    public float distance = 50.0f;         // the distance this object should be from the target.  Negative numbers will place us in front of the target.

    [Tooltip("How high should the camera be?")]
    public float height = 35.0f;            // the height we want the camera to be above the target

    [Tooltip("Helps smooth out vertical movements")]
    public float heightDamping = 1.0f;     // Damping for the height movement

    [Tooltip("Helps smooth out rotational movements")]
    public float rotationDamping = 0.0f;   // Damping for the rotational movement


    
	
	// Update is called once per frame
	void LateUpdate () {
        
        float wantedRotationAngle = targetTransform.eulerAngles.y;
        float wantedHeight = targetTransform.position.y + height;
        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

        // Damp the rotation around the y-axis
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        // Convert the angle into a rotation
        var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        transform.position = targetTransform.position;
        transform.position -= currentRotation * Vector3.forward * distance;

        // Set the height of the camera
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

    }
}
