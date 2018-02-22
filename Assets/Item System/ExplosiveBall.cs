using UnityEngine;

namespace DimensionCollapse
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class ExplosiveBall : MonoBehaviour
    {
        [ReadOnlyInInspector]
        public float centerDamage;

        public float effectiveRadius = 5f;

        public float impactForce = 0f;

        public ParticleSystem hitViewEffect;

        private new Collider collider;

        private bool IsAggressive;

        private void OnEnable()
        {
            IsAggressive = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!IsAggressive)
            {
                return;
            }
            IsAggressive = false;

            if (hitViewEffect != null)
            {
                ParticleSystem particle = Instantiate(hitViewEffect, transform.position, transform.rotation);
            }

            OnExplode();

            GetComponent<ParticleAutoFadeOut>()?.FadeOut();
        }

        public virtual void OnExplode()
        {
            Vector3 explosionPos = transform.position;
            Collider[] victims = Physics.OverlapSphere(explosionPos, effectiveRadius, LayerMask.GetMask("Default", "Player"));
            foreach (var victim in victims)
            {
                if (ItemUtils.IsPlayer(victim.gameObject))
                {
                    PlayerManager playerManager = victim.GetComponent<PlayerManager>();
                    playerManager.OnAttacked(Mathf.Lerp(centerDamage, 0, Vector3.Distance(victim.transform.position, explosionPos) / effectiveRadius));
                    playerManager.AddImpact(victim.transform.position - explosionPos, impactForce);
                }
                else
                {
                    victim.GetComponent<Rigidbody>()?.AddExplosionForce(impactForce, explosionPos, effectiveRadius, 2);
                }
            }
        }
    }
}