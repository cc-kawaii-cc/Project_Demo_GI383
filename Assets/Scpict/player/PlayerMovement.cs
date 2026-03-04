using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float gravity = -19.62f; // แรงโน้มถ่วง (ปรับตามความเหมาะสม)
    public float jumpHeight = 2f;

    Vector3 velocity;
    bool isGrounded;

    // ตรวจสอบพื้น
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    void Update()
    {
        // เช็คว่าเท้าแตะพื้นหรือไม่
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // ให้ตัวติดพื้นไว้เล็กน้อย
        }

        // รับค่า Input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // ตรวจสอบว่ากดปุ่มวิ่ง (Shift) หรือไม่
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // คำนวณทิศทางตามตัวละคร
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // การกระโดด
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // คำนวณแรงโน้มถ่วง
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}