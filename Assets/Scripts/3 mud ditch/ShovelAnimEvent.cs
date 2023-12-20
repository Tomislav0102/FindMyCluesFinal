using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShovelAnimEvent : MonoBehaviour
{
    [SerializeField] MudDitch mudDitch;
    public void AE()
    {
        mudDitch.psDig.Play();
    }
}
