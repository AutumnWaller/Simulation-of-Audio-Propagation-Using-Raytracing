using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float m_xRotation = 0.0f;
    public float m_rotateSpeed = 100.0f;
    public float m_movementSpeed = 26.0f;
    public Transform m_playerTransform;
    public Rigidbody m_rb;

    private CharacterController m_charController;

    private void Awake() {
        m_charController = GetComponent<CharacterController>();
    }

    private void Update() {
        float hor = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");
        Vector2 mouse = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        m_xRotation -= mouse.y;
        mouse.y = Mathf.Clamp(mouse.y * Time.deltaTime, -90, 90);

        transform.localRotation = Quaternion.Euler(m_xRotation, 0, 0);
        m_playerTransform.Rotate(Vector3.up * (mouse.x * Time.deltaTime * m_rotateSpeed));

        m_charController.Move((((m_playerTransform.right * hor) + (m_playerTransform.forward * vert)) * Time.deltaTime * m_movementSpeed));

    }
}
