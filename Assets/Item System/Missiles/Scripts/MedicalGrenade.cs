using UnityEngine;

namespace DimensionCollapse
{
    public class MedicalGrenade : AbstractMissile
    {
        public float effectiveRadius = 20f;
        public float centerHealing = 30;

        public override void ExplodeCore()
        {
            Vector3 explosionPos = transform.position;
            Collider[] victims = Physics.OverlapSphere(explosionPos, effectiveRadius, LayerMask.GetMask("Player"));
            foreach (var victim in victims)
            {
                if (ItemUtils.IsPlayer(victim.gameObject))
                {
                    PlayerManager playerManager = victim.GetComponent<PlayerManager>();
                    playerManager.OnHeal(centerHealing);
                }
            }
        }
    }
}
