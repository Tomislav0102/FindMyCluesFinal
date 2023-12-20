using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

public class Guide : MileStone, IActivation
{
    [SerializeField] SkinnedMeshRenderer meshCharacter, meshBag;
    [SerializeField] Material matDissolveChar, matDissolveBag;
    DissolveManager _dissolveManager;
    [SerializeField] float _fallSpeed;
    [HideInInspector] public Vector3[] landingPoints = new Vector3[3];
    [SerializeField] ParticleSystem psLanding;
    [SerializeField] Transform parachute;
    SkinnedMeshRenderer _parachuteSkinnedMesh;
    bool _oneHitpsLanding;
    float ParachuteBlendParameter
    {
        get => _parachuteBlendParameter;
        set
        {
            _parachuteBlendParameter = value;
            _parachuteSkinnedMesh.SetBlendShapeWeight(1, value);
        }
    }
    float _parachuteBlendParameter;

    Animator _anim;
    int _landed = Animator.StringToHash("landed");
    Rig _rigLeftHand, _rigRightHand;
    float _weightHands = 1f;
    BlinkingBlendShape _blinkingBS;
    TalkingBlendShape _talkingBS;
    bool _isTalking;

    bool _hasLanded, _hasTalked, _oneHitPoint, _oneHitOpenUI;
    float _timerLand, _timerStartTalking, _timerDoneTalking;
    public bool IsActive { get; set; }
    LocationMileStone _next;


    protected override void Awake()
    {
        base.Awake();
        _blinkingBS = new BlinkingBlendShape(meshCharacter, 5);
        _talkingBS = new TalkingBlendShape(meshCharacter, 6);
        ExperienceManager.MinMaxDistance = new Vector2(20f, 40f);
        _dissolveManager = new DissolveManager(new Material[] { matDissolveChar, matDissolveBag });
        _next = gm.xp.NextLocation();
    }
    private void OnDisable()
    {
        _dissolveManager.ResetMaterial();
    }
    private void Update()
    {
        if (!IsActive) return;

        Vector3 target = _hasTalked ? _next.myTransform.position : gm.camTr.position;
        Vector3 dir = (target - myTransform.position).normalized;
        dir.y = 0f;
        myTransform.rotation = Quaternion.Lerp(myTransform.rotation, Quaternion.LookRotation(dir), Time.deltaTime);

        if (_timerLand <= 1f)
        {
            _timerLand += 0.15f * Time.deltaTime;
            myTransform.position = Utilities.BezierCurvePosition(landingPoints, _timerLand);
            return;
        }

        _blinkingBS.UpdateLoop();
        if(_isTalking) _talkingBS.UpdateLoop();
        else _talkingBS.StopTalk();

        _anim.SetBool(_landed, true);
        if (!_oneHitpsLanding)
        {
            _oneHitpsLanding = true;
            psLanding.Play();
            DOTween.To(() => ParachuteBlendParameter, x => ParachuteBlendParameter = x, 100f, 1f)
                .OnComplete(() =>
                {
                    parachute.DOScale(Vector3.zero, 0.3f);
                });
        }
        _weightHands = Mathf.MoveTowards(_weightHands, 0f, 0.5f * Time.deltaTime);
        _rigLeftHand.weight = _rigRightHand.weight = _weightHands;
    }
    public override void StartMe()
    {
        if (IsActive) return;
        IsActive = true;

        _anim = GetComponent<Animator>();
        _rigLeftHand = GetComponent<RigBuilder>().layers[0].rig;
        _rigRightHand = GetComponent<RigBuilder>().layers[1].rig;

        _parachuteSkinnedMesh = parachute.GetComponent<SkinnedMeshRenderer>();
        parachute.DOScale(0.02f * Vector3.one, 1f);
    }

    public override void TickMe()
    {
        base.TickMe();
        _hasTalked = true;
        _anim.SetTrigger("doneTalking");
    }
    #region ANIMATION CALLBACKS
    public void TalkCallback(bool startTalk)
    {
        if (startTalk)
        {
            gm.uImanager.ShowHideUI(true, CanvasType.TalkOnly);
        }
        else
        {
            gm.uImanager.ShowHideUI(false, CanvasType.TalkOnly);
        }
        _isTalking = startTalk;
    }
    public void AE_Point()
    {
        IsActive = false;
        Invoke(nameof(EndMe), 3f);
    }
    #endregion
    public override void EndMe() 
    {
        base.EndMe();
        gm.arFloor.SetActive(false);
        meshCharacter.sharedMaterial = matDissolveChar;
        meshBag.sharedMaterial = matDissolveBag;
        _dissolveManager.DissolveMe(() =>
        {
            Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation, myTransform.parent);
            gm.characterTalking.CharacterStart();
            gameObject.SetActive(false);
        }, 2f);
    }
}


