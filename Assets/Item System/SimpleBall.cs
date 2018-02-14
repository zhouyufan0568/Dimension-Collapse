using System.Collections;
using UnityEngine;

namespace DimensionCollapse
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class SimpleBall : MonoBehaviour
    {
        [ReadOnlyInInspector]
        public float damage;

        public ParticleSystem hitViewEffect;

        private new Collider collider;
        private new Rigidbody rigidbody;

        private bool IsAggressive;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

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

            GameObject victim = collision.gameObject;
            if (victim.CompareTag("Player"))
            {
                victim.GetComponent<PlayerManager>().OnAttacked(damage);
            }
            GetComponent<ParticleAutoFadeOut>()?.FadeOut();
        }
    }
}
