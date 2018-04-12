using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public class IKManager : MonoBehaviour
    {
        public WeaponIKScriptable weaponIKData;

        private IkSetting ikSetting;
        private PlayerManager playerManager;

        private Dictionary<int, int> weaponIdToIndex;

        private void Awake()
        {
            ikSetting = GetComponentInChildren<IkSetting>();
            playerManager = GetComponent<PlayerManager>();

            weaponIdToIndex = new Dictionary<int, int>();
            List<int> weaponIds = weaponIKData.weaponIds;
            for (int i = 0; i < weaponIds.Capacity; i++)
            {
                weaponIdToIndex.Add(weaponIds[i], i);
            }
        }

        public void ChangeIKObjs()
        {
            Item itemInHand = playerManager.itemInHand;
            if (itemInHand == null)
            {
                return;
            }

            ikSetting.isActive = true;

            itemInHand.transform.localRotation = Quaternion.Euler(weaponIKData.weaponRotations[weaponIdToIndex[itemInHand.ID]]);
            itemInHand.transform.localScale = weaponIKData.weaponScales[weaponIdToIndex[itemInHand.ID]];
            itemInHand.transform.localPosition = weaponIKData.weaponPositions[weaponIdToIndex[itemInHand.ID]];

            Transform leftHandObj = itemInHand.transform.Find("lefthand");
            Transform rightHandObj = itemInHand.transform.Find("righthand");
            ikSetting.leftHandObj = leftHandObj;
            ikSetting.rightHandObj = rightHandObj;
        }
    }
}

