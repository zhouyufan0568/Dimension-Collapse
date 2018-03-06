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
        public GameObject[] equipedWeapon;
        public Skill[] skills;

        public Dictionary<int, Sprite> ItemSprite=new Dictionary<int,Sprite>();

        void Awake() {
            initItemSprite();
        }

        // Use this for initialization
        void Start()
        {
            playerManager = PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>();
            itemList = playerManager.inventory.ItemList;
            equipedWeapon = playerManager.inventory.EquipedWeapon;
            skills = playerManager.inventory.skills;
        }

        // Update is called once per frame
        void Update()
        {
			Debug.Log ("itemList.Count:"+itemList.Count);
            for(int i=0;i<itemList.Count;i++) {
                backpackStore.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = GetSpriteByID(itemList[i].GetComponent<Item>().ID);
            }
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
    }
}
