using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse {
    public static class ItemUtils
    {
        public static PlayerManager ObtainPlayerManager(GameObject obj)
        {
            if (obj != null)
            {
                return obj.GetComponentInParent<PlayerManager>();
            }

            return null;
        }
    }
}
