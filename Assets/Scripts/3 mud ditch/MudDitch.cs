using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MudDitch : MileStone, IActivation
{
    [SerializeField] Material matGame, matDissolve;

    DissolveManager _dissolveManager;
    DissolveManager _dissolveManagerShovel;
    [SerializeField] Material shovelMat, shovelMatDissolve;

    [SerializeField] GameObject shovel, chest;
    Animator _animShovel;
    [SerializeField] ScriptableInv shovelItem, mudDitchInvItem;
    public ParticleSystem psDig;

    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            if (!IsActive)
            {
                GetComponent<MeshRenderer>().sharedMaterial = matDissolve;
                _dissolveManager.DissolveMe(() => 
                {
                    gm.arFloor.SetActive(false);
                    gameObject.SetActive(false); 
                });
                chest.SetActive(true);

                _animShovel.speed = 0f;
                shovel.GetComponent<MeshRenderer>().sharedMaterial = shovelMatDissolve;
                _dissolveManagerShovel.DissolveMe(null);
                psDig.Play();
            }
        }
    }
    bool _isActive;
    float _currentAnimSpeed = 0f;
    float _dugSoFar;
    bool _playerIsInRangeToDig;
    bool _switchRange;

    protected override void Awake()
    {
        base.Awake();
        _dissolveManager = new DissolveManager(new Material[] { matDissolve });
        _dissolveManagerShovel = new DissolveManager(new Material[] { shovelMatDissolve });
        _animShovel = shovel.GetComponent<Animator>();

    }

    private void Update()
    {
        float dist = Vector3.Distance(Utilities.HorPos(myTransform.position), Utilities.HorPos(gm.camTr.position));
        _playerIsInRangeToDig = dist < 6f;

        if( _playerIsInRangeToDig && !_switchRange && gm.uImanager.ItemInInventory(shovelItem))
        {
            _switchRange = true;
            shovel.GetComponent<MeshRenderer>().sharedMaterial = shovelMatDissolve;
            shovel.SetActive(true);
            _dissolveManagerShovel.DissolveMe(() =>
            {
                shovel.GetComponent<MeshRenderer>().sharedMaterial = shovelMat;
                IsActive = true;
                gm.characterTalking.CharacterInfoText("Tap to dig!");
            }, 0.5f, false);
        }

        if(!IsActive) return;

        _currentAnimSpeed -= Time.deltaTime;
        if(_currentAnimSpeed <= 0f)
        {
            _currentAnimSpeed = 0f;
            _dugSoFar = 0f;
        }
        _animShovel.speed = _currentAnimSpeed;

        _dugSoFar += Time.deltaTime * 0.2f;
        if(_dugSoFar >= 1f)
        {
            IsActive = false;
        }
    }
    private void OnMouseDown()
    {
        if (!gm.uImanager.ItemInInventory(shovelItem))
        {
            gm.characterTalking.CharacterInfoText("You need to find a shovel first...", Emotion.Sad);
            return;
        }
        if (!IsActive)
        {
            return;
        }
        if (!_playerIsInRangeToDig)
        {
            gm.characterTalking.CharacterInfoText("You're too far to dig.\nCome closer.", Emotion.Sad);
            return;
        }
        _currentAnimSpeed = 1f;
    }


    void OnDisable()
    {
        _dissolveManager.ResetMaterial();
        _dissolveManagerShovel.ResetMaterial();
    }
}
