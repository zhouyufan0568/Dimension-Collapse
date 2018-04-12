using UnityEngine;

namespace DimensionCollapse
{
    public class KeyListener : Photon.PunBehaviour
    {
        private RPCManager rpcManager;
        private PlayerManager playerManager;
        private AnimationCenter animationCenter;

        void Start()
        {
            rpcManager = GetComponent<RPCManager>();
            playerManager = GetComponent<PlayerManager>();
            animationCenter = GetComponentInChildren<AnimationCenter>();
        }

        void Update()
        {
            if (!photonView.isMine)
            {
                return;
            }

            animationCenter.HandleInput();

            if (rpcManager == null)
            {
                return;
            }

            if (Input.GetButton("Backpack"))
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                (playerManager.itemInHand as Missile)?.OnChargeStart();
                RangedWeaponChanger rangedWeaponChanger = playerManager.itemInHand as RangedWeaponChanger;
                if(rangedWeaponChanger != null)
                {
                    rpcManager.RangedWeaponChargerStartRPC();
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (playerManager.itemInHand != null)
                {
                    if (playerManager.itemInHand is Missile)
                    {
                        (playerManager.itemInHand as Missile)?.OnCharge();
                        
                    }
                    else if (playerManager.itemInHand is Weapon)
                    {
                        Weapon weapon = playerManager.itemInHand as Weapon;
                        if (weapon.CanAttack())
                        {
                            rpcManager.UseItemInHandRPC();
                        }
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                Missile missile = playerManager.itemInHand as Missile;
                if (missile != null)
                {
                    Vector3 force = missile.OnChargeEnd();
                    rpcManager.ThrowItemInHandRPC(force);
                    playerManager.itemInHand = null;
                    playerManager.GetComponent<Inventory>().RemoveReference(missile.gameObject);
                }

                SpaceGun spaceGun = playerManager.itemInHand as SpaceGun;
                if (spaceGun != null)
                {
                    rpcManager.SpaceGunAttackEndRPC();
                }

                RangedWeaponChanger rangedWeaponChanger = playerManager.itemInHand as RangedWeaponChanger;
                if(rangedWeaponChanger != null)
                {
                    rpcManager.RangedWeaponChargerEndRPC();
                }
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                rpcManager.CastSkillOneRPC();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                rpcManager.CastSkillTwoRPC();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                rpcManager.PickUpItemRPC();
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                rpcManager.DropHandItemRPC();
            }

            if (Input.GetButton("MainWeapon1")) {
                rpcManager.EquipeWeapon(0);
            }

            if (Input.GetButton("MainWeapon2"))
            {
                rpcManager.EquipeWeapon(1);
            }

            if (Input.GetButton("Missile"))
            {
                rpcManager.EquipeWeapon(2);
            }

            if (Input.GetButton("Remedy1"))
            {
                rpcManager.EquipeWeapon(3);
            }

            if (Input.GetButton("Remedy2"))
            {
                rpcManager.EquipeWeapon(4);
            }
        }
    }
}
