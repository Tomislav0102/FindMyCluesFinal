using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JigsawManager : MileStone
{
    public static System.Action JigsawPuzzleDone;
    public static System.Action TilesCollected;
    Camera _cam;
    
    [SerializeField] LayerMask layTileSlot, layTileCollected, layBoard;
    RaycastHit _hit;
    JigCollected _carryJig;
    const int CONST_RAYDISTANCE = 50;
    Dictionary<JigCollected, JigSlot> _dicPairs = new Dictionary<JigCollected, JigSlot>();

    [SerializeField] Transform finalImage;
    [SerializeField] ParticleSystem psShowFinish;
    [SerializeField] Transform wholeTable, parUpper, parLower, parInAir, parStartingDeck;
    [SerializeField] BoxCollider boardCollider;
    [SerializeField] Sprite[] allPieces;
    JigCollected[] _jigs;
    Transform[] _jigsTransforms;
    JigSlot[] _slots;
    Transform[] _inAirTransforms;
    [HideInInspector] public Transform[] lowerTransforms;
    readonly Vector2Int _dimensions = new Vector2Int(6, 4);
    readonly Vector2 _offset = new Vector2(2.5f, 1.5f);
    readonly Vector2 _spread = new Vector2(1.4f, 1.9f);
    int _numOfTiles;
    int _counterCollect;
    bool _canPickuUpTiles, _tilesCollected, _tilesMovingToLower, _puzzleDone;
    float _moveTimer;

    DissolveManager _dissolveManager;
    ScreenPositioning _screenPositioning;
    bool _screenSwitch;
    [SerializeField] ParticleSystem psStart;

    public bool printDictionary;

    protected override void Awake()
    {
        base.Awake();
        myTransform.LookAt(Utilities.HorPos(gm.camTr.position));
        ExperienceManager.MinMaxDistance = new Vector2(4f, 35f);

        _cam = gm.cam;
        boardCollider.enabled = false;
        _dissolveManager = new DissolveManager(new Material[] { gm.matSpriteDissolve });
        finalImage.localScale = Vector3.zero;
        _numOfTiles = allPieces.Length;
    }
    private void Start()
    {
        ArrangeTable();
        ArrangePieces();
        ArrangeInAir();
        for (int i = 0; i < _numOfTiles; i++)
        {
            _jigs[i].myTransform.localPosition += 0.01f * Vector3.forward;
            _jigs[i].myTransform.localEulerAngles = Random.Range(0f, 360f) * Vector3.forward;
            _jigs[i].myRenderer.enabled = true;
        }
        _dissolveManager.DissolveMe(() => 
        {
            for (int i = 0; i < _numOfTiles; i++)
            {
                _jigs[i].myRenderer.sharedMaterial = gm.matSpriteDef;
            }
        }, 1f, false);
    }
    public override void StartMe()
    {
        gm.arFloor.SetActive(false);
        _screenPositioning = new ScreenPositioning(wholeTable, gm.focusPoint.GetChild(gm.xp.currentLocation.ordinal));
        gm.characterTalking.CharacterInfoText("There are puzzle pieces floating above. Tap on them to collect.");


        float time = 2f;
        psStart.Play();
        for (int i = 0; i < _numOfTiles; i++)
        {
            _jigs[i].myTransform.DOMove(_inAirTransforms[i].position, time);
            _jigs[i].myTransform.DORotateQuaternion(_inAirTransforms[i].rotation, time);
        }
        StartCoroutine(TilesAtPlace(time));
        for (int i = 0; i < _numOfTiles; i++)
        {
            _jigs[i].myRenderer.sharedMaterial = gm.matSpriteDef;
        }

        IEnumerator TilesAtPlace(float t)
        {
            yield return new WaitForSeconds(t);
            _canPickuUpTiles = true;

        }
    }


    private void OnEnable()
    {
        JigsawPuzzleDone += () => _puzzleDone = true;
    }
    private void OnDisable()
    {
        JigsawPuzzleDone -= () => _puzzleDone = true;
        _dissolveManager.ResetMaterial();
    }

    #region//SETUP
    void ArrangeTable()
    {
        _slots = new JigSlot[_numOfTiles];
        for (int i = 0; i < _numOfTiles; i++)
        {
            _slots[i] = parUpper.GetChild(i).GetComponent<JigSlot>();
            _slots[i].name = $"Slot {_slots[i].coors.x}-{_slots[i].coors.y}";
            _slots[i].jigsawManager = this;
        }
        for (int i = 0; i < _numOfTiles; i++)
        {
            _slots[i].transform.localPosition = new Vector3(_slots[i].coors.x - _offset.x, _slots[i].coors.y - _offset.y, 0f);
        }
     //   parUpper.localScale = new Vector3(6f/4f, 4f/6f, 1f);
    }
    void ArrangePieces()
    {
        _jigs = Utilities.GetallChildren<JigCollected>(parStartingDeck);
        _jigsTransforms = Utilities.GetallChildren<Transform>(parStartingDeck);
        for (int i = 0; i < _numOfTiles; i++)
        {
            _jigs[i].GetComponent<SpriteRenderer>().sprite = allPieces[_jigsTransforms[i].GetSiblingIndex()];
            _jigs[i].jigsawManager = this;
            _dicPairs.Add(_jigs[i], null);
            _jigs[i].coors = _slots[i].coors;
        }
        lowerTransforms = Utilities.GetallChildren<Transform>(parLower);
        for (int i = 0; i < _numOfTiles; i++)
        {
            _jigs[i].name = $"Tile {_jigs[i].coors.x}-{_jigs[i].coors.y}";
            lowerTransforms[i].localPosition = new Vector3(_spread.x * ( _jigs[i].coors.x - _offset.x), _spread.y * (_jigs[i].coors.y - _offset.y), 0f);
        }

    }
    void ArrangeInAir()
    {
        _inAirTransforms = Utilities.GetallChildren<Transform>(parInAir);
        for (int i = 0; i < _numOfTiles; i++)
        {
            _inAirTransforms[i].Rotate(Random.Range(0, 360) * Vector3.one);
            _inAirTransforms[i].position += Random.Range(-5f, 5f) * myTransform.forward;
        }
    }
    #endregion
    #region //UPDATE LOOPS
    private void Update()
    {
        if (printDictionary)
        {
            printDictionary = false;
            foreach (KeyValuePair<JigCollected, JigSlot> item in _dicPairs)
            {
                print($"{item.Key} ____ {item.Value}");
            }

        }

        if (_screenPositioning != null) _screenPositioning.UpdateLoop(ref _screenSwitch);
        if (_puzzleDone) return;

        if (_screenPositioning != null)
        {
            for (int i = 0; i < _numOfTiles; i++)
            {
                if (_jigs[i].jState != JigsawState.InAir) continue;
                _jigsTransforms[i].Rotate(20f * Time.deltaTime * Vector3.up, Space.Self);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out _hit, CONST_RAYDISTANCE, layTileCollected) && _hit.collider != null)
            {
                _carryJig = _hit.collider.GetComponent<JigCollected>();
                if (_carryJig.jState == JigsawState.InAir)
                {
                    CollectTile(_carryJig);
                    _carryJig = null;
                    return;
                }
                _carryJig.myRenderer.sortingOrder = 10;
                _dicPairs[_carryJig] = null;
            }
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (_carryJig == null) return;

            if (Physics.Raycast(gm.camTr.position, (_carryJig.myTransform.position - gm.camTr.position).normalized, out _hit, CONST_RAYDISTANCE, layTileSlot) && _hit.collider != null)
            {
                JigSlot slot = _hit.collider.GetComponent<JigSlot>();

                foreach (KeyValuePair<JigCollected, JigSlot> item in _dicPairs)
                {
                    if (item.Value == slot)
                    {
                        ReturnCollectedToLower();
                        return;
                    }
                }

                if (_dicPairs[_carryJig] == null)
                {
                    _dicPairs[_carryJig] = slot;
                    _carryJig.myTransform.SetPositionAndRotation(slot.myTransform.position, slot.myTransform.rotation);
                    _carryJig.psSet.Play();
                    _carryJig = null;
                    CheckPuzzleCompleted();
                }
                else ReturnCollectedToLower();
            }
            else ReturnCollectedToLower();
        }
    }
    private void FixedUpdate()
    {
        if (_puzzleDone) return;
        if (_carryJig == null) return;

        if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out _hit, CONST_RAYDISTANCE, layBoard) && _hit.collider != null)
        {
            _carryJig.myTransform.position = _hit.point + 2f * wholeTable.up;
        }

    }
    #endregion

    #region //UTILS
    public void CollectTile(JigCollected jig)
    {
        if (!_canPickuUpTiles)
        {
            gm.characterTalking.CharacterInfoText("You're too far away, come closer.", Emotion.Sad);
            return;
        }
        if (_tilesCollected || _puzzleDone || _tilesMovingToLower) return;

        jig.jState = JigsawState.Transit;
        jig.lowerPositionCounter = _counterCollect;
        _tilesMovingToLower = true;
        StartCoroutine(MoveTile(jig, lowerTransforms[_counterCollect], () =>
        {
            if (_counterCollect >= _numOfTiles)
            {
                TilesCollected?.Invoke();
                _tilesCollected = true;
                boardCollider.enabled = true;
                gm.characterTalking.CharacterStart(false);
                gm.characterTalking.CharacterInfoText("All puzzle pieces are collected. Now, solve the puzzle!", Emotion.Smile);
                gm.uImanager.displayDistance.transform.parent.gameObject.SetActive(false);
                for (int i = 0; i < _numOfTiles; i++)
                {
                    _slots[i].myRenderer.enabled = true;
                }
                Vector3 skejl = parUpper.localScale;
                parUpper.DOScale(skejl, 1f)
                          .From(Vector3.zero)
                          .SetEase(Ease.OutBack);
            }
            _tilesMovingToLower = false;
            jig.psTrail.Stop();
        }));
        _counterCollect++;
    }
    void ReturnCollectedToLower()
    {
        SpriteRenderer sr = _carryJig.myRenderer;
        StartCoroutine(MoveTile(_carryJig, lowerTransforms[_carryJig.lowerPositionCounter], () =>
        {
            sr.sortingOrder = 0;
        }));
        _carryJig = null;
    }

    IEnumerator MoveTile(JigCollected jig, Transform target, System.Action callback)
    {
        while(_moveTimer < 0.5f)
        {
            _moveTimer += Time.deltaTime * 2f;
            Vector3 pos = Vector3.Lerp(jig.myTransform.position, target.position, _moveTimer);
            Quaternion rot = Quaternion.Lerp(jig.myTransform.rotation, target.rotation, _moveTimer);
            jig.myTransform.SetPositionAndRotation(pos, rot);
            yield return null;
        }
        _moveTimer = 0f;
        jig.myTransform.SetPositionAndRotation(target.position, target.rotation);
        jig.myTransform.parent = target.parent;
        jig.jState = JigsawState.Collected;
        callback?.Invoke();
    }

    void CheckPuzzleCompleted()
    {
        foreach (KeyValuePair<JigCollected, JigSlot> item in _dicPairs)
        {
            if (item.Value == null || item.Key.coors != item.Value.coors) return;
        }

        JigsawPuzzleDone?.Invoke();
        _puzzleDone = true;
        for (int i = 0; i < _numOfTiles; i++)
        {
            _slots[i].myRenderer.enabled = false;
            _jigs[i].myRenderer.sharedMaterial = gm.matSpriteDissolve;
        }
        psShowFinish.Play();
        finalImage.GetComponent<SpriteRenderer>().sharedMaterial = gm.matSpriteDef;
        _dissolveManager.DissolveMe(() =>
        {
            finalImage.DOScale(new Vector3(1.2f, 0.8f, 1f), 2f)
                .SetEase(Ease.OutBounce)
                .OnComplete(() =>
                {
                    gm.characterTalking.CharacterStart();
                    gm.uImanager.displayDistance.transform.parent.gameObject.SetActive(true);
                    psShowFinish.Stop();
                    finalImage.GetComponent<SpriteRenderer>().sharedMaterial = gm.matSpriteDissolve;
                    _dissolveManager.DissolveMe(() =>
                    {
                      //  gm.xp.NewLocation();
                        Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation, myTransform.parent);
                        gameObject.SetActive(false);
                    }, 3f);
                });
        },0.5f);

    }

    #endregion
}
