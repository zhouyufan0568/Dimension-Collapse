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
                    if (!(playerManager.itemInHand is Missile))
                    {
                        rpcManager.UseItemInHandRPC();
                    }
                    else
                    {
                        (playerManager.itemInHand as Missile)?.OnCharge();
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector3? force = (playerManager.itemInHand as Missile)?.OnChargeEnd();
                if (force.HasValue)
                {
                    rpcManager.ThrowItemInHandRPC(force.Value);
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
