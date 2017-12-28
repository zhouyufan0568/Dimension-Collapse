using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ZKAnimator : MonoBehaviour
{

    private Animator animator;

    private const float maxIdleTime = 3.0f; // If player static util this time, randomly play a new idle animation.
    private float currentIdleTime; // Current idle continuing time.
                                   // Use this for initialization
    void Start()
    {
        animator = this.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Can't get animator component from " + this.gameObject.name);
        }
    }
    private void OnEnable()
    {
        currentIdleTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        randomPlayIdle();
        RunningAction();
        WalkingAction();
        JumpAction();


        // Conclusion:Random between 1,2,3,4
        // System.Random ro = new System.Random();
        // int oneRandom = ro.Next(1, 5);
        // Debug.Log("Test random: " + oneRandom);

    }

    private void JumpAction()
    {
        if(Input.GetButtonDown("Jump"))
        {
            // Debug.Log("Jump trigger!ww");
            animator.SetTrigger("Jump");
        }
    }

    private void WalkingAction()
    {
        if (Input.GetAxis("Vertical") != 0)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }

    private void RunningAction()
    {
        if (Input.GetButtonDown("Accelerate"))
        {
            animator.SetBool("IsRunning", true);
        }
        if (Input.GetButtonUp("Accelerate"))
        {
            animator.SetBool("IsRunning", false);
        }
    }

    /// <summary>
    /// Random play alternate idle animation.
    /// </summary>
    private void randomPlayIdle()
    {
        // Animator is not playing alternate animation, and if animator is not in idle, that's meaning player is doing other animation, so just return
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Idle"))
        {
            currentIdleTime = 0f;
            animator.SetInteger("AlternateIdleIndex", 0);
            return;
        }
        // Animator is playing alternate animation,return
        if (animator.GetInteger("AlternateIdleIndex") != 0)
        {
            // It means alternate animation is over, should set the Index to  0, or player will stuck.Meanwhile current animator shouldn't be Alternate Idle(It's normalizedTime may over 0.99 and instantly back to Alternate Idle).
            if (animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.99f && !animator.GetCurrentAnimatorStateInfo(1).IsName("Alternate Idle"))
            {
                animator.SetInteger("AlternateIdleIndex", 0);
            }
            return;
        }
        //Now player is continuing idle enough time,so randomly play a alternate idle animation.
        if (currentIdleTime > maxIdleTime)
        {
            currentIdleTime = 0f;
            System.Random ro = new System.Random();
            int oneRandom = ro.Next(1, 5);
            animator.SetInteger("AlternateIdleIndex", oneRandom);
        }
        //Still not enough.
        else
        {
            animator.SetInteger("AlternateIdleIndex", 0);
            currentIdleTime += Time.deltaTime;
        }
    }
}
