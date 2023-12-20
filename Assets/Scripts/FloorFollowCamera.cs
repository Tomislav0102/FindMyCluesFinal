using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorFollowCamera : MonoBehaviour
{
    Transform camTr;

    private void Start()
    {
        camTr = GameManager.Instance.camTr;
    }
    private void Update()
    {
        transform.position = new Vector3(camTr.position.x, 0f, camTr.position.z);
    }
}
