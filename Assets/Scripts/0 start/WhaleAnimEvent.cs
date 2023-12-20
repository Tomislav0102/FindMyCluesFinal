using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleAnimEvent : MonoBehaviour
{
    [SerializeField] Whale whale;



    public void AE_OpenMouth()
    {
        whale.CallBackAnimEvent();
    }
}
