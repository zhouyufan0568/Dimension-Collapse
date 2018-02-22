using UnityEngine;

namespace DimensionCollapse
{
    public class RPCManager : Photon.PunBehaviour
    {
        private PlayerManager playerManager;
        #region private add by ZQF
        //拾取脚本
        private PickupManager pickupManager;
        #endregion
        private void Start()
        {
            playerManager = GetComponent<PlayerManager>();

            #region add by ZQF
            pickupManager = GetComponent<PickupManager>();
            #endregion
        }

        public void UseItemInHandRPC()
        {
            if (photonView.isMine)
            {
                photonView.RPC("UseItemInHand", PhotonTargets.All);
            }
        }

        public void ThrowItemInHandRPC(Vector3 force)
        {
            if (photonView.isMine)
            {
                photonView.RPC("ThrowItemInHand", PhotonTargets.All, force);
            }
        }

        public void CastSkillOneRPC()
        {
            if (photonView.isMine)
            {
                photonView.RPC("CastSkillOne", PhotonTargets.All);
            }
        }

        public void CastSkillTwoRPC()
        {
            if (photonView.isMine)
            {
                photonView.RPC("CastSkillTwo", PhotonTargets.All);
            }
        }

        public void SpaceGunAttackEndRPC()
        {
            if (photonView.isMine)
            {
                photonView.RPC("SpaceGunAttackEnd", PhotonTargets.All);
            }
        }

        public void PickUpItemRPC()
        {
            if (photonView.isMine)
            {
                int photonViewId = pickupManager.PickItem();
                if (photonViewId != -1)
                {
                    photonView.RPC("PickUpItem", PhotonTargets.All, photonViewId);
                }
            }
        }

        public void DropHandItemRPC()
        {
            if (photonView.isMine)
            {
                photonView.RPC("DropHandItem", PhotonTargets.All);
            }
        }

        public void SwitchItemInHandRPC()
        {
            if (photonView.isMine)
            {
                photonView.RPC("SwitchItemInHand", PhotonTargets.All);
            }
        }

        [PunRPC]
        private void UseItemInHand()
        {
            Item item = playerManager.itemInHand;
            (item as Weapon)?.Attack();
        }

        [PunRPC]
        private void ThrowItemInHand(Vector3 force)
        {
            Item item = playerManager.itemInHand;
            (item as Missile)?.Throw(force);
        }

        [PunRPC]
        private void SpaceGunAttackEnd()
        {
            (playerManager.itemInHand as SpaceGun)?.AttackEnd();
        }

        [PunRPC]
        private void CastSkillOne()
        {
            CastSkill(playerManager.skillOne);
        }

        [PunRPC]
        private void CastSkillTwo()
        {
            CastSkill(playerManager.skillTwo);
        }

        private void CastSkill(Skill skill)
        {
            (skill as NondirectiveSkill).Cast();
        }

        [PunRPC]
        private void SwitchItemInHand()
        {
            playerManager.SwitchItemInHand();
        }

        #region function add by ZQF
        [PunRPC]
        private void PickUpItem(int photonViewId)
        {
            pickupManager.PickItemCore(photonViewId);
        }
        [PunRPC]
        private void DropHandItem()
        {
            pickupManager.DropHandItem();
        }
        #endregion

    }
}
