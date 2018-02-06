using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public abstract class Missile : Item
    {
        public abstract void OnChargeStart();

        public abstract void OnCharge();

        public abstract Vector3 OnChargeEnd();

        public abstract void Throw(Vector3 force);
    }
}


