using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class test : MonoBehaviour
{
    public LayerMask mask;

    private void Awake()
    {
        gameObject.layer = mask;
    }
}







/*
public CharacterTalking characterTalking;
public bool hit;

private void Start()
{
    characterTalking.CharacterStart();
}

private void Update()
{
    if (hit)
    {
        hit = false;
        characterTalking.CharacterInfoText("kdjfsklsdfj");
    }
}


public int prvi, drugi, rez;


private void Update()
{
    rez = prvi % drugi;
}

*/