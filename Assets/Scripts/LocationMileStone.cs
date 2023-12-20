using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationMileStone : MileStone, IActivation
{
    MileStone _next;

    enum InstantiateRange
    {
        Mid,
        Close
    }
    [SerializeField] InstantiateRange range;
    bool _midOneHit;
    bool _closeOneHit;
    [Header("Location specific")]
    public string nameOfLocation;
    [field:SerializeField] public bool IsActive { get; set; }
    [HideInInspector] public int ordinal;

    public bool hasCustomRange;
    [HideInInspector] public Vector2 customRange = new Vector2(4f, 10f);
    protected override void Awake()
    {
        base.Awake();
        IsActive = true;
        ordinal = myTransform.GetSiblingIndex();
    }

    public override void TickMe() //used only when restarting game from PlayerPrefs
    {
        base.TickMe();
        for (int i = 0; i < gm.allInvItems.Length; i++)
        {
            if (gm.allInvItems[i].experinceOrdinal < ordinal) gm.uImanager.AddItemInInventory(gm.allInvItems[i], false);
        }
    }
    public override void InMidRange()
    {
        if (!gm.xp.IsTracking || !IsActive) return;
        base.InMidRange();

        if (!_midOneHit)
        {
            if (_next == null)
            {
                if (range == InstantiateRange.Mid)
                {
                    SpawnObject();
                }
            }
        }

        _midOneHit = true;
        _closeOneHit = false;
    }
    public override void InCloseRange()
    {
        if (!gm.xp.IsTracking || !IsActive) return;
        base.InCloseRange();

        if (!_closeOneHit)
        {
            if (_next == null)
            {
                SpawnObject();
            }
            _next.StartMe();
        }

        _midOneHit = false;
        _closeOneHit = true;
    }
    void SpawnObject()
    {
        gm.characterTalking.CharacterInfoText($"You've reached {nameOfLocation}.\nMove your phone to find AR experince");

        if (prefabToSpawn == null)
        {
            print("No experince to spawn, this is end");
            return;
        }
        gm.arFloor.SetActive(true);
        GameObject experince = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation, myTransform);
        experince.name = prefabToSpawn.name;
        _next = experince.GetComponent<MileStone>();
    }

    public override void CancelExperince()
    {
        if (!gm.xp.IsTracking || !IsActive || _next == null) return;
        base.CancelExperince();
        _midOneHit = _closeOneHit = false;

        for (int i = 0; i < myTransform.childCount; i++)
        {
            if (myTransform.GetChild(i).CompareTag("SpawnPoint")) continue;
            Destroy(myTransform.GetChild(i).gameObject);
        }
        for (int i = 0; i < gm.allInvItems.Length; i++)
        {
            if (gm.allInvItems[i].experinceOrdinal == ordinal) gm.uImanager.RemoveItemFromInventory(gm.allInvItems[i]);
        }
        gm.uImanager.ShowHideUI(false, CanvasType.All);
        if (!hasCustomRange) gm.xp.ResetDistance();
        gm.arFloor.SetActive(true);
        if (ordinal > 0) gm.characterTalking.CharacterStart();
    }
}
