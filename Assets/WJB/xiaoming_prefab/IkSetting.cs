using UnityEngine;

public class IkSetting : MonoBehaviour
{

    Animator animator;                      // 玩家动画控制器
    public bool isActive = false;            // 是否启用IK
    public Transform lookObj = null;        // 玩家头部IK标记物
    public Transform leftHandObj = null;    // 玩家左手IK标记物
    public Transform rightHandObj = null;   // 玩家右手IK标记物
    public Transform bodyObj = null;        // 玩家身体IK标记物

    void Start()
    {
        //
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK()
    {
        if (animator)
        {
            if (isActive)
            {
                if (lookObj != null)
                {
                    //设置玩家的头部IK，使玩家的头部面向头部IK标记物所在的位置
                    animator.SetLookAtWeight(1.0f);
                    animator.SetLookAtPosition(lookObj.position);
                }
                if (bodyObj != null)
                {
                    //设置玩家躯干IK，使玩家躯干的旋转角与bodyObj对象的旋转角度相同
                    animator.bodyRotation = bodyObj.rotation;
                }
                if (leftHandObj != null)
                {
                    //设置玩家左手的IK，使玩家左手的位置尽量靠近leftHandObj对象，左手的朝向与leftHandObj相同
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
                }
                if (rightHandObj != null)
                {
                    //设置玩家右手的IK，使玩家右手的位置尽量靠近RightHandObj对象，右手的朝向与RightHandObj相同
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }
            }
            else
            {
                //取消玩家角色的IK，使玩家角色的头部，驱赶，左右手的位置和朝向受正向动力学控制
                animator.SetLookAtWeight(0);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            }
        }
    }
}
