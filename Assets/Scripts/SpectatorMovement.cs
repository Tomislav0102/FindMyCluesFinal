using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorMovement : MonoBehaviour
{
    public float brzinaKretanja = 5f;
    public float brzinaRotacije = 8f;
    float hor, ver;
    Vector3 _moveDir;
    Vector2 mouseRot;
    float maxAngle;
    Rigidbody _rigid;
    bool _isActive = true;
    public bool fullRotations;
    private void Awake()
    {
        _rigid = GetComponent<Rigidbody>();
    }
    private void Start()
    {
       // Utilities.ShowCursor(false);
        maxAngle = 60 / brzinaRotacije;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) _isActive = !_isActive;
        if (!_isActive) return;
        hor = Input.GetAxis("Horizontal");
        ver = Input.GetAxis("Vertical");

        mouseRot.x += Input.GetAxis("Mouse X");
        mouseRot.y += Input.GetAxis("Mouse Y");
        mouseRot.y = Mathf.Clamp(mouseRot.y, -maxAngle, maxAngle);

        if (fullRotations) transform.eulerAngles = new Vector3(-mouseRot.y, mouseRot.x, 0f) * brzinaRotacije;
        else transform.eulerAngles = new Vector3(0f, mouseRot.x, 0f) * brzinaRotacije;
    }

    private void FixedUpdate()
    {
        Vector3 moveDir = (transform.forward * ver + transform.right * hor);
        moveDir.y = 0f;
        moveDir.Normalize();
        _rigid.velocity = moveDir * brzinaKretanja;
    }
}
