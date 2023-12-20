using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARLocation;

public class ExperienceManager : MonoBehaviour, IActivation
{
    [SerializeField] GameManager gm;
    [SerializeField] RectTransform pointer;
    GameObject _pointerGO;
    [SerializeField] LocationData[] locations;

    [SerializeField] Transform parLocations;
    Transform[] _locationTransforms;
    [HideInInspector] public LocationMileStone[] locationMileStones;

    public static Vector2 MinMaxDistance = new Vector2(4f, 10f);
    readonly Vector2 _defaultDistance = new Vector2(4f, 10f);

    bool _inExperince;
    public bool IsTracking;
    public bool IsActive { get; set; }

    [Header("Start location")]
    public LocationMileStone currentLocation;
    public LocationMileStone NextLocation()
    {
        LocationMileStone next = null;
        for (int i = 0; i < locationMileStones.Length; i++)
        {
            if (currentLocation == locationMileStones[i] && i < locationMileStones.Length - 1)
            {
                next = locationMileStones[i + 1];
                break;
            }
        }

        return next;
    }


    #region MONO
    private void Awake()
    {
        _locationTransforms = Utilities.GetallChildren<Transform>(parLocations);
        locationMileStones = Utilities.GetallChildren<LocationMileStone>(parLocations);
        _inExperince = false;
        _pointerGO = pointer.gameObject;
    }
    IEnumerator Start()
    {
        if (gm.usePlayerPrefs) currentLocation = locationMileStones[PlayerPrefs.GetInt(Utilities.Ordinal)];
        currentLocation.TickMe();
        if(currentLocation.ordinal > 0)
        {
            gm.uImanager.ProgressUpdate(currentLocation.ordinal - 1);
            gm.characterTalking.CharacterStart();
        }

        if (gm.build)
        {
            for (int i = 0; i < locations.Length; i++)
            {
                if (_locationTransforms[i].TryGetComponent(out MeshRenderer mesh)) mesh.enabled = false;
                PlaceAtLocation.AddPlaceAtComponent(_locationTransforms[i].gameObject, locations[i].Location, Utilities.Opt());
            }
        }

        yield return new WaitForSeconds(gm.build ? 3f : 0.1f);
        IsActive = true;

    }
    void OnDisable()
    {
        ResetDistance();
    }
    private void Update()
    {
        if (!IsActive || currentLocation == null)  return;

        float remainDisLocation = Vector3.Distance(Utilities.HorPos(gm.camTr.position), Utilities.HorPos(currentLocation.myTransform.position));
        _pointerGO.SetActive(false);
        gm.uImanager.Proximity(false);

        if (!currentLocation.IsActive) return;

        if (remainDisLocation < MinMaxDistance.x) 
        {
            _inExperince = true;
            currentLocation.InCloseRange();
        }
        else if (remainDisLocation < MinMaxDistance.y)
        {
            currentLocation.InMidRange();
        }
        else 
        {
            gm.uImanager.Proximity(true, currentLocation.nameOfLocation, remainDisLocation);
            _pointerGO.SetActive(true);
            currentLocation.CancelExperince();

            if (_inExperince)
            {
                gm.characterTalking.CharacterInfoText($"You've ventured too far. Return to {currentLocation.nameOfLocation}.", Emotion.Sad);
                _inExperince = false;
            }
            else
            {
                Vector3 targetDir = gm.cam.WorldToScreenPoint(currentLocation.myTransform.position) - pointer.position;
                pointer.up = targetDir.normalized;
                Vector3 targetPosInCameraSpace = gm.camTr.InverseTransformPoint(currentLocation.myTransform.position);
                pointer.localScale = new Vector3(1, targetPosInCameraSpace.z >= 0f ? 1 : -1, 1);
            }
        }

    }
    #endregion

    public void NewLocation()
    {
        if (!gm.gameEnd) gm.characterTalking.CharacterInfoText($"{currentLocation.nameOfLocation} is completed, find the next AR experince.", Emotion.Smile);
        gm.uImanager.ProgressUpdate(currentLocation.ordinal);
        gm.UpdateBackEnd(currentLocation.ordinal);
        currentLocation = NextLocation();

        _inExperince = false;
        PlayerPrefs.SetInt(Utilities.Ordinal, currentLocation.ordinal);
        if (currentLocation.hasCustomRange) MinMaxDistance = currentLocation.customRange;
        else ResetDistance();
    }
    public void ResetDistance() => MinMaxDistance = _defaultDistance;

    public void TrackingStarted()
    {
        IsTracking = true;
    }
    public void TrackingLost()
    {
        IsTracking = false;
    }
    public void TrackingRestored()
    {
        IsTracking = true;
    }

}
