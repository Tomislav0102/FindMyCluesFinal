using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_ScratchOffMap : StateMachineBehaviour
{
    [SerializeField] int ordinal;
    ScratchOffMap _scratchOffMap;
    bool _oneHit;
    ScratchOffMap ScratchOffMap(Animator anim)
    {
        if(_scratchOffMap == null) _scratchOffMap = anim.GetComponentInParent<ScratchOffMap>();
        return _scratchOffMap;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_oneHit) return;
        if(stateInfo.normalizedTime >= 1f)
        {
            _oneHit = true;
            ScratchOffMap(animator).AnimCallback(ordinal);
        }
    }

}
