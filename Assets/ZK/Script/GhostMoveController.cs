using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMoveController : Photon.PunBehaviour
{
	[SerializeField] private Transform mCamera;

    public float MoveSpeed = 8f;
    public float RotSpeed = 25f;
//    bool isRotate = false;

 //   private float trans_y = 0;
//    private float trans_x = 0;
  //  private float trans_z = 0;

    private float eulerAngles_x;
    private float eulerAngles_y;

    // Use this for initialization
    void Start()
    {
		if (photonView.isMine == false)
		{
			mCamera.gameObject.SetActive(false);
			this.enabled = false;
		}

		Cursor.lockState = CursorLockMode.Locked;

        Vector3 eulerAngles = this.transform.eulerAngles;//当前物体的欧拉角

        this.eulerAngles_x = eulerAngles.y;

        this.eulerAngles_y = eulerAngles.x;
    }


    void Update()
    {

        this.eulerAngles_x += (Input.GetAxis("Mouse X") * this.RotSpeed) * Time.deltaTime;

        this.eulerAngles_y -= (Input.GetAxis("Mouse Y") * this.RotSpeed) * Time.deltaTime;

        Quaternion quaternion = Quaternion.Euler(this.eulerAngles_y, this.eulerAngles_x, (float)0);

        this.transform.rotation = quaternion;

        moveByKey(MoveSpeed);
    }


    void moveByKey(float speed)
    {
        /*if (Input.GetKey(KeyCode.Q))
        {
            this.transform.Translate(Vector3.down * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.E))
        {
            this.transform.Translate(Vector3.up * speed * Time.deltaTime);
        }*/

        float moveV = Input.GetAxis("Vertical");
        float moveH = Input.GetAxis("Horizontal");

        // default translate relative to Space.self
        this.transform.Translate(Vector3.forward * speed * moveV * Time.deltaTime + Vector3.right * speed * moveH * Time.deltaTime);
    }

}
