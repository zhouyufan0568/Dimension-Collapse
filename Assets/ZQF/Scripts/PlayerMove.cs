using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

    public float speed = 5f;//总速度系数

    public float jumpSpeed = 5.0f;
    public float gravity = 10f;
    public float frontSpeed = 1.0f;//向前走的相对速度
    public float backSpeed = 0.5f;//后退的相对速度
    public float sideSpeed = 0.8f;//左右走的相对速度
    private CharacterController controller;
    private Vector3 directionVector = Vector3.zero;


	// Use this for initialization
	void Start () {
        controller = GetComponent<CharacterController>();
    }
	
	// Update is called once per frame
	void Update () {
        if (controller.isGrounded)
        {
            float horizontalAxis = Input.GetAxis("Horizontal");
            float verticalAxis = Input.GetAxis("Vertical");
            float vv = verticalAxis > 0 ? verticalAxis * frontSpeed : verticalAxis * backSpeed;
            float hh = horizontalAxis * sideSpeed;
            directionVector = transform.TransformDirection(new Vector3(hh, 0, vv))*speed;
            
            if (Input.GetButton("Jump"))
            {
                directionVector.y = jumpSpeed;
            }
        }
        //else
        //{
        //    float horizontalAxis = Input.GetAxis("Horizontal");
        //    float verticalAxis = Input.GetAxis("Vertical");
        //    float vv = verticalAxis > 0 ? verticalAxis * frontSpeed : verticalAxis * backSpeed;
        //    float hh = horizontalAxis * sideSpeed;
        //    directionVector.x = hh * speed/2;
        //    directionVector.z = vv * speed/2;
        //    directionVector = transform.TransformDirection(new Vector3(hh / 2, directionVector.y, vv / 2)) * speed;
        //}
        directionVector.y -= gravity * Time.deltaTime;
        controller.Move(directionVector*Time.deltaTime);
        
    }


}
