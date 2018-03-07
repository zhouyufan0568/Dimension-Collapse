using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DimensionCollapse
{
    public class BackpackManager : MonoBehaviour
    {

        public GameObject swapTemp;
        public GameObject backpackStore;
        public GameObject equipBar;
        public GameObject skillBar;
        private PlayerManager playerManager;
        public List<GameObject> itemList;
        public GameObject[] Weapon;
        public GameObject[] Missile;
        public GameObject[] Remedy;
        public GameObject[] Skills;
        int lastcnt = 0;

        public Dictionary<int, Sprite> ItemSprite=new Dictionary<int,Sprite>();

        void Awake() {
            initItemSprite();
        }

        // Use this for initialization
        void Start()
        {
            playerManager = PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>();
            itemList = playerManager.inventory.ItemList;
            Weapon = playerManager.inventory.Weapon;
            Skills = playerManager.inventory.Skills;
            Missile = playerManager.inventory.Missile;
            Remedy = playerManager.inventory.Remedy;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateInspire();
        }

        void OnDisable()
        {
            if (swapTemp.transform.childCount != 0)
            {
                Destroy(swapTemp.transform.GetChild(0).gameObject);
            }
        }

        void initItemSprite() {
            ItemSprite.Add(3003, Resources.Load("ItemSprite/Weapon/FlowerWand", typeof(Sprite)) as Sprite);
			ItemSprite.Add(2001, Resources.Load("ItemSprite/Weapon/BombGrenade", typeof(Sprite)) as Sprite);
			ItemSprite.Add(7001, Resources.Load("ItemSprite/Weapon/M16A4", typeof(Sprite)) as Sprite);
			ItemSprite.Add(6001, Resources.Load("ItemSprite/Weapon/MAC11", typeof(Sprite)) as Sprite);
			ItemSprite.Add(2003, Resources.Load("ItemSprite/Weapon/MedicalGrenade", typeof(Sprite)) as Sprite);
			ItemSprite.Add(2002, Resources.Load("ItemSprite/Weapon/Molotov", typeof(Sprite)) as Sprite);
			ItemSprite.Add(6002, Resources.Load("ItemSprite/Weapon/MP5", typeof(Sprite)) as Sprite);
			ItemSprite.Add(8001, Resources.Load("ItemSprite/Weapon/SpaceGun", typeof(Sprite)) as Sprite);
			ItemSprite.Add(2, Resources.Load("ItemSprite/Weapon/SpaceExchangeBall", typeof(Sprite)) as Sprite);
			ItemSprite.Add(3002, Resources.Load("ItemSprite/Weapon/StarMagicWand", typeof(Sprite)) as Sprite);
			ItemSprite.Add(3001, Resources.Load("ItemSprite/Weapon/Wiley", typeof(Sprite)) as Sprite);
        }

        Sprite GetSpriteByID(int id) {
            return ItemSprite.ContainsKey(id) ? ItemSprite[id] : null;
        }

        /// <summary>
        /// update inspire of item
        /// </summary>
        /// <param name="st"></param>
        /// <param name="cnt"></param>
        /// <param name="obj"></param>
        private void UpdateInspire() {
            int cnt = 0;

            for (int i = 0; i < Weapon.Length; i++)
            {
                if (equipBar.transform.GetChild(cnt).GetChild(0).tag != "Untagged"&&Weapon[i] != null)
                {
                    equipBar.transform.GetChild(cnt).GetChild(0).GetComponent<Image>().sprite = GetSpriteByID(Weapon[i].GetComponent<Item>().ID);
                }
                else
                {
                    equipBar.transform.GetChild(cnt).GetChild(0).GetComponent<Image>().sprite = null;
                }
                cnt++;
            }

            for (int i = 0; i < Missile.Length; i++)
            {
                if (equipBar.transform.GetChild(cnt).GetChild(0).tag != "Untagged" && Missile[i] != null)
                {
                    equipBar.transform.GetChild(cnt).GetChild(0).GetComponent<Image>().sprite = GetSpriteByID(Missile[i].GetComponent<Item>().ID);
                }
                else
                {
                    equipBar.transform.GetChild(cnt).GetChild(0).GetComponent<Image>().sprite = null;
                }
                cnt++;
            }

            for (int i = 0; i < Remedy.Length; i++)
            {
                if (equipBar.transform.GetChild(cnt).GetChild(0).tag != "Untagged" && Remedy[i] != null)
                {
                    equipBar.transform.GetChild(cnt).GetChild(0).GetComponent<Image>().sprite = GetSpriteByID(Remedy[i].GetComponent<Item>().ID);
                }
                else
                {
                    equipBar.transform.GetChild(cnt).GetChild(0).GetComponent<Image>().sprite = null;
                }
                cnt++;
            }

            for (int i = 0; i < Skills.Length; i++) {
                if (skillBar.transform.GetChild(i).GetChild(0).tag != "Untagged" && Skills[i] != null)
                {
                    skillBar.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = GetSpriteByID(Skills[i].GetComponent<Item>().ID);
                }
                else
                {
                    skillBar.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = null;
                }
            }

            for (int i = 0; i < itemList.Count; i++)
            {
                if (backpackStore.transform.GetChild(i).transform.GetChild(0).tag != "Untagged")
                {
                    backpackStore.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = GetSpriteByID(itemList[i].GetComponent<Item>().ID);
                }
            }

            while (lastcnt >itemList.Count) {
                backpackStore.transform.GetChild(lastcnt-- - 1).transform.GetChild(0).GetComponent<Image>().sprite = null;
            }

            lastcnt = itemList.Count;
        }
    }
}
