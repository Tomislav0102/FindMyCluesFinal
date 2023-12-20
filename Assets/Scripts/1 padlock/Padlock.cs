using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Padlock : MileStone, IActivation
{
    [SerializeField] GameObject[] allRings;
    [SerializeField] Transform body, hook;

    [SerializeField] GameObject finalClue;

    [SerializeField] Material[] dissolveMats;
    DissolveManager _dissolveManager;

    int[] _currentValues;
    public bool IsActive { get; set; }

    bool _onScreen;
    public bool inPosition;
    ScreenPositioning _screenPositioning;
    readonly int[] _padlockSolution = new int[] { 2, 5, 6, 3 };

    readonly Vector2 _posY = new Vector2(0.572f, 1f);
    readonly Vector3 _startRot = new Vector3(90f, 180f, 0f);
    readonly Vector3 _endRot = new Vector3(18f, 180f, 0f);


    protected override void Awake()
    {
        base.Awake();
        myTransform.LookAt(new Vector3(gm.camTr.position.x, myTransform.position.y, gm.camTr.position.z));
        _dissolveManager = new DissolveManager(dissolveMats);
        _dissolveManager.DissolveMe(null, 1f, false);

        GetCode();

        _screenPositioning = new ScreenPositioning(body, gm.focusPoint.GetChild(gm.xp.currentLocation.ordinal));
    }
    void Update()
    {
        if(!IsActive || !_onScreen) return;
        _screenPositioning.UpdateLoop(ref inPosition);
    }
    public override void StartMe()
    {
        base.StartMe();
        if (IsActive) return;
        IsActive = true;

        gm.arFloor.SetActive(false);
        GameObject go = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation, myTransform.parent);
        go.GetComponent<ClueOnScreen>().CallBackAtEnd += () =>
        {
            _onScreen = true;
        };

    }

    void GetCode()
    {
        _currentValues = new int[allRings.Length];
        for (int i = 0; i < allRings.Length; i++)
        {
            _currentValues[i] = Random.Range(0, 9);
        }
        bool allAreEqual = true;
        for (int i = 0; i < _currentValues.Length; i++)
        {
            if (_currentValues[i] != _padlockSolution[i]) allAreEqual = false;
        }
        if (allAreEqual) GetCode();
        else
        {
            for (int i = 0; i < allRings.Length; i++)
            {
                allRings[i].GetComponent<PadlockRing>().Init(_currentValues[i]);
            }
        }
    }


    public void InputNumber(int ringIndex, int value)
    {
        if (!IsActive) return;

        _currentValues[ringIndex] = value;
        for (int i = 0; i < _currentValues.Length; i++)
        {
            if (_currentValues[i] != _padlockSolution[i]) return;
        }
        gm.characterTalking.CharacterInfoText("Well done!", Emotion.Smile);

        _onScreen = false;
        IsActive = false;
        hook.DOLocalMoveY(0.85f, 0.3f)
            .SetEase(Ease.OutBack)
            .OnComplete(Done1);

    }
    void Done1()
    {
        hook.DOLocalRotate(30f * Vector3.up, 0.3f).OnComplete(Done2);
    }
    void Done2()
    {
        _dissolveManager.DissolveMe(() =>
        {
            Instantiate(finalClue, spawnPoint.position, spawnPoint.rotation, myTransform.parent);
            gameObject.SetActive(false);
        }, 1f, true);
    }

    void OnDisable()
    {
        _dissolveManager.ResetMaterial();
    }

}