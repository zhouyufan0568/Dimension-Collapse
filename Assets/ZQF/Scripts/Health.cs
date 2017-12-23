using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{

    public class Health : Photon.PunBehaviour, IPunObservable
    {
        public float maxHealth = 200;
        public float health;
        //private PhotonView photonView;
        // Use this for initialization
        void Start()
        {
            //photonView = GetComponent<PhotonView>();
            health = 200;
        }

        // Update is called once per frame
        void Update()
        {
            if (!photonView.isMine)
            {
                return;
            }
            if (health < 0)
            {
                health = 0;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                this.health = 200;
            }
        }

        public void OnAttacked(int primaryDamage, Vector3 contact)
        {
            if (!photonView.isMine)
            {
                return;
            }
            //Debug.Log(primaryDamage + "受到的伤害");
            this.health -= primaryDamage;
            //Debug.Log("Hash: " + this.gameObject.GetHashCode() + ", health is: " + health);
        }
        public void OnAttacked(int primaryDamage)
        {
            if (!photonView.isMine)
            {
                return;
            }
            //Debug.Log(primaryDamage + "受到的伤害");
            this.health -= primaryDamage;
        }
        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                stream.SendNext(health);
            }
            else
            {
                this.health = (float)stream.ReceiveNext();
            }
        }
    }
}
