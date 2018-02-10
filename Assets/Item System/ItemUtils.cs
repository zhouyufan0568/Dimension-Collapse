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

        public static void Play(ParticleSystem particleSystem)
        {
            Play(particleSystem, false);
        }

        public static void Play(ParticleSystem particleSystem, bool withChildren)
        {
            if (particleSystem != null)
            {
                particleSystem.Play(withChildren);
            }
        }

        public static void Play(AudioSource audioSource)
        {
            Play(audioSource, 0);
        }

        public static void Play(AudioSource audioSource, ulong delay)
        {
            if (audioSource != null)
            {
                audioSource.Play(delay);
            }
        }

        public static bool IsMine(GameObject obj)
        {
            PhotonView photonView = obj.GetComponent<PhotonView>();
            return photonView != null ? photonView.isMine : false;
        }

        public static bool IsPlayer(GameObject obj)
        {
            return obj.CompareTag("Player");
        }
    }
}
