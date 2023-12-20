using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class InvSlot : MonoBehaviour, IPointerClickHandler
{
    GameManager _gm;
    Transform _myTransform;
    RectTransform _myRectTransform;
    Vector3 _startPos;
    int _myOrdinal;
    Tween _twMove, _twScale;
    const float CONST_SPEED = 0.5f;

    Transform _center;
    TextMeshProUGUI _displayNameEnlarged;

    [SerializeField] TextMeshProUGUI displayText;
    [SerializeField] RawImage rawImage;
    bool IsEnlarged
    {
        get => _isEnlarged;
        set
        {
            if (!_canEnlarge) return;
            _isEnlarged = value;
            _displayNameEnlarged.text = _isEnlarged ? displayText.text : "";
            if (_isEnlarged)
            {
                displayText.enabled = false;
                _gm.uImanager.DisplayInvItems(false, _myOrdinal);

                _twMove = _myTransform.DOMove(_center.position, CONST_SPEED).OnComplete(() => EndTween(true));
                _twScale = _myTransform.DOScale(5f * Vector3.one, CONST_SPEED);
            }
            else
            {
                _twMove = _myTransform.DOMove(_startPos, CONST_SPEED).OnComplete(() => EndTween(false));
                _twScale = _myTransform.DOScale(Vector3.one, CONST_SPEED);
            }
            _canEnlarge = !_canEnlarge;
        }
    }
    bool _isEnlarged;
    bool _canEnlarge = true;

    void Awake()
    {
        _gm = GameManager.Instance;
        _myTransform = transform;
        _myRectTransform = GetComponent<RectTransform>();
        _startPos = _myTransform.position;
    }
    public void AddItem(ScriptableInv invData, Transform center, TextMeshProUGUI display, int ordinal)
    {
        displayText.text = invData.invName;
        rawImage.texture = invData.renderTexture;
        _center = center;
        _displayNameEnlarged = display;
        _myOrdinal = ordinal;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        IsEnlarged = !IsEnlarged;
    }

    public void ForceDefaultPosition()
    {
        if (_myTransform == null || _myTransform.localScale == Vector3.one) return;

        if (_twMove != null && _twMove.IsActive()) _twMove.Kill();
        if (_twScale != null && _twScale.IsActive()) _twScale.Kill();
        EndTween(false);
        _myTransform.position = _startPos;
        _myRectTransform.anchoredPosition = Vector2.zero;
        _myTransform.localScale = Vector3.one;
    }


    void EndTween(bool endEnlargment)
    {
        _canEnlarge = true;
        if (endEnlargment)
        {

        }
        else
        {
            _gm.uImanager.DisplayInvItems(true);
            displayText.enabled = true;
        }
    }

}