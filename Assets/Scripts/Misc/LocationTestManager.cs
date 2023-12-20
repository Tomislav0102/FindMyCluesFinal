using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using ARLocation;
using UnityEngine.UI;
using TMPro;

public class LocationTestManager : MonoBehaviour
{
    public TextMeshProUGUI display;
    public Transform pointer;
  //  public LocationData[] locations;
    public Transform parObjects;
    GameObject[] _objectsToPlace;
    int _counterLocations;


    private void Awake()
    {
        _objectsToPlace = Utilities.GetallChildrenGameobjects(parObjects);
    }
    private void Start()
    {
        //for (int i = 0; i < locations.Length; i++)
        //{
        //    PlaceAtLocation.AddPlaceAtComponent(_objectsToPlace[i], locations[i].Location, Utilities.Opt());
        //}
    }

    private void Update()
    {
        Vector3 tar = _objectsToPlace[_counterLocations].transform.position;
        pointer.LookAt(tar);
        float dist = Vector3.Distance(pointer.position, tar);
       // display.text = $"Location - {locations[_counterLocations].Location.Label}\nDistance - {dist}";
    }

    public void BtnChangeTarget()
    {
        _counterLocations = (1 + _counterLocations) % _objectsToPlace.Length;
    }

}
