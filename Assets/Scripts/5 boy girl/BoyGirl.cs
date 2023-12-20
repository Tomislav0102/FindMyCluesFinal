using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoyGirl : MileStone, IActivation
{
    [SerializeField] GameObject[] actors;
    [SerializeField] Animator[] anims;
    [SerializeField] SkinnedMeshRenderer[] skinnedMeshes;
    [SerializeField] Material[] mats;
    DissolveManager _dissolveManager;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            if (_isActive)
            {
                gm.uImanager.ShowHideUI(true, CanvasType.TalkOnly);
            }
            else
            {
                gm.uImanager.ShowHideUI(false, CanvasType.TalkOnly);
                _dissolveManager.DissolveMe(() =>
                {
                   // gm.xp.NewLocation();
                    Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation, myTransform.parent);
                    gameObject.SetActive(false);
                });
            }
        }
    }
    bool _isActive;
    float _timer, _timerTotal;
    const int CONST_MAXTIME = 10;
    float TalkTime() => Random.Range(3f, 10f);
    int _boyGirlCounter;
    int[] _idleNames, _talkNames;
    BlinkingBlendShape[] _blinking;
    TalkingBlendShape[] _talkingBS = new TalkingBlendShape[2];

    protected override void Awake()
    {
        base.Awake();
        _dissolveManager = new DissolveManager(mats);
        _dissolveManager.DissolveMe(null, 1f, false);
        myTransform.LookAt(Utilities.HorPos(gm.camTr.position));
        _blinking = new BlinkingBlendShape[2];
        for (int i = 0; i < 2; i++)
        {
            _blinking[i] = new BlinkingBlendShape(skinnedMeshes[i], i);
        }
        Utilities.FillAnimStateNames(ref _idleNames, ref  _talkNames, 4);
        _talkingBS[0] = new TalkingBlendShape(skinnedMeshes[0], 2);
        _talkingBS[1] = new TalkingBlendShape(skinnedMeshes[1], 0);
      //  gm.characterTalking.CharacterInfoText("Come closer to hear what they are talking about.");
    }
    public override void StartMe()
    {
        base.StartMe();
        IsActive = true;
    }
    public override void TickMe()
    {
        base.TickMe();
        if (!IsActive) return;
        IsActive = false;
    }
    void OnDisable()
    {
        _dissolveManager.ResetMaterial();
    }

    private void Update()
    {
        for (int i = 0; i < 2; i++)
        {
            _blinking[i].UpdateLoop();
        }
        if (!IsActive) return;

        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            _timer = TalkTime();
            _boyGirlCounter = (1 + _boyGirlCounter) % 2;
            for (int i = 0; i < 2; i++)
            {
                if (i == _boyGirlCounter)
                {
                    anims[i].CrossFade(_talkNames[Random.Range(0, _talkNames.Length)], 0.1f);
                }
                else
                {
                    anims[i].CrossFade(_idleNames[Random.Range(0, _idleNames.Length)], 0.1f);
                }
            }
        }
        _talkingBS[_boyGirlCounter].UpdateLoop();
        _talkingBS[(1 + _boyGirlCounter) % 2].StopTalk();

        _timerTotal += Time.deltaTime;
        if(_timerTotal > CONST_MAXTIME) TickMe();
    }


}
