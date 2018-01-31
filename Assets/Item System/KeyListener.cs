using UnityEngine;

namespace DimensionCollapse
{
    public class KeyListener : Photon.PunBehaviour
    {
        RPCManager rpcManager;
        void Start()
        {
            rpcManager = GetComponent<RPCManager>();
        }

        void Update()
        {
            if (rpcManager == null)
            {
                return;
            }

            if (Input.GetMouseButton(0))
            {
                rpcManager.UseItemInHandRPC();
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
