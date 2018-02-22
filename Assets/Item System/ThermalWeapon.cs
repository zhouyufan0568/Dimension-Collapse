using System;
using System.Collections;
using UnityEngine;

namespace DimensionCollapse
{
    public class ThermalWeapon : Weapon
    {
        [Tooltip("Unlimited Shoot")]
        public bool UnlimitFire = false;
        [Tooltip("The bullet to be used")]
        public GameObject bulletPrefab;
        [Tooltip("The number of the bullets loaded.")]
        [ReadOnlyInInspector]
        public int bulletInMagazine = 0;
        [Tooltip("The maximun number of the bullets that can be loaded.")]
        public int magazineCapacity = 30;
        [Tooltip("The number of the remaining bullets")]
        public int bulletStoreLeft = 100;
        [Tooltip("The maximun number of the bullets that can be carried.")]
        public int bulletStoreLimit = 300;
        [Tooltip("The duration that the bullet can fly")]
        public float bulletLifeTime = 3;
        [Tooltip("The maximun flying speed of the bullet")]
        public float bulletSpeed = 100f;
        [Tooltip("The damage that one bullet can cause.")]
        public float damage = 10f;

        [Tooltip("The sound effect to be played when shooting.")]
        public AudioSource shootSoundEffect;
        [Tooltip("The sound effect to be player when reloading.")]
        public AudioSource reloadSoundEffect;
        [Tooltip("The particle effect to be played when shooting.")]
        public ParticleSystem shootViewEffect;

        [Tooltip("The point where bullets actually come out.")]
        public Transform gunpoint;

        public bool singleShootMode = true;
        public float singleShootInterval = 0.2f;

        public bool burstShootMode = false;
        public float intervalBetweenBurst = 1f;
        public float intervalBetweenShoot = 0.1f;
        public int bulletShootInSingleBurst = 3;

        private Camera ownerCamera;
        private Vector3 screenCenter;
        private LayerMask raycastLayerMask;
        private RaycastHit hitInfo;
        private float effectiveRange;

        private IShootStrategy[] shootStrategies;
        private int strategyUsingIndex;

        private PlayerManager ownerPlayerManager;

        private ObjectPoolManager.ObjectPool bulletPool;

        private void Awake()
        {
            screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            raycastLayerMask = LayerMask.GetMask("Map", "Player");

            bulletInMagazine = magazineCapacity;
            bulletPool = ObjectPoolManager.INSTANCE.GetObjectPool(bulletPrefab, magazineCapacity + 10, false);
            effectiveRange = bulletSpeed * bulletLifeTime;

            int strategyCount = 0;
            strategyCount += singleShootMode ? 1 : 0;
            strategyCount += burstShootMode ? 1 : 0;
            shootStrategies = new IShootStrategy[strategyCount];
            int index = 0;
            if (singleShootMode)
            {
                shootStrategies[index++] = new SingleShootStrategy(this, singleShootInterval);
            }
            if (burstShootMode)
            {
                shootStrategies[index++] = new BurstShootStrategy(this, intervalBetweenBurst, intervalBetweenShoot, bulletShootInSingleBurst);
            }
            strategyUsingIndex = 0;
        }

        public override void Attack()
        {
            shootStrategies[strategyUsingIndex].Fire();
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
            ObjectPoolManager.INSTANCE.FillPool(bulletPool);
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
            ObjectPoolManager.INSTANCE.RecyclePool(bulletPool);
        }

        public string ModeUsing
        {
            get
            {
                IShootStrategy strategy = shootStrategies[strategyUsingIndex];
                if (strategy is SingleShootStrategy)
                {
                    return "Single";
                }
                if (strategy is BurstShootStrategy)
                {
                    return "Burst";
                }
                return "";
            }
        }

        public void Reload(Action whenEmpty = null, Action whenReloadSucceed = null)
        {
            if (!Picked)
            {
                Debug.Log("Error: try to reload a gun that haven't been picked up.");
                return;
            }

            if (bulletStoreLeft <= 0)
            {
                whenEmpty?.Invoke();
                return;
            }

            if (bulletInMagazine < magazineCapacity)
            {
                int shortage = magazineCapacity - bulletInMagazine;
                int toBeReloaded = bulletStoreLeft >= shortage ? shortage : bulletStoreLeft;
                bulletInMagazine += toBeReloaded;
                bulletStoreLeft -= toBeReloaded;
                ItemUtils.Play(reloadSoundEffect);
                whenReloadSucceed?.Invoke();
            }
        }

        public bool IsMagazineEmpty()
        {
            return bulletInMagazine <= 0 ? true : false;
        }

