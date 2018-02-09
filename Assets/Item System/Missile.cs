using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public abstract class Missile : Item
    {
        /// <summary>
        /// Invoke it when start charging;
        /// </summary>
        public abstract void OnChargeStart();

        /// <summary>
        /// Invoke it when charging;
        /// </summary>
        public abstract void OnCharge();

        /// <summary>
        /// Invoke it when end up charging to get the force to be added to the missile.
        /// </summary>
        /// <returns>Force to be added to the missile</returns>
        public abstract Vector3 OnChargeEnd();

        /// <summary>
        /// Throw the missile by the given force.
        /// </summary>
        /// <param name="force">Force. Including direction and magnitude.</param>
        public abstract void Throw(Vector3 force);
    }
}


