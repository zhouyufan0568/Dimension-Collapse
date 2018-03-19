using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public abstract class Weapon : Item
    {
		public abstract void Attack();  //武器攻击函数，由人物来调用进行攻击

        public abstract bool CanAttack();

        public virtual void Attack(bool charging){ //充能武器攻击函数，charging代表是是否正在充能
            throw new System.NotImplementedException();
        }
    }
}