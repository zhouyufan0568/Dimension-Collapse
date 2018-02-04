using System;
using UnityEngine;

namespace DimensionCollapse
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class SimpleBullet : MonoBehaviour
    {
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
            lastFramePos = transform.position;
            isAggressive = true;
        }

        private void Update()
        {
            if (!isAggressive)
            {
                return;
            }
 
            lastFramePos = transform.position;
        }

        private void LateUpdate()
        {
            if (!isAggressive)
            {
                return;
            }

            Vector3 vector = transform.position - lastFramePos;
            RaycastHit[] hitInfos = Physics.RaycastAll(lastFramePos, vector.normalized, vector.sqrMagnitude, raycastLayerMask);
            GameObject victim = null;
            float minDist = float.MaxValue;
            foreach (var hitInfo in hitInfos)
            {
                if (hitInfo.distance < minDist)
                {
                    victim = hitInfo.transform.gameObject;
                    minDist = hitInfo.distance;
                }
            }
            if (victim != null)
            {
                OnCollisionEnterCore(victim, victim.transform.position);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!isAggressive)
            {
                return;
            }

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
                gameObject.transform.position = point;
            }
        }
    }
}

