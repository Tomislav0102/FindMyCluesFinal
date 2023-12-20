using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OldKuwaitPickups : MonoBehaviour, IActivation
{
    [SerializeField] OldKuwait oldKuwait;
    [SerializeField] ParticleSystem ps;
    public ScriptableInv myInvItem;

    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            if (ps == null) return;
            if (_isActive) ps.Play();
            else ps.Stop();
        }
    }
    bool _isActive;
    private void OnMouseDown()
    {
        if (!IsActive) return;
        IsActive = false;
        oldKuwait.GetItem(this);
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        OldKuwait.PlayerInWorld += (bool b) => IsActive = b;
    }
    void OnDisable()
    {
        OldKuwait.PlayerInWorld -= (bool b) => IsActive = b;
    }


}
