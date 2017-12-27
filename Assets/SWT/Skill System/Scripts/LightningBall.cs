using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public class LightningBall : AbstractNonDirectiveSkill
    {
        public float cooldownTimeTotal = 12f;
        private float cooldownTimeLeft = 0f;

        public int ballNumOneShot = 8;

        public GameObject ball;

        public float lifetime = 8f;

        private PlayerManager playerManager;

        public override float CooldownTimeLeft
        {
            get
            {
                return cooldownTimeLeft;
            }
        }

        public override float CooldownTimeTotal
        {
            get
            {
                return cooldownTimeTotal;
            }
        }

        public override void CastCore()
        {
            playerManager = ItemUtils.ObtainPlayerManager(gameObject);
            if (playerManager == null)
            {
                Debug.Log("Skill is casted when not attached to any player.");
                return;
            }

            float angleBetweenTwoBall = 180f / ballNumOneShot;
            for (float curAngle = -90f; curAngle <= 90f ; curAngle += angleBetweenTwoBall)
            {
                GameObject newBall = Instantiate(
                    ball,
                    playerManager.transform.position + playerManager.transform.forward * 5f,
                    playerManager.camera.transform.rotation * Quaternion.AngleAxis(curAngle, playerManager.camera.transform.up)
                    );
                LightningBallObj ballObj = newBall.GetComponent<LightningBallObj>();
                ballObj.owner = playerManager.gameObject;
                Destroy(newBall, lifetime);
            }
        }
    }
}
