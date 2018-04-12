using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public class SpeedController : MonoBehaviour
    {
        private Animator animator;
        private Player player;

        private void Start()
        {
            animator = GetComponent<Animator>();
            player = GetComponent<Player>();
        }

        private void Update()
        {
            if (animator.GetBool("isStand"))
            {
                player.m_WalkSpeed = 2f;
                player.m_RunSpeed = 4f;
            }
            else if (animator.GetBool("isCrouch"))
            {
                player.m_WalkSpeed = 0.8f;
                player.m_RunSpeed = 1.2f;
            }
            else if (animator.GetBool("isProne"))
            {
                player.m_WalkSpeed = 0.3f;
                player.m_RunSpeed = 0.5f;
            }
        }
    }
}
