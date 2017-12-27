using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public class SpaceExchangeBallTest : MonoBehaviour
    {
        private SpaceExchangeBall spaceExchangeBall;

        private void Start()
        {
            spaceExchangeBall = GetComponent<SpaceExchangeBall>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                spaceExchangeBall.Cast();
            }
        }
    }
}