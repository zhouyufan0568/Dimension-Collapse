using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public class FrameFragment : MonoBehaviour
    {
        [ReadOnlyInInspector]
        public Molotov molotov;

        private void Update()
        {
            transform.rotation = Quaternion.identity;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                molotov.victims.Add(other.gameObject.GetComponent<PlayerManager>());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                molotov.victims.Remove(other.gameObject.GetComponent<PlayerManager>());
            }
        }
    }
}

