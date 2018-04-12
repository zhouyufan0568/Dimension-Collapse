using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public class RangedWeaponChanger : Weapon
    {
        public BulletCharge TheBullet;  //武器所使用的子弹
        public float InitialV; //发射子弹的初始速度:决定射程
        public float Interval; //两次射击间隔：决定射速
        public int damage; //每发子弹的伤害值，这里为初始伤害
        private int damageThis; //这发子弹的实际伤害
        public int CurrentChanger; //武器正在使用的子弹数量
        public int CurrentChangerCapacity; //武器正在使用的子弹最大容量
        public int AlternativeCharger; //武器备用子弹数量
        public int AlternativeChargerCapacity; //武器备用子弹最大容量
        private LinkedList<BulletCharge> bulletList = new LinkedList<BulletCharge>();  //武器子弹列表，先生成同一时间能存在的最大数量子弹，供发射时调用
        private float currentInterval = float.MaxValue; //上一次射击距离现在的时间，用于实现武器射速限制

        public Camera m_Camera; //当前使用的相机，用于将子弹方向对准屏幕中心
        private RaycastHit hitInfo; //将子弹方向对准屏幕中心辅助对象
        private Transform gunpoint; //空物体：枪口,子弹实际上从此点进行发射
        private Vector3 force; //将子弹方向对准屏幕中心辅助对象 
        private AudioSource Audio_Shoot; //枪声
        private ParticleSystem ShellParticle; //弹出弹壳特效
        private ParticleSystem FlashParticle; //枪口闪光特效
        private AudioSource audioCharging;   //充能音效
        private bool canPlayAudio; // 充能可以播放，一次充能只会播放一次音效 
        public int damageDelta; //每0.5秒伤害增量，蓄力情况下会根据这个量增加伤害

        public int maxDamage; //最大伤害量

        private float chargingTime = 0; //已经充能的时间

        private BulletCharge currentBullet = null;

        private void Awake()
        {
            initThisGun();  //进行初始化
        }
        private void initThisGun()
        {
            try
            {
                initBulletList();     //先生成同一时间能存在的最大数量子弹的对象池，供发射时调用
            }
            catch (System.NullReferenceException e)
            {
                Debug.Log("此武器：" + name + " 没有绑定子弹！" + e);
            }

            try
            {
                //获取枪口位置，以便发射子弹
                gunpoint = this.transform.Find("Empty_Gunpoint").gameObject.GetComponent<Transform>();
            }
            catch (System.NullReferenceException e)
            {
                Debug.Log("此武器：" + name + " 不含Empty_Gunpoint,也即缺少空物体：枪口。 " + e);
            }

            try
            {
                Audio_Shoot = this.transform.Find("Empty_Gunpoint").gameObject.GetComponent<AudioSource>();
                if (Audio_Shoot == null)
                {
                    throw new System.NullReferenceException();
                }
            }
            catch (System.NullReferenceException e)
            {
                Debug.Log("此武器：" + name + " 不含Audio_Shoot,也即缺少枪声文件。 " + e);
            }

            try
            {
                ShellParticle = this.transform.Find("Particle_Shell").gameObject.GetComponent<ParticleSystem>();
            }
            catch (System.NullReferenceException e)
            {
                Debug.Log("此武器：" + name + " 不含ShellParticle，也即没有弹壳弹出特效。 " + e);
            }

            try
            {
                FlashParticle = this.transform.Find("Particle_Flash").gameObject.GetComponent<ParticleSystem>();
            }
            catch (System.NullReferenceException e)
            {
                Debug.Log("此武器：" + name + " 不含FlashParticle，也即没有枪口闪光特效。 " + e);
            }
            audioCharging = this.GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (this.currentBullet != null && this.currentBullet.isCharging && this.currentBullet.isActiveAndEnabled)
            {
                // Debug.Log("charing!");
                this.currentBullet.setInitTransformAndDamageForCharge(gunpoint, this.damageThis, true, damage);
                WeaponCharging();
            }

            if (currentInterval == float.MaxValue)
            {
                return;
            }
            else if (currentInterval < Interval)
            {
                currentInterval += Time.deltaTime;
                return;
            }
            currentInterval = float.MaxValue;


        }

        public override void Attack(bool charging)
        {
            // 正在充能的话，退出，因为update里已经进行相关操作，除非发射指令来，就把它发射出去
            if (this.currentBullet != null)
            {
                if (charging)
                {
                    return;
                }
                else
                {
                    chargedAction();
                    return;
                }
            }
            // 只有为true时才能进入发射流程
            if (!charging)
            {
                return;
            }
            //当前间隔大于武器射速间隔时，才进行射击
            if (CanAttack())
            {
                //且现有子弹大于0
                if (CurrentChanger > 0)
                {
                    charingAction();
                }
                else //否则重装子弹
                {
                    reload();
                }
            }
        }

        private void charingAction()
        {
            // Debug.Log("charingAction");
            currentInterval = 0;

            CurrentChanger--;
            //Debug.Log(CurrentChanger);


            BulletCharge currentBullet;
            currentBullet = bulletList.First.Value;
            this.currentBullet = currentBullet;

            BulletCharge tempBullet = currentBullet;
            bulletList.RemoveFirst();
            bulletList.AddLast(tempBullet);
            canPlayAudio = true;
            //尚未完成充能，进入充能增加伤害的阶段
            currentBullet.setInitTransformAndDamageForCharge(gunpoint, damageThis, true, damage);
            WeaponCharging();
            // Debug.Log("charging = true! hash:" + (this.currentBullet.GetHashCode() == currentBullet.GetHashCode()));
        }
        private void chargedAction()
        {
            // Debug.Log("chargedAction");

            /*-------------------------------------------朝向准心发射代码----------------------------------------------*/
            //  m_Camera = Camera.main;
            //if (m_Camera == null) { Debug.Log("null!!!!"); }-
            //通过摄像机在屏幕中心点位置发射一条射线  
            Ray ray = m_Camera.ScreenPointToRay(new Vector3(Screen.width >> 1, Screen.height >> 1, 0));
            if (Physics.Raycast(ray, out hitInfo, 1000))//如果射线碰撞到物体  
            {
                force = hitInfo.point;//记录碰撞的目标点的方向  

                force -= gunpoint.position;

                force.Normalize();
                force *= InitialV;
            }
            else
            {
                //将目标点设置在摄像机自身前方1000米处  
                force = ray.direction;


                force.Normalize();
                force *= InitialV;
            }
            /*---------------------------------------------------------------------------------------------------------*/

            gunpoint.LookAt(force);
            // Debug.DrawLine(gunpoint.position, force, Color.red, 20000, false);

            //Debug.Log("charging = false! hash:" + (this.currentBullet.GetHashCode() == currentBullet.GetHashCode()));
            currentBullet.setInitTransformAndDamageForCharge(gunpoint, damageThis, false, damage);
            this.chargingTime = 0f;

            /*-------------------------------------------朝向准心发射代码----------------------------------------------*/

            //Debug.DrawRay(gunpoint.position,force,Color.green, 20000, false);
            /*---------------------------------------------------------------------------------------------------------*/

            //   Vector3 force = gunpoint.forward * InitialV;
            // 发射
            currentBullet.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);

            audioCharging.Stop();
            canPlayAudio = true;
            //播放声音
            if (Audio_Shoot != null)
            {
                Audio_Shoot.Play();
            }
            //播放弹壳弹出特效
            if (ShellParticle != null)
            {
                ShellParticle.Play();
            }
            //播放枪口闪光特效
            if (FlashParticle != null)
            {
                FlashParticle.Play();
            }


            //Debug.Log("addlast");
            this.currentBullet = null;
            this.damageThis = 0;

            //Debug.Log("子弹发射！");
        }
        private void WeaponCharging()
        {
            if (!audioCharging.isPlaying && canPlayAudio)
            {
                canPlayAudio = false;
                audioCharging.Play();
            }
            this.chargingTime += Time.deltaTime;
            this.damageThis = (int)Math.Round(chargingTime * this.damageDelta * 2 + this.damage);
            if (this.damageThis >= this.maxDamage)
            {
                this.damageThis = this.maxDamage;
            }
        }

        private void initBulletList()
        {
            TheBullet.gameObject.SetActive(true);   //此行代码不加的话，在子弹的Prefab激活栏中不打勾时，就会出现获取不到子弹生命周期的情况，原因是Prefab不打勾不会在Instantiate后调用Awake()！

            GameObject bulletListParent = new GameObject(this.TheBullet.name + "sList");
            BulletCharge newBullet = Instantiate(TheBullet, bulletListParent.GetComponent<Transform>(), true) as BulletCharge;
            // bulletList.AddLast(newBullet);
            // newBullet.setBulletList(bulletList);

            float bulletRealLifeTime = newBullet.getBulletRealLifeTime();
            //Debug.Log("RangedWeapon:" + bulletRealLifeTime);
            for (int i = 0; i < bulletRealLifeTime / Interval; i++)
            {
                newBullet = Instantiate(TheBullet, bulletListParent.GetComponent<Transform>(), true) as BulletCharge;
                bulletList.AddLast(newBullet);
                newBullet.setBulletList(bulletList);
                //Debug.Log("i = " + i);
            }
        }
        //换弹
        public void reload()
        {
            //如果现有弹夹没子弹了
            if (CurrentChanger == 0)
            {
                //检查备用弹夹有没有子弹
                if (AlternativeCharger > 0)
                {
                    //播放换弹动画，等待换弹间隔     
                    {

                    }

                    //备用弹夹有子弹，进行装弹
                    int temp = AlternativeCharger;
                    AlternativeCharger -= CurrentChangerCapacity;
                    //有可能备用子弹数少于当前弹夹最大容量，所以直接减到负值，这时候把它归零
                    if (AlternativeCharger < 0)
                    {
                        AlternativeCharger = 0;
                        CurrentChanger += temp;
                    }
                    else
                    {
                        CurrentChanger += CurrentChangerCapacity;
                    }
                }
            }
        }

        //临时代码
        public void newReload()
        {
            this.AlternativeCharger = this.AlternativeChargerCapacity;
        }

        /// <summary>
        /// Change the camera used for shooting.
        /// Add by SWT.
        /// </summary>
        public override void OnPickedUp(PlayerManager playerManager)
        {
            chargingTime = 0; //已经充能的时间
            currentBullet = null;

            m_Camera = playerManager.camera;
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
            this.chargingTime = 0;
            Picked = true;
        }

        /// <summary>
        /// Set the camera to null when thrown.
        /// Add by SWT.
        /// </summary>
        public override void OnThrown()
        {
            m_Camera = null;
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }
            ItemUtils.FreezeRigidbodyWithoutPositionY(GetComponent<Rigidbody>());
            Picked = false;
        }

        public override bool CanAttack()
        {
            return currentInterval > Interval;
        }
        public override void Attack()
        {
            Debug.LogWarning("Charge weapon should run charge type attack, it should has two parameter");
        }
    }
}
