using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ARLocation;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool build;
    [SerializeField] Image fadeInImage;
    [SerializeField] GameObject[] arObjects;
    [SerializeField] GameObject arCamera, editorCamera;
    [HideInInspector] public Camera cam;
    [HideInInspector] public Transform camTr;
    [HideInInspector] public Transform focusPoint;
    [HideInInspector] public ScriptableInv[] allInvItems;
    public CharacterTalking characterTalking;
    public GameObject arFloor;
    public ExperienceManager xp;
    public UImanager uImanager;
    public PlaceAtLocation placeAtLocation;
    public Material matSpriteDef, matSpriteDissolve;
    public TimeManager timeManager;
    public bool usePlayerPrefs;
    public static Lang Language;
    [HideInInspector] public bool gameEnd;
    private void Awake()
    {
        Instance = this;

        allInvItems = Resources.LoadAll<ScriptableInv>("InventoryItems");

        cam = build ? arCamera.GetComponent<Camera>() : editorCamera.GetComponent<Camera>();
        cam.GetComponent<BoxCollider>().enabled = false;
        camTr = cam.transform;
        focusPoint = camTr.GetChild(0);
        placeAtLocation.enabled = build;
        for (int i = 0; i < arObjects.Length; i++)
        {
            arObjects[i].SetActive(build);
        }

        timeManager = new TimeManager(uImanager.timeText);
        if (!usePlayerPrefs) ResetPlayerPrefs();

        fadeInImage.DOFade(0f, 2f)
            .From(1f);

        Language = (Lang)PlayerPrefs.GetInt(Utilities.Language);
    }

    void OnEnable()
    {
        Utilities.GameDone += EndGame;
        Utilities.NewLanguage += SetLanguage;
    }
    private void OnDisable()
    {
        Utilities.GameDone -= EndGame;
        Utilities.NewLanguage -= SetLanguage;
    }
    void SetLanguage(Lang l)
    {
        Language = l;
        PlayerPrefs.SetInt(Utilities.Language, (int)l);
    }
    void EndGame()
    {
        gameEnd = true;
        ResetPlayerPrefs();
    }
    void ResetPlayerPrefs()
    {
        PlayerPrefs.SetInt(Utilities.Ordinal, 0);
        PlayerPrefs.SetFloat(Utilities.TimeElapsed, 0);
        PlayerPrefs.SetFloat(Utilities.TimeExtra, 0);
    }
    private void Update()
    {
        if (gameEnd) return;
        timeManager.UpdateLoop();
    }

    public void TrackingStarted() => xp.IsTracking = true;
    public void TrackingLost() => xp.IsTracking = false;
    public void TrackingRestored() => xp.IsTracking = true;


    /// <summary>
    /// Player prefs should be uploaded to server after each completed experince and/or after game is completed. TimeElapsed and TimeExtra are to be summed, that is total time.
    /// Last line defines behaviour after game is completed. It would be nice if that behaviour is in "EndGame()" method and not here.
    /// </summary>
    /// <param name="currentOrdinal">Experince that has been completed</param>
    public void UpdateBackEnd(int currentOrdinal)
    {
        /*
        PlayerPrefs.GetInt(Utilities.Ordinal);
        PlayerPrefs.GetFloat(Utilities.TimeElapsed);
        PlayerPrefs.GetFloat(Utilities.TimeExtra);
        */

        if (currentOrdinal == 8) Utilities.GameDone?.Invoke();
    }
}

