using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;     

public class ScratchOffMap : MileStone, IActivation
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Transform coinTransform;
    Animator _animCoin;
    [SerializeField] Transform[] masks;
    [SerializeField] ScriptableInv scratchOffMap;
    [SerializeField] SpriteRenderer[] scratchSurfaces;
    [SerializeField] ParticleSystem psScratch;
    public bool IsActive { get; set; }
    bool _onMouseDown, _inPosition, _oneHit;
    int _counterScratch;
    DissolveManager _dissolveManager;
    ScreenPositioning _screenPositioning;

    protected override void Awake()
    {
        base.Awake();
        _dissolveManager = new DissolveManager(new Material[] { gm.matSpriteDissolve });
        _dissolveManager.DissolveMe(() =>
        {
            spriteRenderer.sharedMaterial = gm.matSpriteDef;
        }, 1f, false);
        _screenPositioning = new ScreenPositioning(myTransform, gm.focusPoint.GetChild(gm.xp.currentLocation.ordinal));
        _animCoin = coinTransform.GetComponent<Animator>();
    }

    public override void StartMe()
    {
        base.StartMe();
        if (IsActive) return;
        IsActive = true;
        gm.arFloor.SetActive(false);
        gm.characterTalking.CharacterInfoText("Tap on the map to scratch it with a coin from your inventory!");
    }
    void Update()
    {
        if (!IsActive) return;
        _screenPositioning.UpdateLoop(ref _inPosition);

        //if (Input.GetKeyDown(KeyCode.Alpha1)) _animCoin.Play("move0");
        //if (Input.GetKeyDown(KeyCode.Alpha2)) _animCoin.Play("move1");
        //if (Input.GetKeyDown(KeyCode.Alpha3)) _animCoin.Play("move2");
    }

    private void OnMouseDown()
    {
        if (!IsActive || _onMouseDown || !_inPosition) return;
        _onMouseDown = true;

        if (!_oneHit)
        {
            _oneHit = true;
            gm.arFloor.SetActive(false);
            scratchSurfaces[1].enabled = false;
            coinTransform.gameObject.SetActive(true);
            psScratch.Play();
        }


        if(_counterScratch == 0)
        {
            _animCoin.Play("move0");
        }
        else if(_counterScratch == 1)
        {
            _animCoin.Play("move1");
        }
        else if(_counterScratch == 2)
        {
            _animCoin.Play("move2");
        }

        int maskCounter = Mathf.Min(_counterScratch, masks.Length - 1);
        masks[maskCounter].gameObject.SetActive(true);
    }

    private void End()
    {
        scratchSurfaces[0].DOFade(0f, 2f)
                        .OnComplete(() =>
                        {
                            psScratch.Stop();
                            gm.uImanager.AddItemInInventory(scratchOffMap);
                            spriteRenderer.sharedMaterial = gm.matSpriteDissolve;
                            _dissolveManager.DissolveMe(() =>
                            {
                                gm.xp.NewLocation();
                                gm.arFloor.SetActive(true);
                                gameObject.SetActive(false);
                            }, 3f);
                        });
    }

    void OnDisable()
    {
        _dissolveManager.ResetMaterial();
    }

    public void AnimCallback(int animationOrdinal)
    {
        _counterScratch = animationOrdinal + 1;
        _onMouseDown = false;
        string charTalkLine = string.Empty;
        if (_counterScratch == 1)
        {
            charTalkLine = "Tap again.";
            masks[0].parent = myTransform;
        }
        else if (_counterScratch == 2)
        {
            charTalkLine = "Almost there, tap just one more time.";
            masks[1].parent = myTransform;
        }
        else if (_counterScratch == 3)
        {
            charTalkLine = "That's it!";
            End();
        }
        gm.characterTalking.CharacterInfoText(charTalkLine, _counterScratch == 3 ? Emotion.Smile : Emotion.Neutral);
    }
}
