using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public class Bullet : MonoBehaviour
    {
        public String BulletName; //子弹的名称
        public int ID; //子弹的ID
        public float BulletLifeTime; //子弹的生命周期，超过这个时间的子弹，将进行删除
        private int damage;   //子弹的伤害值，由枪赋予
        private float currentBulletLifeTime; //用于记录当前子弹生命时间
        private bool isFirst; //用于记录这个子弹是否处于即将发射状态
        private LinkedList<Bullet> bulletList; //这个链表和RangedWeapon中武器子弹列表相同，用于更新子弹状态
        private Rigidbody bulletRigidbody; //辅助归零子弹速度和角速度，节省系统开销
        private Transform flying; //子弹飞行特效，如果有的话在子弹飞行中显示
        private Transform explosion; //子弹爆炸特效，如果有的话在子弹碰撞到其他物体时显示
        protected float BulletRealLifeTime;    //子弹的实际生命周期，如果子弹在碰撞后还需要显示爆炸动画的话，那么需要将子弹的原始生命周期加上这个动画的持续时间
        private Boolean isDead; //记录子弹是否已经死亡，防止因超过生命周期而删除的子弹重复多次显示渲染粒子特效
        private AudioSource audioExplosion;   //子弹爆炸特效，如果有的话在执行爆炸特效时播放
        void Awake()
        {
            initThisBullet();
            //Debug.Log("子弹awake执行!");
        }
        private void initThisBullet()
        {
            //子弹没有碰撞体的话，警告!
            if (this.gameObject.GetComponent<Collider>() == null)
            {
                Debug.Log("子弹没有添加碰撞体！");
            }
            //子弹没有刚体的话，警告！
            if (this.gameObject.GetComponent<Rigidbody>() == null)
            {
                Debug.Log("子弹没有添加刚体！");
            }
            bulletRigidbody = GetComponent<Rigidbody>();

            //绑定特效
            flying = this.transform.Find("Particle_Flying");
            explosion = this.transform.Find("Particle_Explosion");
            audioExplosion = this.GetComponent<AudioSource>();
            //先把爆炸的碰撞体隐藏掉，不然爆炸的碰撞体也会在子弹飞行中引发碰撞检测函数,并顺便计算实际生命周期
            if (explosion != null)
            {
                explosion.GetComponent<Rigidbody>().isKinematic = true; //不设置遵循动力学的话，不能发射
                explosion.GetComponent<Collider>().enabled = false;
                BulletRealLifeTime = BulletLifeTime + explosion.GetComponent<ParticleSystem>().main.duration;
            }
            else
            {
                BulletRealLifeTime = BulletLifeTime;
                //Debug.Log(BulletRealLifeTime);
            }
            this.gameObject.SetActive(false);

        }

        //测试证明调用SetActive不会重新调用Start()
        void Start()
        {
            //Debug.Log("Start调用！");
        }
        void Update()
        {
            //Debug.Log("Update调用");
            if (isFirst)
            { //如果这个子弹是即将发射状态，那么初始化它的生命周期，并显示飞行特效
                currentBulletLifeTime = 0;
                isFirst = false;
                //显示飞行粒子特效,如果有的话
                if (flying != null)
                {
                    flying.GetComponent<ParticleSystem>().Play();
                }
            }
            currentBulletLifeTime += Time.deltaTime;
            if (!isDead) //活着的子弹才进行是否超过生命周期判定
            {
                isOverLifeTime(); //检测是否超过生命周期  
            }
        }

        //检测是否超过生命周期,超过就删除此子弹,如果有爆炸特效的话，触发爆炸特效
        private void isOverLifeTime()
        {
            if (currentBulletLifeTime > BulletLifeTime)
            {
                //让子弹停止,很关键的代码~不然要加很多的代码来让爆炸位置不动,就算没有爆炸，这条语句也不会引发错误  
                bulletRigidbody.isKinematic = true;
                isDead = true;
                if (flying != null)
                {
                    StopFlyingAndExplode();
                }
                StartCoroutine(delayDelete(BulletRealLifeTime - BulletLifeTime));
                //Debug.Log("超过生命周期而删除");
            }
        }


        //检测是否进行了碰撞，进行了就删除此子弹,如果有爆炸特效的话，触发爆炸特效
        private void OnCollisionEnter(Collision other)
        {
            //此判断是为了防止在极端情况下：物体一出现就发生了两次碰撞，而引发错误，比如把枪口伸到物体里面
            if (true || !isDead)
            {
                //让子弹停止,很关键的代码~不然要加很多的代码来让爆炸位置不动,就算没有爆炸，这条语句也不会引发错误  
                bulletRigidbody.isKinematic = true;

                Temp_Scarecrow player = other.gameObject.GetComponent<Temp_Scarecrow>();
                if (flying != null)
                {
                    StopFlyingAndExplode();

                    if (explosion != null)
                    {


                        SphereCollider thisSphereCollider = explosion.GetComponent<SphereCollider>();
                        //此函数用于判断爆炸范围内有无玩家，有的话就进行扣血
                        explosionDamage(thisSphereCollider.transform.TransformPoint(thisSphereCollider.center), thisSphereCollider.bounds.size.x / 2);

                        /* 精确测试爆炸范围代码 
                        GameObject testSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        testSphere.GetComponent<Collider>().enabled = false;
                        testSphere.transform.localScale = thisSphereCollider.bounds.size ;
                        testSphere.transform.position =  thisSphereCollider.transform.TransformPoint(thisSphereCollider.center);
                        */
                    }
                }
                else
                {
                    //临时扣血代码
                    if (player != null)
                    {
                        player.OnAttacked(this.damage, other.contacts[0]);
                        //Debug.Log("OnAttacked执行！");
                        player = null;
                    }
                }
                //此判断是为了防止子弹在某些情况下能触发两个碰撞（比如把枪口伸到物体里面），就会调用两次本函数，那么第二次的碰撞就会因为物体是非激活状态而引发错误
                if (this.gameObject.activeInHierarchy == true)
                {
                    StartCoroutine(delayDelete(BulletRealLifeTime - BulletLifeTime));
                }
            }
            isDead = true;
        }
        //此函数用于判断爆炸范围内有无玩家，有的话就进行扣血, 参数：center-圆心坐标（世界坐标系），radius-半径
        private void explosionDamage(Vector3 center, float radius)
        {
            Debug.Log("center: " + center + "radius" + radius);
            Collider[] colliders = Physics.OverlapSphere(center, radius);//此函数用于检测周围球形范围内碰撞体，后期可加入LayoutMask用于筛选
            //没有碰撞体直接返回
            if (colliders.Length <= 0)
            {
                return;
            }

            for (int i = 0; i < colliders.Length; i++)
            {
                Temp_Scarecrow scarecrow = colliders[i].gameObject.GetComponent<Temp_Scarecrow>();
                if (scarecrow != null)
                {
                    //计算下人物所要受到的伤害，目前使用简单线性关系：y =100 * （1 - x） ，y是受到的伤害量，x是两点间距离占球形碰撞体半径的百分比，
                    //0代表在圆心爆炸，受到满额100点伤害，1代表在边缘处爆炸，受到0点伤害
                    int currentDamage = (int)(this.damage * (1 - (Vector3.Distance(scarecrow.transform.position, center) / radius)));
                    Debug.Log("this.damage:" + this.damage + ",distance:" + Vector3.Distance(scarecrow.transform.position, center) + ",radius:" + radius + ",percentage:" + (Vector3.Distance(scarecrow.transform.position, center) / radius));
                    scarecrow.OnAttacked(currentDamage);
                }
                //Debug.Log(colliders[i].gameObject.name);
            }
        }
        private IEnumerator delayDelete(float delayTime)
        {
            //等待延迟时间
            for (float i = 0; i < delayTime; i += Time.deltaTime)
            {
                yield return 0;
            }

            Bullet tempBullet = this;
            bulletList.RemoveLast();
            bulletList.AddFirst(tempBullet);
            this.gameObject.SetActive(false);
        }
        public void setInitTransformAndDamage(Transform initTransform, int damage)
        {
            this.gameObject.SetActive(true);

            this.damage = damage;

            this.transform.position = initTransform.position;
            this.transform.rotation = initTransform.rotation;
            this.transform.eulerAngles = initTransform.eulerAngles;

            this.GetComponent<Collider>().enabled = true; //启用物体本身的碰撞体            

            bulletRigidbody.velocity = Vector3.zero;
            bulletRigidbody.angularVelocity = Vector3.zero;
            bulletRigidbody.isKinematic = false; //让子弹可以受力而运动
            if (flying != null)
            {
                flying.gameObject.SetActive(true);
            }
            isDead = false;
            isFirst = true;
        }
        //获取子弹生命周期，给武器调用
        public float getBulletRealLifeTime()
        {
            return BulletRealLifeTime;
        }
        public void setBulletList(LinkedList<Bullet> bulletList)
        {
            this.bulletList = bulletList;
        }
        //停止飞行粒子特效并且触发爆炸特效
        private void StopFlyingAndExplode()
        {
            flying.GetComponent<ParticleSystem>().Stop();
            flying.gameObject.SetActive(false);
            if (explosion != null)
            {

                explosion.GetComponent<ParticleSystem>().Play(); //爆炸特效显示

                //调用爆炸碰撞体控制函数，现在的思路是碰撞发生后，将碰撞体残留0.2秒，然后消除，不然会在爆炸地点一直存在一个球状空气墙
                StartCoroutine(explosionCollisionControler());

                if (audioExplosion != null)
                {
                    audioExplosion.Play(); //如果有的话，播放爆炸声音
                }
            }

        }

        private IEnumerator explosionCollisionControler()
        {
            this.GetComponent<Collider>().enabled = false; //禁用物体本身的碰撞体
            explosion.GetComponent<Collider>().enabled = true; //启动爆炸的碰撞体
            for (float i = 0; i < 0.1; i += Time.deltaTime)
            {
                // Debug.Log("explosion Exist");
                yield return 0;
            }
            explosion.GetComponent<Collider>().enabled = false; //消除爆炸的碰撞体
        }
    }
}