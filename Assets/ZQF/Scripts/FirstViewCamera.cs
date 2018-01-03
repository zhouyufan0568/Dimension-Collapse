using UnityEngine;

/**
 * 
 * make cameraDistance alterable
 * 
 */

public class FirstViewCamera : Photon.PunBehaviour {

    [SerializeField] private Transform mCenter;
    [SerializeField] private Transform mCamera;
    [SerializeField] private Transform mRaycastor;

    //处理背靠墙体视角问题
    public float cameraDistance;

    void Update()
    {
        RaycastHit hit;
        Vector3 forward = mCamera.position - mRaycastor.position;
        if(Physics.Raycast(mRaycastor.position,forward.normalized,out hit, cameraDistance))
        {
            if (mCamera != hit.collider.transform)
            {
                mCamera.position = hit.point;
            }
        }
        else
        {
            mCamera.position = mRaycastor.position + forward.normalized * cameraDistance;
        }
        //Debug.DrawRay(mRaycastor.position, forward.normalized*cameraDistance, Color.red, 0);
    }

    void Start()
    {
        if (photonView.isMine == false)
        {
            mCamera.gameObject.SetActive(false);
            this.enabled = false;
        }
        //cameraDistance = (mCamera.position - mRaycastor.position).magnitude;
    }

}