/*
[SerializeField] SkinnedMeshRenderer meshCharacter, meshBag;
[SerializeField] Material matDissolveChar, matDissolveBag;
DissolveManager _dissolveManager;
[SerializeField] float _fallSpeed;
[HideInInspector] public Vector3[] landingPoints = new Vector3[3];
[SerializeField] ParticleSystem psLanding;
[SerializeField] Transform parachute;
SkinnedMeshRenderer _parachuteSkinnedMesh;
bool _oneHitpsLanding;
float ParachuteBlendParameter
{
    get => _parachuteBlendParameter;
    set
    {
        _parachuteBlendParameter = value;
        _parachuteSkinnedMesh.SetBlendShapeWeight(1, value);
    }
}
float _parachuteBlendParameter;

Animator _anim;
int _landed = Animator.StringToHash("landed");
Rig _rigLeftHand, _rigRightHand;
float _weightHands = 1f;
BlinkingBlendShape _blinkingBS;
TalkingBlendShape _talkingBS;

bool _hasLanded, _hasTalked, _oneHitPoint, _oneHitOpenUI;
float _timerLand, _timerStartTalking, _timerDoneTalking;
public bool IsActive { get; set; }

protected override void Awake()
{
    base.Awake();
    _blinkingBS = new BlinkingBlendShape(meshCharacter, 5);
    _talkingBS = new TalkingBlendShape(meshCharacter, 6);
    ExperienceManager.MinMaxDistance = new Vector2(20f, 40f);
    _dissolveManager = new DissolveManager(new Material[] { matDissolveChar, matDissolveBag });
}
private void OnEnable()
{
    gm.uImanager.btnEndTalk.onClick.AddListener(TickMe);
}
private void OnDisable()
{
    gm.uImanager.btnEndTalk.onClick.RemoveListener(TickMe);
    _dissolveManager.ResetMaterial();
}

private void Update()
{
    if (!IsActive) return;

    Vector3 target = _hasTalked ? _next.myTransform.position : gm.camTr.position;
    Vector3 dir = (target - myTransform.position).normalized;
    dir.y = 0f;
    myTransform.rotation = Quaternion.Lerp(myTransform.rotation, Quaternion.LookRotation(dir), Time.deltaTime);

    if (_timerLand <= 1f)
    {
        _timerLand += 0.15f * Time.deltaTime;
        myTransform.position = Utilities.BezierCurvePosition(landingPoints, _timerLand);
        return;
    }

    _blinkingBS.UpdateLoop();

    _anim.SetBool(_landed, true);
    if (!_oneHitpsLanding)
    {
        _oneHitpsLanding = true;
        psLanding.Play();
        DOTween.To(() => ParachuteBlendParameter, x => ParachuteBlendParameter = x, 100f, 1f)
            .OnComplete(() =>
            {
                parachute.DOScale(Vector3.zero, 0.3f);
            });
    }
    _weightHands = Mathf.MoveTowards(_weightHands, 0f, 0.5f * Time.deltaTime);
    _rigLeftHand.weight = _rigRightHand.weight = _weightHands;

    _timerStartTalking += Time.deltaTime;
    if (_timerStartTalking > 1f && !_oneHitOpenUI)
    {
        _oneHitOpenUI = true;
        gm.uImanager.ShowHideUI(true, CanvasType.TalkOnly);
    }

    if (_oneHitOpenUI && !_hasTalked)
    {
        _talkingBS.UpdateLoop();
    }

    if (_hasTalked && !_oneHitPoint)
    {
        _talkingBS.StopTalk();
        _oneHitPoint = true;
        _anim.SetTrigger("point");
    }
}
public override void StartMe()
{
    if (IsActive) return;
    IsActive = true;

    _anim = GetComponent<Animator>();
    _rigLeftHand = GetComponent<RigBuilder>().layers[0].rig;
    _rigRightHand = GetComponent<RigBuilder>().layers[1].rig;

    _next = gm.xp.locationMileStones[1];
    _parachuteSkinnedMesh = parachute.GetComponent<SkinnedMeshRenderer>();
    parachute.DOScale(0.02f * Vector3.one, 1f);
}

public override void TickMe()
{
    base.TickMe();
    _hasTalked = true;
    _anim.SetTrigger("doneTalking");
}
public void AE_Point()
{
    gm.xp.NewLocation((LocationMileStone)_next);
    IsActive = false;
    Invoke(nameof(EndMe), 3f);
}

public override void EndMe() //this isn' finished
{
    base.EndMe();
    meshCharacter.sharedMaterial = matDissolveChar;
    meshBag.sharedMaterial = matDissolveBag;
    _dissolveManager.DissolveMe(() =>
    {
        gm.characterTalking.CharacterStart();
        gameObject.SetActive(false);
    }, 2f);
}
*/