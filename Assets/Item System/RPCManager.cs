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

        public void RangedWeaponChargerStartRPC()
        {
            if (photonView.isMine)
            {
                photonView.RPC("RangedWeaponChargerStart", PhotonTargets.All);
            }
        }

        public void RangedWeaponChargerEndRPC()
        {
            if (photonView.isMine)
            {
                photonView.RPC("RangedWeaponChargerEnd", PhotonTargets.All);
            }
        }

        public void EquipeWeapon(int index)
        {
            if (photonView.isMine)
            {
                photonView.RPC("EquipeWeaponByButton", PhotonTargets.All, index);
            }
        }

        public void DropItem(Inventory.ItemType it, int index)
        {
            if (photonView.isMine)
            {
                photonView.RPC("DropItemByUI", PhotonTargets.All, it, index);
            }
        }

        public void SwapItem(Inventory.ItemType itemType, int itemindex, Inventory.ItemType toitemType, int toitemindex)
        {
            if (photonView.isMine)
            {
                photonView.RPC("SwapItemByUI", PhotonTargets.All, itemType, itemindex, toitemType, toitemindex);
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

        [PunRPC]
        private void EquipeWeaponByButton(int index)
        {
            pickupManager.EquipeWeaponByButton(index);
        }

        [PunRPC]
        private void DropItemByUI(Inventory.ItemType it, int index)
        {
            pickupManager.DropItemByUI(it, index);
        }

        [PunRPC]
        private void SwapItemByUI(Inventory.ItemType itemType, int itemindex, Inventory.ItemType toitemType, int toitemindex)
        {
            pickupManager.SwapItemByUI(itemType, itemindex, toitemType, toitemindex);
        }
        #endregion

        [PunRPC]
        private void RangedWeaponChargerStart()
        {
            (playerManager.itemInHand as RangedWeaponChanger)?.Attack(true);
        }

        [PunRPC]
        private void RangedWeaponChargerEnd()
        {
            (playerManager.itemInHand as RangedWeaponChanger)?.Attack(false);
        }

    }
}
