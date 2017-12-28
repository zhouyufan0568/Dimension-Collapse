using UnityEngine;

public class FirstViewCamera : Photon.PunBehaviour {

    [SerializeField] private Transform mCenter;
    [SerializeField] private Transform mCamera;
    [SerializeField] private Transform mRaycastor;
    [SerializeField] private Transform m_MiniMap;

    //处理背靠墙体视角问题
    [SerializeField] private float cameraDistance = 2;

    void Update()
    {
        updateCameraDistance();
        RaycastHit hit;
        Vector3 forward = mCamera.position - mRaycastor.position;
        if(Physics.Raycast(mRaycastor.position,forward.normalized,out hit, cameraDistance))
        {
            if (mCamera != hit.collider.transform)
            {
                mCamera.position = hit.point+new Vector3(0,0.1f,0);
            }
        }
        else
        {
            mCamera.position = mRaycastor.position + forward.normalized * cameraDistance;
        }
        Debug.DrawRay(mRaycastor.position, forward.normalized*cameraDistance, Color.red, 0);
    }

    void Start()
    {
        if (photonView.isMine == false)
        {
            mCamera.gameObject.SetActive(false);
            m_MiniMap.gameObject.SetActive(false);
            this.enabled = false;
        }
        //cameraDistance = (mCamera.position - mRaycastor.position).magnitude;
    }

    private void updateCameraDistance()
    {
        
    }

}
