using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DimensionCollapse;

namespace DimensionCollapse
{
    public class Shoot:MonoBehaviour
    {
        public Weapon weapon;

        [PunRPC]
        void ShootCore()
        {
            if (weapon != null)
            {
                weapon.Attack();
            }
               
        }
    }
}