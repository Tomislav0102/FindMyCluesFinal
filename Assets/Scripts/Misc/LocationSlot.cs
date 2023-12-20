using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
//using ARLocation;

public class LocationSlot : MonoBehaviour, IPointerClickHandler
{
    GameManager _gm;
    [SerializeField] TextMeshProUGUI display;
   // [SerializeField] LocationData location;


    private void Awake()
    {
        _gm = GameManager.Instance;
        //if (location == null)
        //{
        //    gameObject.SetActive(false);
        //}
        //display.text = $"{transform.GetSiblingIndex()} - {location.Location.Label}";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       // _gm.placeAtLocation.Location = location.Location;
        transform.parent.gameObject.SetActive(false);
    }
}
