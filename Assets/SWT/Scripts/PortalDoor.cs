using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public class PortalDoor : MonoBehaviour
    {
        [HideInInspector]
        public int doorIndex;

        [HideInInspector]
        public Portal portalManager;

        private void OnTriggerEnter(Collider other)
        {
            portalManager.MoveObjectFrom(other.gameObject, doorIndex);
        }
    }
}
