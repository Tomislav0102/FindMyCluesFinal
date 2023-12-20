using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Chest : MileStone, IActivation
{
    float _chestStartScale;
    [SerializeField] Transform key;

    DissolveManager _dissolveManager;
    [SerializeField] Material matDissolve;
    [SerializeField] Transform lid;
    public bool IsActive { get; set; }
    bool _chestOpened;
    bool _inPosition;
    ScreenPositioning _screenPositioning;
    [SerializeField] Transform locationOfMap, endPosMap;
    [SerializeField] ParticleSystem ps;

    protected override void Awake()
    {
        base.Awake();
        myTransform.parent = gm.xp.currentLocation.myTransform;
        _chestStartScale = myTransform.localScale.x;
        key.gameObject.SetActive(false);
        _screenPositioning = new ScreenPositioning(myTransform, gm.focusPoint.GetChild(gm.xp.currentLocation.ordinal));
        _dissolveManager = new DissolveManager(new Material[] { matDissolve, gm.matSpriteDissolve });

    }
    void Start()
    {
        IsActive = true;
        myTransform.DOScale(_chestStartScale * Vector3.one, 1f)
            .From(Vector3.zero)
            .OnComplete(()=> gm.characterTalking.CharacterInfoText("Tap on the chest to unlock it with key from your inventory."));
    }

    void Update()
    {
        if (!IsActive) return;
        _screenPositioning.UpdateLoop(ref _inPosition);
    }


    private void OnMouseDown()
    {
        if (!IsActive || _chestOpened || !_inPosition) return;
        _chestOpened = true;

        key.gameObject.SetActive(true);
        key.DOLocalMoveZ(0f, 0.5f)
            .OnComplete(() =>
            {
                key.DOLocalRotate(300f * Vector3.forward, 0.5f)
                    .OnComplete(() =>
                    {
                        ps.Play();
                        lid.DOLocalRotate(-75f * Vector3.right, 1f)
                            .SetEase(Ease.OutBack)
                            .OnComplete(OpenChest);
                    });
            });

    }
    void OpenChest()
    {
        key.gameObject.SetActive(false);

        GameObject go = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation, myTransform.parent);
        go.GetComponent<ClueOnScreen>().CallBackAtEnd += () =>
        {
            ps.Stop();
            gm.characterTalking.CharacterInfoText("Congratulations you just finished your first AR Mystery Route Game. Go to the city steps for the finish line!", Emotion.Smile);
            _dissolveManager.DissolveMe(() =>
            {
                gameObject.SetActive(false);
            }, 0.5f);
        };
        //locationOfMap.DOJump(endPosMap.position, 0.5f, 1, 0.5f);
        //locationOfMap.DOScale(0.8f * Vector3.one, 1f)
        //    .OnComplete(() =>
        //    {
        //        StartCoroutine(FinalOpenChest());
        //    });
    }
    //IEnumerator FinalOpenChest()
    //{
    //    gm.uImanager.AddItemInInventory(mudDitchInvItem);
    //    yield return new WaitForSeconds(4f);
    //    ps.Stop();
    //    _dissolveManager.DissolveMe(() =>
    //    {
    //        gm.xp.NewLocation();
    //        gm.arFloor.SetActive(true);
    //        gameObject.SetActive(false);
    //    }, 3f);
    //}
    void OnDisable()
    {
        _dissolveManager.ResetMaterial();
    }
}
