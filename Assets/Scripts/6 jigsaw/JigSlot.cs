using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JigSlot : Jig
{

    protected override void Awake()
    {
        base.Awake();
        IsActive = false;
      //  IsActive = true;
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        JigsawManager.TilesCollected += () => IsActive = true;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        JigsawManager.TilesCollected -= () => IsActive = true;
    }
}
