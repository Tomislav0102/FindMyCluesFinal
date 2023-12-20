using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueOnScreen : MileStone, IActivation
{
    [SerializeField] bool forDisplayOnly;
    [SerializeField] ScriptableInv invItem;
    Renderer[] _meshes;
    [SerializeField] Material matGame, matDissolve;
    DissolveManager _dissolveManager;
    ScreenPositioning _screenPositioning;
    bool _inPosition;
    public bool IsActive { get; set; }
    [SerializeField] bool startNewLocation = true;
    public System.Action CallBackAtEnd;

    protected override void Awake()
    {
        base.Awake();
        if (!forDisplayOnly)
        {
            _screenPositioning = new ScreenPositioning(myTransform, gm.focusPoint.GetChild(gm.xp.currentLocation.ordinal).GetChild(0));
            _dissolveManager = new DissolveManager(new Material[] { matDissolve });
            myTransform.DOScale(0.4f * Vector3.one, 1f)
                .From(Vector3.zero);

            IsActive = true;
        }

        _meshes = new Renderer[2];
        for (int i = 0; i < _meshes.Length; i++)
        {
            _meshes[i] = myTransform.GetChild(i).GetComponent<Renderer>();
            _meshes[i].enabled = false;
        }
        _meshes[(int)GameManager.Language].enabled = true;
    }

    private void Update()
    {
        if(forDisplayOnly) return;
        _screenPositioning.UpdateLoop(ref _inPosition);
    }
    void OnEnable()
    {
        Utilities.NewLanguage += Language;
    }
    private void OnDisable()
    {
        Utilities.NewLanguage -= Language;
        if (!forDisplayOnly) _dissolveManager.ResetMaterial();
    }
    void Language(Lang l)
    {
        for (int i = 0; i < _meshes.Length; i++)
        {
            _meshes[i].enabled = false;
        }
        _meshes[(int)l].enabled = true;
    }
    public override void EndMe()
    {
        base.EndMe();
        for (int i = 0; i < _meshes.Length; i++)
        {
            _meshes[i].sharedMaterial = matDissolve;
        }
        _dissolveManager.DissolveMe(() =>
        {
            gm.uImanager.AddItemInInventory(invItem);
            if (startNewLocation) gm.xp.NewLocation();
            CallBackAtEnd?.Invoke();

            gameObject.SetActive(false);
        });
    }

    private void OnMouseDown()
    {
        if (!_inPosition || !IsActive || forDisplayOnly) return;
        IsActive = false;
        EndMe();
    }

}