        public virtual void ShootSingleBullet()
        {
            if (!IsWeaponValid())
            {
                return;
            }

            if (!UnlimitFire && IsMagazineEmpty())
            {
                return;
            }

            Ray raycastDir = ownerCamera.ScreenPointToRay(screenCenter);
            Debug.DrawRay(gunpoint.position, raycastDir.direction * 1000);
            Vector3 bulletDir = Vector3.zero;
            if (Physics.Raycast(raycastDir, out hitInfo, effectiveRange, raycastLayerMask))
            {
                bulletDir = (hitInfo.point - gunpoint.transform.position).normalized;
            }
            else
            {
                bulletDir = ownerCamera.transform.forward;
            }

            GameObject bullet = bulletPool.Next(false);
            bullet.transform.position = gunpoint.position;
            bullet.transform.rotation = Quaternion.LookRotation(bulletDir);
            SimpleBullet simpleBullet = bullet.GetComponent<SimpleBullet>();
            if (simpleBullet != null)
            {
                simpleBullet.damage = damage;
            }
            SimpleBall simpleBall = bullet.GetComponent<SimpleBall>();
            if (simpleBall != null)
            {
                simpleBall.damage = damage;
            }
            ExplosiveBall explosiveBall = bullet.GetComponent<ExplosiveBall>();
            if (explosiveBall != null)
            {
                explosiveBall.centerDamage = damage;
            }
            Rigidbody rigidbody = bullet.GetComponent<Rigidbody>();
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            bullet.SetActive(true);
            rigidbody.AddForce(bullet.transform.forward * bulletSpeed, ForceMode.VelocityChange);
            bulletInMagazine--;

            ItemUtils.Play(shootSoundEffect);
            ItemUtils.Play(shootViewEffect, true);

            StartCoroutine(SendBackBullet(bullet));
        }

        private bool IsWeaponValid()
        {
            if (bulletPrefab == null)
            {
                Debug.Log(name + ": Bullet prefab is missing.");
                return false;
            }
            if (gunpoint == null)
            {
                Debug.Log(name + ": Gunpoint is missing.");
                return false;
            }
            if (bulletPool.IsEmpty)
            {
                Debug.Log(name + ": Bullet pool is empty.");
                return false;
            }
            return true;
        }

        private IEnumerator SendBackBullet(GameObject bullet)
        {
            yield return new WaitForSeconds(bulletLifeTime);
            if (bullet.activeInHierarchy)
            {
                bullet.SetActive(false);
            }
        }

        public override bool CanAttack()
        {
            return shootStrategies[strategyUsingIndex].CanShoot();
        }

        private interface IShootStrategy
        {
            bool CanShoot();
            void Fire();
        }

        private class SingleShootStrategy : IShootStrategy
        {
            private ThermalWeapon weapon;
            private float interval;
            private float lastShootTime;
            public SingleShootStrategy(ThermalWeapon weapon, float interval)
            {
                this.weapon = weapon;
                this.interval = interval;
                lastShootTime = -interval;
            }

            public bool CanShoot()
            {
                float currentTime = Time.time;
                return currentTime - lastShootTime >= interval;
            }

            public void Fire()
            {
                if (CanShoot())
                {
                    lastShootTime = Time.time;
                    weapon.ShootSingleBullet();
                }
            }
        }

        private class BurstShootStrategy : IShootStrategy
        {
            private ThermalWeapon weapon;
            private float intervalBetweenBurst;
            private float intervalBetweenShoot;
            private int bulletShootInSingleBurst;
            private float lastBurstTime;
            public BurstShootStrategy(ThermalWeapon weapon, float intervalBetweenBurst, float intervalBetweenShoot, int bulletShootInSingleBurst)
            {
                this.weapon = weapon;
                this.intervalBetweenBurst = intervalBetweenBurst;
                this.intervalBetweenShoot = intervalBetweenShoot;
                this.bulletShootInSingleBurst = bulletShootInSingleBurst;
                lastBurstTime = -intervalBetweenBurst;
            }

            public bool CanShoot()
            {
                float currentTime = Time.time;
                return currentTime - lastBurstTime >= intervalBetweenBurst;
            }

            public void Fire()
            {
                if (CanShoot())
                {
                    lastBurstTime = Time.time;
                    weapon.StartCoroutine(Burst());
                }
            }

            private IEnumerator Burst()
            {
                weapon.ShootSingleBullet();
                for (int i = 0; i < bulletShootInSingleBurst - 1; i++)
                {
                    yield return new WaitForSeconds(intervalBetweenShoot);
                    weapon.ShootSingleBullet();
                }
            }
        }
    }
}
