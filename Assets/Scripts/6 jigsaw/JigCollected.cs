using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JigCollected : Jig
{
    public JigsawState jState;
    public int lowerPositionCounter; //needed for returning to lower part (drag is canceled, etc.)
    public ParticleSystem psTrail;
    public ParticleSystem psSet;

    protected override void Awake()
    {
        base.Awake();
        IsActive = true;
    }



}
