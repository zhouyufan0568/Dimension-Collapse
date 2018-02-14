using UnityEngine;

namespace DimensionCollapse
{
    public class KeyListener : Photon.PunBehaviour
    {
        RPCManager rpcManager;
        PlayerManager playerManager;

        void Start()
        {
            rpcManager = GetComponent<RPCManager>();
            playerManager = GetComponent<PlayerManager>();
        }

        void Update()
        {
            if (rpcManager == null)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                (playerManager.itemInHand as Missile)?.OnChargeStart();
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
                }

                SpaceGun spaceGun = playerManager.itemInHand as SpaceGun;
                if (spaceGun != null)
                {
                    rpcManager.SpaceGunAttackEndRPC();
                }
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                rpcManager.CastSkillOneRPC();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                rpcManager.PickUpItemRPC();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                rpcManager.DropHandItemRPC();
            }

        }
    }
}
