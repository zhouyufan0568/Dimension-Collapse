using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCenter : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    private Animator animator;
    private IkSetting ikSetting;

    void Start ()
    {
        ikSetting = GetComponent<IkSetting>();
        animator = GetComponent<Animator>();
    }
	// updata中是用于测试的，用的时候注释掉就好
	//void Update () {
 //       //换枪动画测试
 //       //if (Input.GetKeyDown(KeyCode.H))
 //       //{
 //       //    animator.SetTrigger("holdGun_ttigger");
 //       //}
 //       //ik测试
 //       //if (Input.GetKeyDown(KeyCode.T))
 //       //{
 //       //    GetComponent<IkSetting>().isActive = true;
 //       //}
 //       //if (Input.GetKeyDown(KeyCode.P)) {
 //       //    GetComponent<IkSetting>().isActive = false;
 //       //}
 //   }

    public void HandleInput()
    {
        //站立蹲
        if (Input.GetKeyDown(KeyCode.Z))
        {
            AnimationOfProne();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            AnimationOfCrouch();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            AnimationOfStand();
        }
        // 跳
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AnimationOfJump();
        }
        //移动
        if (Input.GetKey(KeyCode.W))
        {
            vertical = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            vertical = -1;
        }
        else
        {
            vertical = 0;
        }
        if (Input.GetKey(KeyCode.A))
        {
            horizontal = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            horizontal = 1;
        }
        else
        {
            horizontal = 0;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            vertical = 2;
        }
        AimationOfMove();
    }

    //当玩家跳跃时调用,不需要输入参数
    public void AnimationOfJump() {
        if (animator.GetBool("isStand"))
        {
            animator.SetTrigger("isJump");
        }
    }

    //当玩家射击时候调用，输入true代表在射击状态，输入false表示不在射击状态
    public void AnimationOfShooting(bool isShooting)
    {
        animator.SetBool("isShooting", isShooting);
    }

    //当玩家按下蹲时调用,不需要参数
    public void AnimationOfCrouch()
    {
        if (animator.GetBool("isCrouch")==false)
        {
            animator.SetBool("isCrouch", true);
            animator.SetBool("isStand", false);
            animator.SetBool("isProne", false);
        }
    }

    //当玩家按匍匐键时调用,不需要参数
    public void AnimationOfProne()
    {
        if (animator.GetBool("isLie") == false)
        {
            animator.SetBool("isCrouch", false);
            animator.SetBool("isStand", false);
            animator.SetBool("isProne", true);
        }
    }

    //当玩家按站立状态时调用,不需要参数
    public void AnimationOfStand()
    {
        if (animator.GetBool("isStand") == false)
        {
            animator.SetBool("isCrouch", false);
            animator.SetBool("isStand", true);
            animator.SetBool("isProne", false);
        }
    }

    //玩家移动，需要输入玩家水平方向输入以及垂直方向输入
    public void AimationOfMove()
    {
        animator.SetFloat("horizontal", horizontal);
        animator.SetFloat("vertical", vertical);
    }

    //换枪
    public void AimationOfGetGun()
    {
        animator.SetTrigger("HodGunTrigger");
    }

    public void AimationOfDie()
    {

    }

    public void EnableIK()
    {
        ikSetting.isActive = true;
    }

    public void DisableIK()
    {
        ikSetting.isActive = false;
    }
}
