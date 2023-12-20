using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShovelMudCombined : MileStone
{
    [SerializeField] Transform shovel;
    [SerializeField] Transform ditch;


    protected override void Awake()
    {
        base.Awake();
        Vector3 dir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        float range = Random.Range(5f, 20f);
        shovel.position = dir * range + myTransform.position;
        dir *= -1;
        ditch.position = dir * range - 0.2f * Vector3.up + myTransform.position;
        ExperienceManager.MinMaxDistance = new Vector2(4f, 35f);
        gm.characterTalking.CharacterInfoText("Find a shovel.");
    }
}
