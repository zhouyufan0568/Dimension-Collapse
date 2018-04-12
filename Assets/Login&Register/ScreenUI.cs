using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class ScreenUI : MonoBehaviour
    {
        public abstract void In();

        public abstract void Out();
    }
}
