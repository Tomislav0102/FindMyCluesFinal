using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UImanager : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI infoText;
    Transform _infoTextTransform;

    [SerializeField] Transform talkUI;
    [SerializeField] GameObject mainUIgame;
    [SerializeField] GameObject[] specificUIgame;

    [SerializeField] Image bkgProximity;
    readonly Color coldCol = Color.blue;
    readonly Color hotCol = Color.red;
    Tween _tweenBloatBackground;

    [Header("Inventory")]
    [SerializeField] TextMeshProUGUI displayNameItemEnlarged;
    [SerializeField] GameObject[] inventorySlotsScreens;
    List<InvSlot> _allInvSlots = new List<InvSlot>();
    List<GameObject> _invSlotsGOs = new List<GameObject>();
    [SerializeField] Transform centerScreen;
    List<ScriptableInv> _acquiredItems = new List<ScriptableInv>();
    [SerializeField] Transform parProgressLines, parCurrentLocationSquares;
    [SerializeField] Transform parInventoryCameras;
    [SerializeField] GameObject[] changeInvScreen;
    Button[] _btnChangeInvScreens;
    int _currentInvSlotScreenOpen;

    [Header("Buttons")]
    [SerializeField] Button[] selectUIbuttons;
    Image[] _selectUIhighlights;
    [SerializeField] Button btnInventory, btnLeaderboard;
    [SerializeField] Button[] btnsCloseUI;

    [Header("AR")]
    public TextMeshProUGUI displayDistance;
    [SerializeField] Button changeButton;
    [SerializeField] GameObject selectLocationWindow;
    [SerializeField] Transform parConsole;
    [SerializeField] Button btnToggleConsole;
    TextMeshProUGUI[] _consoleLines;
    int _counterConsoleLine;



    #region MONO
    private void Awake()
    {
        bkgProximity.color = coldCol;
        _tweenBloatBackground = bkgProximity.transform.DOScale(1.01f * Vector3.one, 1f)
            .SetLoops(-1, LoopType.Yoyo);

        _infoTextTransform = infoText.transform;
        _infoTextTransform.localScale = Vector3.zero;

        #region INVENTORY
        for (int i = 0; i < inventorySlotsScreens.Length; i++)
        {
            for (int j = 0; j < inventorySlotsScreens[i].transform.childCount; j++)
            {
                Transform ch = inventorySlotsScreens[i].transform.GetChild(j);
                _allInvSlots.Add(ch.GetComponent<InvSlot>());
                _invSlotsGOs.Add(ch.gameObject);
            }
        }
        _btnChangeInvScreens = new Button[2];
        for (int i = 0; i < 2; i++)
        {
            _btnChangeInvScreens[i] = changeInvScreen[i].GetComponentInChildren<Button>();
            changeInvScreen[i].SetActive(false);
        }
        #endregion

        _selectUIhighlights = new Image[selectUIbuttons.Length];
        for (int i = 0; i < _selectUIhighlights.Length; i++)
        {
            _selectUIhighlights[i] = selectUIbuttons[i].GetComponent<Image>();
        }

        #region AR
        _consoleLines = Utilities.GetallChildren<TextMeshProUGUI>(parConsole);
        for (int i = 0; i < _consoleLines.Length; i++)
        {
            _consoleLines[i].text = " ";
        }
        selectLocationWindow.SetActive(false);
        #endregion
    }
    void Start()
    {
        ShowHideUI(false, CanvasType.All);
        ChangeInventorySlotsScreens();
    }

    void OnEnable()
    {
        btnInventory.onClick.AddListener(ToggleInventory);
        btnLeaderboard.onClick.AddListener(()=>ShowHideUI(true, CanvasType.SpecificOnly, 2));
        for (int i = 0; i < btnsCloseUI.Length; i++)
        {
            btnsCloseUI[i].onClick.AddListener(()=>
            {
                ShowHideUI(false, CanvasType.SpecificOnly);
            });
        }

        _btnChangeInvScreens[0].onClick.AddListener(() => SwitchNextInventoryWindow());
        _btnChangeInvScreens[1].onClick.AddListener(() => SwitchNextInventoryWindow());

        //unity bug, can't use loops
        selectUIbuttons[0].onClick.AddListener(() => ShowHideUI(true, CanvasType.SpecificOnly, 0));
        selectUIbuttons[1].onClick.AddListener(() => ShowHideUI(true, CanvasType.SpecificOnly, 1));
        selectUIbuttons[2].onClick.AddListener(() => ShowHideUI(true, CanvasType.SpecificOnly, 2));
        selectUIbuttons[3].onClick.AddListener(() => ShowHideUI(true, CanvasType.SpecificOnly, 3));
        selectUIbuttons[4].onClick.AddListener(() => ShowHideUI(true, CanvasType.SpecificOnly, 4));


        changeButton.onClick.AddListener(BtnChange);
        btnToggleConsole.onClick.AddListener(() =>
        {
            parConsole.gameObject.SetActive(!parConsole.gameObject.activeInHierarchy);
        });
    }

    void OnDisable()
    {
        btnInventory.onClick.RemoveListener(ToggleInventory);
        btnLeaderboard.onClick.RemoveAllListeners();
        for (int i = 0; i < btnsCloseUI.Length; i++)
        {
            btnsCloseUI[i].onClick.RemoveAllListeners();
        }
        for (int i = 0; i < selectUIbuttons.Length; i++)
        {
            selectUIbuttons[i].onClick.RemoveAllListeners();
        }
        for (int i = 0; i < _btnChangeInvScreens.Length; i++)
        {
            _btnChangeInvScreens[i].onClick.RemoveAllListeners();
        }
        changeButton.onClick.RemoveAllListeners();
        btnToggleConsole.onClick.RemoveAllListeners();
        _tweenBloatBackground.Kill();

    }
    #endregion

    public void Proximity(bool showBorder, string targetName = "", float dist = 0f)
    {
        bkgProximity.enabled = showBorder;
        if (!showBorder) return;

        displayDistance.text = $"{targetName} is {dist:0.0} meters away";
        dist -= ExperienceManager.MinMaxDistance.y;
        dist /= 40f;
        bkgProximity.color = Color.Lerp(hotCol, coldCol, dist);
    }
    public void GetInfoText(string info)
    {
        infoText.text = info;
        _infoTextTransform.DOScale(Vector3.one, 1f)
                            .OnComplete(() =>
                            {
                                _infoTextTransform.DOScale(Vector3.zero, 1f)
                                                    .SetDelay(5f);
                            });
    }


    #region INVENTORY
    void SwitchNextInventoryWindow()
    {
        _currentInvSlotScreenOpen = (1 + _currentInvSlotScreenOpen) % 2;
        ChangeInventorySlotsScreens();
    }
    public void AddItemInInventory(ScriptableInv invData, bool showPopupMessage = true)
    {
        if (_acquiredItems.Contains(invData)) return;
        _acquiredItems.Add(invData);

        ApplyChangesToInventory(showPopupMessage);
    }
    public void RemoveItemFromInventory(ScriptableInv invData)
    {
        if (!_acquiredItems.Contains(invData)) return;
        _acquiredItems.Remove(invData);

        ApplyChangesToInventory(false);
    }
    public bool ItemInInventory(ScriptableInv invData)
    {
        if (_acquiredItems.Contains(invData)) return true;
        return false;
    }
    public void DisplayInvItems(bool displayAll, int ordinal = 99)
    {
        if (displayAll)
        {
            for (int i = 0; i < _acquiredItems.Count; i++)
            {
                _invSlotsGOs[i].SetActive(true);
            }

            ChangeInventorySlotsScreens();
        }
        else
        {
            for (int i = 0; i < changeInvScreen.Length; i++)
            {
                changeInvScreen[i].SetActive(false);
            }

            if (ordinal > _acquiredItems.Count - 1) return;
            for (int i = 0; i < _acquiredItems.Count; i++)
            {
                _invSlotsGOs[i].SetActive(false);
            }
            _invSlotsGOs[ordinal].SetActive(true);
        }
    }
    void ApplyChangesToInventory(bool showPopupMessage)
    {
        for (int i = 0; i < _invSlotsGOs.Count; i++)
        {
            _invSlotsGOs[i].SetActive(false);
        }
        for (int i = 0; i < _acquiredItems.Count; i++)
        {
            _invSlotsGOs[i].SetActive(true);
            _allInvSlots[i].AddItem(_acquiredItems[i], centerScreen, displayNameItemEnlarged, i);
            if (showPopupMessage && i == _acquiredItems.Count - 1) GameManager.Instance.characterTalking.CharacterInfoText($"New item in inventory:\n{_acquiredItems[i].invName}!", Emotion.Smile);
        }

        ChangeInventorySlotsScreens();
    }
    void ChangeInventorySlotsScreens()
    {
        for (int i = 0; i < inventorySlotsScreens.Length; i++)
        {
            inventorySlotsScreens[i].SetActive(false);
        }
        inventorySlotsScreens[_currentInvSlotScreenOpen].SetActive(true);


        for (int i = 0; i < changeInvScreen.Length; i++)
        {
            changeInvScreen[i].SetActive(false);
        }
        if (_acquiredItems.Count > 9) changeInvScreen[_currentInvSlotScreenOpen].SetActive(true);

    }
    public void ToggleInventory()
    {
        if (!specificUIgame[4].activeInHierarchy) ShowHideUI(true, CanvasType.SpecificOnly, 4);
        else ShowHideUI(false, CanvasType.SpecificOnly, 4);
    }

    #endregion


    public void ShowHideUI(bool show, CanvasType element, int ordinal = 99)
    {
        for (int i = 0; i < _allInvSlots.Count; i++)
        {
            _allInvSlots[i].ForceDefaultPosition();
        }
        Vector3 endScale = show ? Vector3.one : Vector3.zero;
        switch (element)
        {
            case CanvasType.All:
                Method(show, ordinal);
                talkUI.localScale = endScale;
                break;

            case CanvasType.SpecificOnly:
                Method(show, ordinal);
                break;

            case CanvasType.TalkOnly:
                talkUI.localScale = endScale;
                break;
        }

        for (int i = 0; i < _selectUIhighlights.Length; i++)
        {
            _selectUIhighlights[i].color = new Color(_selectUIhighlights[i].color.r, _selectUIhighlights[i].color.g, _selectUIhighlights[i].color.b, 0f);
        }
        if (ordinal < _selectUIhighlights.Length) _selectUIhighlights[ordinal].color = new Color(_selectUIhighlights[ordinal].color.r, _selectUIhighlights[ordinal].color.g, _selectUIhighlights[ordinal].color.b, show ? 1f : 0f);

        void Method(bool show, int ordinal)
        {
            mainUIgame.SetActive(show);
            for (int i = 0; i < specificUIgame.Length; i++)
            {
                specificUIgame[i].SetActive(false);
            }
            if (show && ordinal < specificUIgame.Length) specificUIgame[ordinal].SetActive(true);
        }
    }


    public void ProgressUpdate(int ordinal)
    {
        for (int i = 0; i < parProgressLines.childCount; i++)
        {
            if (i <= ordinal)
            {
                parProgressLines.GetChild(i).GetChild(0).gameObject.SetActive(true);
                parProgressLines.GetChild(i).GetChild(1).gameObject.SetActive(false);
                
                int ordinalCurrent = Mathf.Min(i + 1, parCurrentLocationSquares.childCount - 1);
                parCurrentLocationSquares.GetChild(ordinalCurrent).GetChild(0).gameObject.SetActive(true);
            }
        }
    }
    #region AR
    void BtnChange()
    {
        selectLocationWindow.SetActive(true);
    }

    public void NewLineInConsole(string st)
    {
        for (int i = _consoleLines.Length - 1; i > 0; i--)
        {
            _consoleLines[i].text = _consoleLines[i - 1].text;
        }
        _consoleLines[0].text = $"{_counterConsoleLine} - {st}";
        _counterConsoleLine++;
    }
    #endregion
}
