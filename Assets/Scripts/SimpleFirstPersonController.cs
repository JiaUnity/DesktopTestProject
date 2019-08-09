using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimpleFirstPersonController : MonoBehaviour
{
    private CharacterController character_controller;
    private Transform main_camera;
    public int walk_speed = 5;
    public int rotate_speed = 5;

    // Use this for initialization
    void Start()
    {
        character_controller = GetComponent<CharacterController>();
        main_camera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        RotateView();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void RotateView()
    {
        float x = Input.GetAxis("Mouse X") * rotate_speed;
        float y = Input.GetAxis("Mouse Y") * rotate_speed;

        Quaternion charTargetRot = transform.localRotation * Quaternion.Euler(0f, x, 0f);
        Quaternion camTargetRot = main_camera.localRotation * Quaternion.Euler(-y, 0f, 0f);

        transform.localRotation = charTargetRot;
        main_camera.localRotation = ClampRotationAroundXAxis(camTargetRot);
    }

    private void Move()
    {
        Vector3 desiredMove = transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal");
        desiredMove = desiredMove.normalized * walk_speed;

        character_controller.Move(desiredMove * Time.fixedDeltaTime);
    }

    private Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, -90, 90);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
}
