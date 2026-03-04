using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    float xRotation = 0f;

    void Start()
    {
        // ล็อกเมาส์ไว้กลางจอและซ่อนเคอร์เซอร์
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // รับค่าเมาส์
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // ส่วนที่ทำให้ก้ม-เงย (ก้มเงยคือการหมุนแกน X ของกล้อง)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // บรรทัดนี้สำคัญที่สุด! ต้องใส่ที่ตัวกล้อง
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // ส่วนที่ทำให้หมุนซ้าย-ขวา (หมุนตัวผู้เล่น)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}