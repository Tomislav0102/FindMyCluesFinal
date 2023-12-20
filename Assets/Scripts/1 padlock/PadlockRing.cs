using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PadlockRing : MonoBehaviour
{
    int _current;
    Transform[] _nums;
    bool _canTurn = true;
    Vector3 _lastMousePos;

    [SerializeField] int myIndex;
    [SerializeField] Padlock padlock;
    Transform _ringTransform;


    public void Init(int index)
    {
        _ringTransform = transform.GetChild(0);
        _nums = new Transform[_ringTransform.childCount - 1];
        for (int i = 0; i < _nums.Length; i++)
        {
            _nums[i] = _ringTransform.GetChild(i);
        }

        _ringTransform.DOLocalRotate(new Vector3(36f * index, 0f, 0f), 0.1f)
         .OnComplete(() => _current = CurrentIndex());
    }

    private void OnMouseDown()
    {
        if (!padlock.inPosition) return;
        _lastMousePos = Input.mousePosition;
    }
    private void OnMouseDrag()
    {
        if (!padlock.inPosition) return;
        Vector3 delta = Input.mousePosition - _lastMousePos;
        if (delta.y == 0f)
        {
            return;
        }
        TurnRing(delta.y > 0f);
        _lastMousePos = Input.mousePosition;
    }

    public void TurnRing(bool upDirection = true)
    {
        if (!_canTurn) return;
        _canTurn = false;

        _current = CurrentIndex();

        _ringTransform.DOLocalRotate(new Vector3(36f * TargetIndex(upDirection), 0f, 0f), 0.3f)
            .OnComplete(() =>
            {
                _current = CurrentIndex();
                padlock.InputNumber(myIndex, _current);
                _canTurn = true;
            });
    }

    int TargetIndex(bool upDirection)
    {
        int targetIndex = _current + (upDirection ? -1 : 1);

        if (targetIndex > 9) targetIndex = 0;
        else if (targetIndex < 0) targetIndex = 9;
        return targetIndex;
    }
    int CurrentIndex()
    {
        Vector3 parForward = transform.forward;
        float minVal = Mathf.NegativeInfinity;
        int index = 0;
        for (int i = 0; i < 10; i++)
        {
            float dot = Vector3.Dot(parForward, _nums[i].forward);
            if (dot > minVal)
            {
                minVal = dot;
                index = i;
            }
        }

        return index;
    }
}