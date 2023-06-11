using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class TestController : MonoBehaviour
{
    public float MoveSpeed,mouseSpeed;
    CharacterController cc;


    void Start()
    {
        cc = GetComponent<CharacterController>();
    }


    void Update()
    {

        //moves the vector forward 1 point so original point is //vector 0,0,0 then changes to vector 0,0,1
        Vector3 forward = Input.GetAxis("Vertical") * transform.TransformDirection(Vector3.forward) * MoveSpeed;
        cc.Move(forward * Time.deltaTime);
        cc.SimpleMove(Physics.gravity);
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * Time.deltaTime * mouseSpeed);
    }
}
