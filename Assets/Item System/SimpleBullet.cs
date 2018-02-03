using UnityEngine;

namespace DimensionCollapse
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class SimpleBullet : MonoBehaviour
    {
        public AudioClip hitSoundEffect;
        public ParticleSystem hitViewEffect;
        [ReadOnlyInInspector]
        public float damage;
        [HideInInspector]
        public PlayerManager ownerPlayerManager;

        private new Rigidbody rigidbody;
        private Vector3 lastFramePos;
        private int raycastLayerMask;

        private bool isAggressive;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            raycastLayerMask = LayerMask.GetMask("Map", "Player");
        }

        private void OnEnable()
        {
            rigidbody.velocity = Vector3.zero;
            lastFramePos = transform.position;
            isAggressive = true;
        }

        private void LateUpdate()
        {
            if (!isAggressive)
            {
                return;
            }

            Vector3 vector = transform.position - lastFramePos;
            RaycastHit hitInfo = default(RaycastHit);
            if (Physics.Raycast(lastFramePos, vector.normalized, out hitInfo, vector.sqrMagnitude, raycastLayerMask))
            {
                OnCollisionEnterCore(hitInfo.transform.gameObject, hitInfo.point);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            OnCollisionEnterCore(collision.gameObject, transform.position);
        }

        private void OnCollisionEnterCore(GameObject victim, Vector3 point)
        {
            if (victim.CompareTag("Player"))
            {
                victim.GetComponentInChildren<PlayerManager>()?.OnAttacked((int)damage);
                gameObject.SetActive(false);
            }
            else
            {
                isAggressive = false;
            }
        }
    }
}

