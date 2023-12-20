using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Whale : MileStone, IActivation
{
    [SerializeField] Animator anim;
    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;
    DissolveManager _dissolveManager;
    Vector3 _center;
    public bool IsActive { get; set; }
    float _timerToActivate;
    const int CONST_MAXTIME = 45;

    protected override void Awake()
    {
        base.Awake();
        _dissolveManager = new DissolveManager(new Material[] { skinnedMeshRenderer.sharedMaterial });
    }
    public override void StartMe()
    {
        base.StartMe();
        if (IsActive) return;
        IsActive = true;
        gm.uImanager.GetInfoText("Look up!\nThere's a whale swimming above.");
        _center = gm.xp.currentLocation.myTransform.position;
    }

    private void Update()
    {
        if (!IsActive) return;
        myTransform.RotateAround(_center, Vector3.up, -5f * Time.deltaTime);

        _timerToActivate += Time.deltaTime;
        if(_timerToActivate > CONST_MAXTIME)
        {
            TickMe();
        }
    }
    void OnDisable()
    {
        _dissolveManager.ResetMaterial();
    }
    private void OnMouseDown()
    {
        TickMe();
    }
    public override void TickMe()
    {
        base.TickMe();
        anim.SetTrigger("openMouth");

    }
    public void CallBackAnimEvent()
    {
        IsActive = false;

        GameObject experince = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation, transform.parent);

        Guide g = experince.GetComponent<Guide>();

        g.landingPoints[0] = spawnPoint.position;

        Vector3 middlePos = new Vector3(spawnPoint.position.x, 0f, spawnPoint.position.z) + 20f * myTransform.forward;
        middlePos.y = 0f;
        g.landingPoints[1] = middlePos;

        Vector3 lastPos = new Vector3(gm.camTr.position.x, 0f, gm.camTr.position.z) + 8f * gm.camTr.forward;
        lastPos.y = 0f;
        g.landingPoints[2] = lastPos;

        g.StartMe();

        _dissolveManager.DissolveMe(() => { gameObject.SetActive(false); }, 4f);
    }

}
