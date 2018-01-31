using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public abstract class Item : MonoBehaviour
    {
        public String Name; //物品名称
        public int ID; //物品ID
        public bool Picked; //标识符，物品被捡起为true，没有被捡起为false
    
        /// <summary>
        /// Called when the item is picked up.
        /// </summary>
        /// <param name="playerManager">The PlayerManager instance attached to the player who picks up the item.</param>
        public virtual void OnPickedUp(PlayerManager playerManager)
        {

        }

        /// <summary>
        /// Called when the item is thrown.
        /// </summary>
        public virtual void OnThrown()
        {

        }
    }
}
