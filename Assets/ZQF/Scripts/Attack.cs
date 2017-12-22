using UnityEngine;
using DimensionCollapse;

public class Attack : Photon.PunBehaviour
{

    //public Weapon weapon;
    private Shoot shoot;
    private PhotonView photonVieww;

    void Start()
    {
        shoot = GetComponentInParent<Shoot>();
        photonVieww = GetComponentInParent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (photonVieww.isMine)
            {
                photonVieww.RPC("ShootCore", PhotonTargets.All);
            }
        }
    }
}
