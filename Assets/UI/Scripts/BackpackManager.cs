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
            for(int i=0;i<itemList.Count;i++) {
                backpackStore.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = GetSpriteByID(itemList[i].GetComponent<Item>().ID);
            }
            for (int i = 0; i < equipedWeapon.Length; i++) {
                if (equipedWeapon[i] != null)
                {
                    equipBar.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = GetSpriteByID(equipedWeapon[i].GetComponent<Item>().ID);
                }
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
            ItemSprite.Add(3, Resources.Load("ItemSprite/Weapon/Wiley", typeof(Sprite)) as Sprite);
        }

        Sprite GetSpriteByID(int id) {
            return ItemSprite.ContainsKey(id) ? ItemSprite[id] : null;
        }
    }
}
