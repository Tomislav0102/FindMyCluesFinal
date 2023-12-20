using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Shovel : MileStone, IActivation
{
    [SerializeField] MeshRenderer mesh;
    [SerializeField] Material matGame, matDissolve;
    DissolveManager _dissolveManager;

    [SerializeField] ScriptableInv shovelInvItem;
    [SerializeField] ParticleSystem ps;

    bool _closeEnough;

    public bool IsActive { get; set; }
    protected override void Awake()
    {
        base.Awake();
        _dissolveManager = new DissolveManager(new Material[] { matDissolve });
        ps.transform.parent = myTransform.parent;
        IsActive = true;
    }

    void Update()
    {
        if (!IsActive) return;

        myTransform.Rotate(20f * Time.deltaTime * Vector3.up);

        float distanceToPlayer = Vector3.Distance(Utilities.HorPos(myTransform.position), Utilities.HorPos(gm.camTr.position));
        _closeEnough = distanceToPlayer <= ExperienceManager.MinMaxDistance.x;
        if (!_closeEnough) return;

        IsActive = false;
        ps.Stop();
        gm.uImanager.AddItemInInventory(shovelInvItem);
        mesh.sharedMaterial = matDissolve;
        _dissolveManager.DissolveMe(() =>
        {
            gm.characterTalking.CharacterInfoText("Now find the Mud Ditch.");
            gameObject.SetActive(false);
        }, 3f);

    }
    void OnMouseDown()
    {
        if (!_closeEnough)
        {
            gm.characterTalking.CharacterInfoText("You're too far, come closer.", Emotion.Sad);
        }
    }

    void OnDisable()
    {
        _dissolveManager.ResetMaterial();
    }
}
