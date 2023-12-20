using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;

public class PortalManager : MonoBehaviour, IActivation
{
    GameManager _gm;
    Transform _myTransform;
    Transform _camTr;
    Camera _cam;
    Material[] _worldMats;
    Material _portalPLaneMaterial;
    [SerializeField] PortalWorldCollider portalWorldCollider;
    bool _inside;
    [SerializeField] Material[] otherMatsInWorld;
    int _stencil = Shader.PropertyToID("_StencilComp");
    int _cull = Shader.PropertyToID("_CullMode");
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            portalWorldCollider.IsActive = value;
        }
    }
    bool _isActive;

    public void Init(Material[] worldMats)
    {
        _myTransform = transform;
        _gm = GameManager.Instance;
        _camTr = _gm.camTr;
        _cam = _gm.cam;

        _worldMats = worldMats;
        _portalPLaneMaterial = GetComponent<Renderer>().sharedMaterial;
        IsActive = true;

        OldKuwait.PlayerInWorld += PlayerinWorld;
    }

    private void OnDisable()
    {
        OldKuwait.PlayerInWorld -= PlayerinWorld;
    }

    void PlayerinWorld(bool inside)
    {
        _inside = inside;
        if (!_inside)
        {
            //enable stencil test
            for (int i = 0; i < _worldMats.Length; i++)
            {
                _worldMats[i].SetInt(_stencil, (int)CompareFunction.Equal);
            }
            _portalPLaneMaterial.SetInt(_cull, (int)CullMode.Back);

            for (int i = 0; i < otherMatsInWorld.Length; i++)
            {
                otherMatsInWorld[i].SetInt(_stencil, (int)CompareFunction.Equal);
            }


        }

    }
    private void OnTriggerStay(Collider other)
    {

        Vector3 worldPOs = _camTr.position + _camTr.forward * _cam.nearClipPlane;
        Vector3 camPositionInPortalSpace = _myTransform.InverseTransformPoint(worldPOs);

        if(camPositionInPortalSpace.y <= 0f) //negative value means cam is inside building
        {
            for (int i = 0; i < _worldMats.Length; i++)
            {
                _worldMats[i].SetInt(_stencil, (int)CompareFunction.NotEqual);
            }
            _portalPLaneMaterial.SetInt(_cull, (int)CullMode.Front);
            if(!_inside) OldKuwait.PlayerInWorld?.Invoke(true);

            for (int i = 0; i < otherMatsInWorld.Length; i++)
            {
                otherMatsInWorld[i].SetInt(_stencil, (int)CompareFunction.NotEqual);
            }

        }
        else if (camPositionInPortalSpace.y < 0.5f)
        {
            Vector3 camDirOnHorizontalPlane = Vector3.ProjectOnPlane(_camTr.forward, Vector3.up);

            bool facingPortal = Vector3.Dot(camDirOnHorizontalPlane, _myTransform.up) < 0f;
            CompareFunction komp = facingPortal ? CompareFunction.NotEqual : CompareFunction.Equal;
            CullMode kal = facingPortal ? CullMode.Front: CullMode.Off;

            //disable stencil test
            for (int i = 0; i < _worldMats.Length; i++)
            {
                _worldMats[i].SetInt(_stencil, (int)komp);
            }
            _portalPLaneMaterial.SetInt(_cull, (int)kal);
            if (!_inside) OldKuwait.PlayerInWorld?.Invoke(true);

            for (int i = 0; i < otherMatsInWorld.Length; i++)
            {
                otherMatsInWorld[i].SetInt(_stencil, (int)komp);
            }

        }
        else
        {
            
            OldKuwait.PlayerInWorld?.Invoke(false);
        }

    }

    private void OnDestroy()
    {
        PlayerinWorld(false);
    }

}
