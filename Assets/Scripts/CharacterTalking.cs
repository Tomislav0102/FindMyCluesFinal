using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class CharacterTalking : MonoBehaviour
{
    [SerializeField] Transform talkCloud, characterTransform;
    [SerializeField] TextMeshProUGUI talkText;
    bool _characterWindowOpen;
    bool IsTalking
    {
        get => _isTalking;
        set
        {
            _isTalking = value;
            if (_isTalking)
            {
                animChar.CrossFade(_talkNames[Random.Range(0, _talkNames.Length)], 0.2f);
            }
            else
            {
                animChar.CrossFade(_idleNames[Random.Range(0, _idleNames.Length)], 0.2f);
                skinnedMeshRenderer.SetBlendShapeWeight(CONST_TALKBLENDSHAPE, 0f);
                Emotion = Emotion.Neutral;
            }
        }
    }
    bool _isTalking;
    Tween _tweenTalkGrow, _tweenTalkShrink;
    [SerializeField] Animator animChar;
    int[] _idleNames, _talkNames;
    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;
    const int CONST_BLINK = 5;
    BlinkingBlendShape _blinkingBS;
    TalkingBlendShape _talkingBS;
    const int CONST_TALKBLENDSHAPE = 6;
    const int CONST_SADBLENDSHAPE = 12;
    const int CONST_SMILEBLENDSHAPE = 13;
    Emotion Emotion
    {
        set
        {
            switch (value)
            {
                case Emotion.Neutral:
                    skinnedMeshRenderer.SetBlendShapeWeight(CONST_SADBLENDSHAPE, 0f);
                    skinnedMeshRenderer.SetBlendShapeWeight(CONST_SMILEBLENDSHAPE, 0f);
                    break;
                case Emotion.Sad:
                    skinnedMeshRenderer.SetBlendShapeWeight(CONST_SADBLENDSHAPE, 100f);
                    skinnedMeshRenderer.SetBlendShapeWeight(CONST_SMILEBLENDSHAPE, 0f);
                    break;
                case Emotion.Smile:
                    skinnedMeshRenderer.SetBlendShapeWeight(CONST_SADBLENDSHAPE, 0f);
                    skinnedMeshRenderer.SetBlendShapeWeight(CONST_SMILEBLENDSHAPE, 100f);
                    break;
            }
        }
    }

    private void Awake()
    {
        _blinkingBS = new BlinkingBlendShape(skinnedMeshRenderer, CONST_BLINK);
        _talkingBS = new TalkingBlendShape(skinnedMeshRenderer, CONST_TALKBLENDSHAPE);
        Utilities.FillAnimStateNames(ref _idleNames, ref _talkNames, 4);
    }
    private void Update()
    {
        if (!_characterWindowOpen) return;

        _blinkingBS.UpdateLoop();
        if (IsTalking) _talkingBS.UpdateLoop();
       // else _talkingBS.StopTalk();
       
    }
    public void CharacterStart(bool start = true)
    {
        Vector3 endScale = start ? Vector3.one : Vector3.zero;
        characterTransform.DOScale(endScale, 1f)
            .SetEase(Ease.OutBounce);

        _characterWindowOpen = start;
    }
    public void CharacterInfoText(string info, Emotion emotion = Emotion.Neutral, bool alwaysOpen = false)
    {
        if (!_characterWindowOpen) return;
       talkText.text = info;
        Emotion = emotion;
        if (alwaysOpen)
        {
            talkCloud.localScale = Vector3.one;
            return;
        }
        if (IsTalking)
        {
            if (_tweenTalkGrow != null && _tweenTalkGrow.IsActive()) _tweenTalkGrow.Kill();
            if (_tweenTalkShrink != null && _tweenTalkShrink.IsActive()) _tweenTalkShrink.Kill();
        }
        IsTalking = true;

        int speed = 15;
        _tweenTalkGrow = talkCloud.DOScale(Vector3.one, speed)
                        .SetSpeedBased(true)
                        .OnComplete(() =>
                        {
                            _tweenTalkShrink = talkCloud.DOScale(Vector3.zero, speed)
                                                .SetDelay(5f)
                                                .SetSpeedBased(true)
                                                .OnComplete(() => { IsTalking = false; });
                        });
    }

}
