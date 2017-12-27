using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public class LightningBallTest : MonoBehaviour
    {
        private LightningBall lightningBall;

        // Use this for initialization
        void Start()
        {
            lightningBall = GetComponent<LightningBall>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                lightningBall.Cast();
            }
        }
    }
}
