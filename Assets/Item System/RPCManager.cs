using UnityEngine;

namespace DimensionCollapse
{
    public class RPCManager : Photon.PunBehaviour
    {
        private PlayerManager playerManager;
        private void Start()
        {
             playerManager = GetComponent<PlayerManager>();
        }

        public void UseItemInHandRPC()
        {
            if (photonView.isMine)
            {
                photonView.RPC("UseItemInHand", PhotonTargets.All);
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

        [PunRPC]
        private void UseItemInHand()
        {
            Item item = playerManager.itemInHand;
            (item as Weapon)?.Attack();
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
    }
}
