using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldKuwait : MileStone, IActivation
{
    public static System.Action<bool> PlayerInWorld;

    [SerializeField] GameObject[] pickups;
    [SerializeField] PortalManager portalMgr;
    [SerializeField] ParticleSystem psEnterExit;
    [Space]
    [SerializeField] MeshRenderer[] doorMeshes;
    Transform _doorLeft, _doorRight;
    [SerializeField] MeshRenderer oldKuwaitMesh, skyBoxMesh;

    [Header("Materials")]
    [SerializeField] Material matOldKuwWorld;
    [SerializeField] Material matOldKuwDissolve;
    [SerializeField] Material matSkyboxWorld;
    [SerializeField] Material matSkyboxDissolve;
    [SerializeField] Material matDoor;
    [SerializeField] Material matDoorDissolve;

    List<OldKuwaitPickups> _pickups = new List<OldKuwaitPickups>();
    public bool IsActive { get; set; }

    DissolveManager _dissolveManagerDoor;
    DissolveManager _dissolveManagerWorld;

    protected override void Awake()
    {
        base.Awake();
        myTransform.LookAt(Utilities.HorPos(gm.camTr.position));

        _dissolveManagerDoor = new DissolveManager(new Material[] { matDoorDissolve });
        _dissolveManagerDoor.DissolveMe(() =>
        {
            for (int i = 0; i < doorMeshes.Length; i++)
            {
                doorMeshes[i].sharedMaterial = matDoorDissolve;
            }
        }, 1f, false);
        _doorLeft = doorMeshes[1].transform;
        _doorRight = doorMeshes[2].transform;


        _dissolveManagerWorld = new DissolveManager(new Material[] { matOldKuwDissolve, matSkyboxDissolve });
        portalMgr.Init(new Material[] {matOldKuwWorld, matSkyboxWorld });
    }
    public override void StartMe()
    {
        base.StartMe();
        if (IsActive) return;
        IsActive = true;

        psEnterExit.transform.parent = myTransform.parent;
        DoorOpen(true, null);
        gm.arFloor.SetActive(false);
        gm.characterTalking.CharacterInfoText("Find coin and key before timer runs out...");

    }
    void OnEnable()
    {
        PlayerInWorld += (bool b) =>
        {
            gm.xp.currentLocation.IsActive = !b;
        };
        gm.cam.GetComponent<BoxCollider>().enabled = true;
    }
    void OnDisable()
    {
        PlayerInWorld -= (bool b) =>
        {
            gm.xp.currentLocation.IsActive = !b;
        };
        _dissolveManagerDoor.ResetMaterial();
        _dissolveManagerWorld.ResetMaterial();
        gm.cam.GetComponent<BoxCollider>().enabled = false;
    }
    void DoorOpen(bool open, System.Action callBackAtFinish, bool speedBased = false)
    {
        float time = speedBased ? 150f : 0.5f;
        Vector3 endLeft = open ? -90f * Vector3.up : Vector3.zero;
        Vector3 endRight = open ? 90f * Vector3.up : Vector3.zero;

        _doorLeft.DOLocalRotate(endLeft, time)
            .SetSpeedBased(speedBased);
        _doorRight.DOLocalRotate(endRight, time)
            .SetSpeedBased(speedBased)
            .OnComplete(() => callBackAtFinish?.Invoke());
    }

    public void GetItem(OldKuwaitPickups pu, bool showPopup = true)
    {
        if (!IsActive || _pickups.Contains(pu)) return;

        _pickups.Add(pu);
        gm.uImanager.AddItemInInventory(_pickups[_pickups.Count - 1].myInvItem, showPopup);
        if (_pickups.Count == 2)
        {
            gm.uImanager.displayDistance.text = "";
            portalMgr.GetComponent<IActivation>().IsActive = false;
            skyBoxMesh.sharedMaterial = matSkyboxDissolve;
            oldKuwaitMesh.sharedMaterial = matOldKuwDissolve;
            oldKuwaitMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _dissolveManagerWorld.DissolveMe(null, 0.5f);

            DoorOpen(false, () =>
            {
                psEnterExit.Play();
                gm.arFloor.SetActive(true);
                myTransform.DOLocalMoveY(-5, 5f)
                         .SetEase(Ease.OutQuad)
                         .OnComplete(NewLocation);
            });

            IsActive = false;
        }
    }
    void NewLocation()
    {
        psEnterExit.Stop();
        Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation, myTransform.parent);
        gameObject.SetActive(false);
    }

    public void TimeOut()
    {
        for (int i = 0; i < pickups.Length; i++)
        {
            GetItem(pickups[i].GetComponent<OldKuwaitPickups>(), false);
            pickups[i].GetComponent<IActivation>().IsActive = false;
        }
        gm.characterTalking.CharacterInfoText("Time is out. You got all items but your score will be reduced.", Emotion.Sad);
        gm.timeManager.ExtraTime += 60;
    }
}