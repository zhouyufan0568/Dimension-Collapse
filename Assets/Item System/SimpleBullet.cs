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

        private void FixedUpdate()
        {
            if (!isAggressive)
            {
                return;
            }

            Vector3 dir = transform.forward;
            RaycastHit[] hitInfos = Physics.RaycastAll(lastFramePos, dir.normalized, rigidbody.velocity.sqrMagnitude * Time.fixedDeltaTime, raycastLayerMask);
            int minDistIndex = -1;
            for (int i = 0; i < hitInfos.Length; i++)
            {
                if (minDistIndex == -1 || hitInfos[i].distance < hitInfos[minDistIndex].distance)
                {
                    minDistIndex = i;
                }
            }
            if (minDistIndex != -1)
            {
                OnCollisionEnterCore(hitInfos[minDistIndex].transform.gameObject, hitInfos[minDistIndex].point);
            }

            lastFramePos = transform.position;
        }

        //private void FixedUpdate()
        //{
        //    if (!isAggressive)
        //    {
        //        return;
        //    }

        //    //Debug.Log(lastFramePos + "\n" + transform.position);
        //    Vector3 vector = transform.position - lastFramePos;
        //    if (vector == Vector3.zero)
        //    {
        //        return;
        //    }
        //    RaycastHit[] hitInfos = Physics.RaycastAll(lastFramePos, vector.normalized, vector.sqrMagnitude, raycastLayerMask);
        //    int minDistIndex = -1;
        //    for (int i = 0; i < hitInfos.Length; i++)
        //    {
        //        if (minDistIndex == -1 || hitInfos[i].distance < hitInfos[minDistIndex].distance)
        //        {
        //            minDistIndex = i;
        //        }
        //    }
        //    if (minDistIndex != -1)
        //    {
        //        Debug.Log("here");
        //        OnCollisionEnterCore(hitInfos[minDistIndex].transform.gameObject, hitInfos[minDistIndex].normal);
        //    }

        //    lastFramePos = transform.position;
        //}

        private void OnCollisionEnter(Collision collision)
        {
            if (!isAggressive)
            {
                return;
            }

            OnCollisionEnterCore(collision.gameObject);
        }

        private void OnCollisionEnterCore(GameObject victim)
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
                rigidbody.velocity = Vector3.zero;
                transform.position = Vector3.Lerp(point, transform.position, 0.15f);
            }
        }
    }
}

