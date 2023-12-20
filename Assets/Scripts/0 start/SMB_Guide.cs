using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_Guide : StateMachineBehaviour
{
    Guide _guide;
    Guide Gu(Animator anim)
    {
        if(_guide == null) _guide = anim.GetComponent<Guide>();
        return _guide;
    }


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Gu(animator).TalkCallback(true);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Gu(animator).TalkCallback(false);
    }
}
