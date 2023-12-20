using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalWorldCollider : MonoBehaviour, IActivation
{
    GameManager _gm;
    [SerializeField] PortalManager portalManager;
    [SerializeField] OldKuwait oldKuwait;

    const int CONST_MAXTIME = 50;
    float _timer, _seconds;
    public bool IsActive { get; set; }
    bool _inside;

    private void Awake()
    {
        _gm = GameManager.Instance;
        IsActive = true;
        _timer = CONST_MAXTIME;
    }
    void OnEnable()
    {
        OldKuwait.PlayerInWorld += PlayerInWolrd;
    }
    void OnDisable()
    {
        OldKuwait.PlayerInWorld -= PlayerInWolrd;
    }
    void PlayerInWolrd(bool inside)
    {
        _inside = inside;
        if (!_inside)
        {
            _timer = CONST_MAXTIME;
        }
    }
    private void Update()
    {
        if (!IsActive || !_inside) return;

        _timer -= Time.deltaTime;
        _seconds = _timer % 60;
        _seconds = Mathf.FloorToInt(_seconds);
        string zerBeforeSeconds = _seconds <= 9 ? "0" : "";
        if(_seconds > 0)
        {
            _gm.characterTalking.CharacterInfoText($"Old Kuwait will disappear in\n{zerBeforeSeconds}{_seconds} seconds...", Emotion.Neutral, true);
        }
        else
        {
            oldKuwait.TimeOut();
            IsActive = false;
            gameObject.SetActive(false);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if(!IsActive) return;
        _gm.characterTalking.CharacterInfoText("You've left the Old Kuwait before finding items, return to old door.", Emotion.Sad);
        OldKuwait.PlayerInWorld?.Invoke(false);
    }

}
