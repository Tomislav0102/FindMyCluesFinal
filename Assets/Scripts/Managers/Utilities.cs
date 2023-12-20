using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using ARLocation;


public class Utilities 
{
    public static System.Action GameDone;
    public static System.Action<Lang> NewLanguage;

    public static string Language = "language"; //int (0-english, 1-arabic)
    public static string Ordinal = "ordinal"; //int
    public static string TimeElapsed = "timeElapsed"; //float
    public static string TimeExtra = "extraTime"; //int

    public static void ShowCursor(bool show)
    {
        Cursor.visible = show;
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public static Vector2 GetWorldPositionOfCanvasElement(RectTransform rectElement) //sets gameobject behind the UI element
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectElement, rectElement.position, null, out Vector3 result);
        return result;
    }
    public static T[] GetallChildren<T>(Transform par) where T : Component
    {
        T[] values = new T[par.childCount];
        for (int i = 0; i < par.childCount; i++)
        {
            values[i]= par.GetChild(i).GetComponent<T>();
        }

        return values;
    }
    public static GameObject[] GetallChildrenGameobjects(Transform par)
    {
        GameObject[] values = new GameObject[par.childCount];
        for (int i = 0; i < par.childCount; i++)
        {
            values[i]= par.GetChild(i).gameObject;
        }

        return values;
    }
    public static void FillAnimStateNames(ref int[] idleNames, ref int[] talkNames, int numOfElements)
    {
        idleNames = new int[numOfElements];
        talkNames = new int[numOfElements];

        for (int i = 0; i < numOfElements; i++)
        {
            string ii = "i" + i.ToString();
            idleNames[i] = Animator.StringToHash(ii);
            string tt = "t" + i.ToString();
            talkNames[i] = Animator.StringToHash(tt);
        }

    }
    public static PlaceAtLocation.PlaceAtOptions Opt()
    {
        PlaceAtLocation.PlaceAtOptions oo = new PlaceAtLocation.PlaceAtOptions()
        {
            HideObjectUntilItIsPlaced = true,
            MaxNumberOfLocationUpdates = 4,
            MovementSmoothing = 0.1f,
            UseMovingAverage = false
        };

        return oo;
    }
    public static double GetDecimal(Vector3 degreesMinutesSeconds)
    {
        double fl = 0;
        fl += degreesMinutesSeconds.z / 3600;
        fl += degreesMinutesSeconds.y / 60;
        fl += degreesMinutesSeconds.x;
        return fl;
    }

    public static Vector3 GetDMS(double decimalNumber)
    {
        float deg = Mathf.FloorToInt((float)decimalNumber);
        double remainderDeg = decimalNumber % deg;

        double min = remainderDeg * 60;
        float minmin = Mathf.FloorToInt((float)min);
        double remMin = min % minmin;
        double sec = remMin * 60;


        return new Vector3(deg, minmin, (float)sec);
    }

    public static Vector3 BezierCurvePosition(Vector3[] points, float delatTime)
    {
        Vector3 ab = Vector3.Lerp(points[0], points[1], delatTime);
        Vector3 bc = Vector3.Lerp(points[1], points[2], delatTime);

        return Vector3.Lerp(ab, bc, delatTime);
    }
    public static Vector3 HorPos(Vector3 pos) => new Vector3(pos.x, 0f, pos.z);
    public static Vector3 RdnPosition()
    {
        Vector3 rdnPos = 10f * Random.insideUnitCircle;
        rdnPos.z = rdnPos.y;
        rdnPos.y = 0f;

        return rdnPos;
    }
    public static Vector3 RdnPositionInFront(Vector3 myPos)
    {
        Vector3 dir = (myPos - GameManager.Instance.camTr.position).normalized;
        dir.y = 0f;

        return myPos + Random.Range(3f, 6f) * dir;
    }
}

#region ENUMS
public enum Lang
{
    English,
    Arabic
}
public enum CanvasType
{
    All,
    SpecificOnly,
    TalkOnly
}
public enum Emotion
{
    Neutral,
    Sad,
    Smile
}
public enum JigsawState
{
    InAir,
    Collected,
    Set,
    Transit
}
#endregion


#region INTERFACES
public interface IActivation
{
    bool IsActive { get; set; }
}
public interface IMonoSim
{
    void AwakeListener();
    void StartListener();
    void OnEnableListener();
    void OnDisableListener();
    void UpdateListener();
}
#endregion

#region CLASSES
public class DissolveManager
{
    Material[] _mats;
    float _startingDissolveValue;

    float DissolveAmmount
    {
        get => _dissolveAmmount;
        set
        {
            _dissolveAmmount = value;
            for (int i = 0; i < _mats.Length; i++)
            {
                _mats[i].SetFloat(_amount, value);
            }
        }
    }
    float _dissolveAmmount;
    int _amount = Shader.PropertyToID("_DissolveAmount");
    int _color = Shader.PropertyToID("_DissolveColor");

    public DissolveManager(Material[] mats)
    {
        _mats = mats;
        _startingDissolveValue = _mats[0].GetFloat(_amount);
    }

