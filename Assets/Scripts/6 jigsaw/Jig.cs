using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Jig : MonoBehaviour, IActivation
{
    public JigsawManager jigsawManager;
    public Vector2Int coors;
    public Transform myTransform;
    public SpriteRenderer myRenderer;
    [SerializeField] protected TextMeshPro[] visualCoordinates;
    [SerializeField] protected BoxCollider boxCollider;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            boxCollider.enabled = _isActive;
        }
    }
    bool _isActive;

    protected virtual void Awake()
    {

    }
    void Start()
    {
        for (int i = 0; i < visualCoordinates.Length; i++)
        {
            visualCoordinates[i].text = $"{coors.x}{coors.y}";
        }

    }

    protected virtual void OnEnable()
    {
        JigsawManager.JigsawPuzzleDone += () => IsActive = false;
    }
    protected virtual void OnDisable()
    {
        JigsawManager.JigsawPuzzleDone -= () => IsActive = false;
    }

}
