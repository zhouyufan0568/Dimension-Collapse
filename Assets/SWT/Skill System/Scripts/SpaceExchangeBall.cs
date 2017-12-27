using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public class SpaceExchangeBall : AbstractNonDirectiveSkill
    {
        public float cooldownTimeTotal = 10f;
        private float cooldownTimeLeft = 0f;

        public GameObject ball;

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

            GameObject newBall = Instantiate(ball, playerManager.transform.position + playerManager.transform.forward * 2f, playerManager.camera.transform.rotation);
            SpaceExchangeBallObj ballObj = newBall.GetComponent<SpaceExchangeBallObj>();
            ballObj.sender = playerManager.gameObject;
            cooldownTimeLeft = CooldownTimeTotal;
        }

        private void Update()
        {
            if (cooldownTimeLeft > 0)
            {
                cooldownTimeLeft = Mathf.Clamp(cooldownTimeLeft - Time.deltaTime, 0, CooldownTimeTotal);
            }
        }
    }
}
