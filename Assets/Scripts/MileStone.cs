using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MileStone : MonoBehaviour
{
    protected GameManager gm;

    public Transform myTransform;
    public virtual void StartMe() { }
    public virtual void TickMe() { }
    public virtual void EndMe() { }
    public virtual void InMidRange() { }
    public virtual void InCloseRange() { }
    public virtual void CancelExperince() { }

    public GameObject prefabToSpawn;
    public Transform spawnPoint;

    protected virtual void Awake()
    {
        gm = GameManager.Instance;
    }
}