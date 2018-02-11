using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    [RequireComponent(typeof(LineRenderer))]
    public class SpaceGun : Weapon
    {
        public Transform gunpoint;
        public float effectiveRange = 150f;
        public float damagePerSecond = 30f;
        public AudioSource soundEffect;

        private PlayerManager ownerPlayerManager;
        private Camera ownerCamera;
        private LineRenderer lineRenderer;
        private LayerMask raycastLayerMask;
        private Vector3 screenCenter;

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
            screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            raycastLayerMask = LayerMask.GetMask("Map", "Player");
        }

        public override void Attack()
        {
            Ray raycastDir = ownerCamera.ScreenPointToRay(screenCenter);
            RaycastHit hitInfo;
            Vector3 lazerEnd = Vector3.zero;
            if (Physics.Raycast(raycastDir, out hitInfo, effectiveRange, raycastLayerMask))
            {
                lazerEnd = hitInfo.point;
                GameObject victim = hitInfo.collider.gameObject;
                if (victim.CompareTag("Player"))
                {
                    PlayerManager.playerToPlayerManager[victim].OnAttacked(damagePerSecond * Time.deltaTime);
                }
            }
            else
            {
                lazerEnd = raycastDir.origin + raycastDir.direction.normalized * effectiveRange;
            }

            lineRenderer.SetPosition(0, gunpoint.position);
            lineRenderer.SetPosition(1, lazerEnd);

            if (soundEffect != null)
            {
                soundEffect.Play();
            }
        }

        public void AttackEnd()
        {
            lineRenderer.SetPosition(0, Vector3.zero);
            lineRenderer.SetPosition(1, Vector3.zero);
        }

        public override void OnPickedUp(PlayerManager playerManager)
        {
            ownerPlayerManager = playerManager;
            ownerCamera = playerManager.camera;
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            Rigidbody[] rigidbodys = GetComponents<Rigidbody>();
            foreach (var rigidbody in rigidbodys)
            {
                ItemUtils.FreezeRigidbody(rigidbody);
            }
            Picked = true;
        }

        public override void OnThrown()
        {
            ownerPlayerManager = null;
            ownerCamera = null;
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }
            ItemUtils.FreezeRigidbodyWithoutPositionY(GetComponent<Rigidbody>());
            Picked = false;
        }
    }
}