    public void DissolveMe(System.Action callbackAtFinish, float time = 1f, bool dissolve = true)
    {
        for (int i = 0; i < _mats.Length; i++)
        {
            _mats[i].SetColor(_color, dissolve ? Color.yellow : Color.blue);
        }
        float startValue = dissolve ? 0f : 1f;
        float endValue = dissolve ? 1f : 0f;
        DOTween.To(() => DissolveAmmount, x => DissolveAmmount = x, endValue, time)
            .From(startValue)
            .OnComplete(() =>
            {
               callbackAtFinish?.Invoke();
            });
    }
    public void ResetMaterial()
    {
        for (int i = 0; i < _mats.Length; i++)
        {
            _mats[i].SetFloat(_amount, _startingDissolveValue);
        }

    }
}

public class ScreenPositioning
{
    Transform _myTransform, _target;
    float _moveTimer;

    Vector3 _pos;
    Quaternion _rot;
    public ScreenPositioning(Transform myTransform, Transform target)
    {
        _myTransform = myTransform;
        _target = target;
    }

    public void UpdateLoop(ref bool inPosition)
    {
        _moveTimer += Time.deltaTime;
        _pos = Vector3.Lerp(_myTransform.position, _target.position, _moveTimer);
        _rot = Quaternion.Lerp(_myTransform.rotation, _target.rotation, _moveTimer);
        _myTransform.SetPositionAndRotation(_pos, _rot);

        inPosition = _moveTimer > 1f;
    }
}


public class TimeManager
{
    TextMeshProUGUI _display;
    float _timer, _seconds, _minutes, _minutesToDiplay, _hours;
    string _secDisplay, _minDisplay, _hourDisplay, _final;
    public int ExtraTime
    {
        get => _extraTime;
        set
        {
            _extraTime = value;
            PlayerPrefs.SetInt(Utilities.TimeExtra, _extraTime);
        }
    }
    int _extraTime;
    int FinalTimeForLeaderboard() => Mathf.FloorToInt(_timer) + ExtraTime;

    public TimeManager(TextMeshProUGUI display)
    {
        _display = display;
        ExtraTime = 0;

        if (GameManager.Instance.usePlayerPrefs) _timer = PlayerPrefs.GetFloat(Utilities.TimeElapsed);
        if (GameManager.Instance.usePlayerPrefs) ExtraTime = PlayerPrefs.GetInt(Utilities.TimeExtra);
    }
    public void UpdateLoop()
    {
        _timer += Time.deltaTime;
        _seconds = (_timer + ExtraTime) % 60;
        _minutes = (_timer + ExtraTime) / 60;
        _minutesToDiplay = _minutes % 60;
        _hours = _minutes / 60;

        _seconds = Mathf.FloorToInt(_seconds);
        _minutesToDiplay = Mathf.FloorToInt(_minutesToDiplay);
        _hours = Mathf.FloorToInt(_hours);

        _secDisplay = ZeroBefore(_seconds) + _seconds;
        _minDisplay = ZeroBefore(_minutesToDiplay) + _minutesToDiplay;
        _hourDisplay = ZeroBefore(_hours) + _hours;

        _final = string.Empty;

        if(_minutes > 60)
        {
            _final += _hourDisplay + ":";
            _final += _minDisplay + ":";
        }
        else
        {
            _final += _minDisplay + ":";
        }
        _final += _secDisplay;
        _display.text = _final;

        string ZeroBefore(float time) => time <= 9 ? "0" : "";

        PlayerPrefs.SetFloat(Utilities.TimeElapsed, _timer);
    }
}
public class BlendShapeAnimation
{
    protected SkinnedMeshRenderer _skinnedMeshRenderer;
    protected int _blendOrdinal;

    public BlendShapeAnimation(SkinnedMeshRenderer skinnedMeshRenderer, int blendOrdinal)
    {
        _skinnedMeshRenderer = skinnedMeshRenderer;
        _blendOrdinal = blendOrdinal;
    }
    public virtual void UpdateLoop()
    {

    }
}
public class BlinkingBlendShape : BlendShapeAnimation
{
    float _timer, _blinkValue;

    public BlinkingBlendShape(SkinnedMeshRenderer skinnedMeshRenderer, int blendOrdinal) : base(skinnedMeshRenderer, blendOrdinal)
    {
        _skinnedMeshRenderer = skinnedMeshRenderer;
        _blendOrdinal = blendOrdinal;
        _timer = Random.Range(0, 0.5f);
    }

    public override void UpdateLoop()
    {
        _timer += Time.deltaTime;
        if (_timer > 5f)
        {
            _timer = 0f;
            DOTween.To(() => _blinkValue, x => _blinkValue = x, 100f, 0.05f)
                .SetLoops(2, LoopType.Yoyo);
        }
        _skinnedMeshRenderer.SetBlendShapeWeight(_blendOrdinal, _blinkValue);

    }
}
public class TalkingBlendShape : BlendShapeAnimation
{
    float _blendShapeTalk;
    public TalkingBlendShape(SkinnedMeshRenderer skinnedMeshRenderer, int blendOrdinal) : base(skinnedMeshRenderer, blendOrdinal)
    {
        _skinnedMeshRenderer = skinnedMeshRenderer;
        _blendOrdinal = blendOrdinal;
    }
    public override void UpdateLoop()
    {
        _blendShapeTalk = Mathf.PingPong(300f * Time.time, 50f) + 20;
        _skinnedMeshRenderer.SetBlendShapeWeight(_blendOrdinal, _blendShapeTalk);
    }
    public void StopTalk()
    {
        _skinnedMeshRenderer.SetBlendShapeWeight(_blendOrdinal, 0);
    }
}
#endregion

