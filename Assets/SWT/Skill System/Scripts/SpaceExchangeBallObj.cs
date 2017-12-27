using UnityEngine;

namespace DimensionCollapse
{
    public class SpaceExchangeBallObj : MonoBehaviour
    {
        [HideInInspector]
        public SpaceExchangeBall manager;

        [HideInInspector]
        public GameObject owner;

        public float velocity = 5f;

        public GameObject transformEffect;
        private float transformEffectOffsetY = 1f;
        private float transformEffectLifetime = 2f;

        private void Update()
        {
            transform.position += transform.forward * (velocity * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (owner != null && other.gameObject.CompareTag("Player") && owner != other.gameObject)
            {
                GameObject poorGay = other.gameObject;

                Vector3 temp = owner.transform.position;
                owner.transform.position = poorGay.transform.position;
                poorGay.transform.position = temp;

                if (transformEffect != null)
                {
                    GameObject effect1 = Instantiate(transformEffect, owner.transform.position + Vector3.up * transformEffectOffsetY, Quaternion.identity);
                    GameObject effect2 = Instantiate(transformEffect, poorGay.transform.position + Vector3.up * transformEffectOffsetY, Quaternion.identity);
                    Destroy(effect1, transformEffectLifetime);
                    Destroy(effect2, transformEffectLifetime);
                }

                Destroy(gameObject);
            }
        }
    }
}
