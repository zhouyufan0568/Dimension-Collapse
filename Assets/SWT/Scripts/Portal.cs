using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DimensionCollapse;

namespace DimensionCollapse
{
    public class Portal : AbstractNonDirectiveSkill
    {
        public float cooldownTimeTotal = 10f;
        private float[] cooldownTimeLeft;

        public GameObject protalPrefab;
        private GameObject[] doorObjs;
        private PortalDoor[] portalDoors;
        private int whichDoorNext;

        private PlayerManager playerManager;

        public float offsetInZ = 2f;

        private void Start()
        {
            cooldownTimeLeft = new float[2];
            doorObjs = new GameObject[2];
            portalDoors = new PortalDoor[2];

            InstantiateDoor(0);
            InstantiateDoor(1);

            whichDoorNext = 0;
        }

        public override float CooldownTimeLeft
        {
            get
            {
                return Mathf.Min(cooldownTimeLeft);
            }
        }

        public override float CooldownTimeTotal
        {
            get
            {
                return cooldownTimeTotal;
            }
        }

        public override void CastCore()
        {
            playerManager = ItemUtils.ObtainPlayerManager(gameObject);
            if (playerManager == null)
            {
                Debug.Log("Skill is casted when not attached to any player.");
                return;
            }

            OpenNextDoor();
        }

        private void InstantiateDoor(int index)
        {
            cooldownTimeLeft[index] = 0f;

            doorObjs[index] = Instantiate(protalPrefab);
            portalDoors[index] = doorObjs[index].GetComponent<PortalDoor>();
            if (portalDoors[index] == null)
            {
                portalDoors[index] = doorObjs[index].AddComponent<PortalDoor>();
            }
            portalDoors[index].doorIndex = index;
            portalDoors[index].portalManager = this;
            doorObjs[index].SetActive(false);
        }

        private void OpenNextDoor()
        {
            cooldownTimeLeft[whichDoorNext] = CooldownTimeTotal;
            doorObjs[whichDoorNext].SetActive(true);
            doorObjs[whichDoorNext].transform.position = playerManager.transform.position + playerManager.transform.forward * offsetInZ;
            doorObjs[whichDoorNext].transform.rotation = playerManager.transform.rotation;
            StartCoroutine(SetDoorInactiveAfter(whichDoorNext));

            whichDoorNext = 1 - whichDoorNext;
        }

        public void MoveObjectFrom(GameObject obj, int fromIndex)
        {
            if (!obj.CompareTag("Player"))
            {
                return;
            }

            int toIndex = 1 - fromIndex;
            if (doorObjs[toIndex].activeInHierarchy)
            {
                obj.transform.position = doorObjs[toIndex].transform.position + doorObjs[toIndex].transform.forward * offsetInZ;
                obj.transform.rotation = doorObjs[toIndex].transform.rotation;
            }
        }

        private void UpdateDoorState(int index)
        {
            if (doorObjs[index].activeInHierarchy)
            {
                cooldownTimeLeft[index] = Mathf.Clamp(cooldownTimeLeft[index] - Time.deltaTime, 0, CooldownTimeLeft);
            }
        }

        private IEnumerator SetDoorInactiveAfter(int index)
        {
            yield return new WaitForSeconds(cooldownTimeTotal);
            doorObjs[index].SetActive(false);
        }

        private void Update()
        {
            UpdateDoorState(0);
            UpdateDoorState(1);
        }
    }
}
