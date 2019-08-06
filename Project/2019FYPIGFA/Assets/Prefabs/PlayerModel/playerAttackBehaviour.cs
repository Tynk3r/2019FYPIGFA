using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAttackBehaviour : StateMachineBehaviour
{
    private bool m_inMeleeAttack = false;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, animatorStateInfo, layerIndex);
        if (animator.GetNextAnimatorStateInfo(0).IsName("attack"))
        {
            m_inMeleeAttack = true;
            Debug.Log("attack1");
        }
        else if (m_inMeleeAttack)
        {
            animator.SetBool("attack", true);
            m_inMeleeAttack = false;
            Debug.Log("attack2");
        }
        Player player = (Player)FindObjectOfType(typeof(Player));
    }
}
