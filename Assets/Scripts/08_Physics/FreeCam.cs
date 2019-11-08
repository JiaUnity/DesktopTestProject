using UnityEngine;
using System.Collections;

public class FreeCam : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float turnSpeed = 1f;
    public bool invertVertical = true;

    private Vector3 turnDirection;
    private Vector3 moveDirection;

    private float horizontalMove = 0f;
    private float forwardsMove = 0f;
    private float verticalMove = 0f;
    private float horizontalTurn = 0f;
    private float verticalTurn = 0f;

    void Start()
    {
#if UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Locked;
#endif
    }

    void Update()
    {
        MovementControls();
        TurningControls();
    }

    void MovementControls()
    {
        // Cross-platform Movement Controls
        horizontalMove = Input.GetAxis("Horizontal");
        forwardsMove = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.Space))
            verticalMove = 1f;
        else if (Input.GetKey(KeyCode.LeftControl))
            verticalMove = -1f;
        else
            verticalMove = 0f;

        moveDirection = new Vector3(horizontalMove, verticalMove, forwardsMove);
    }

    void TurningControls()
    {
        horizontalTurn = Input.GetAxis("Mouse X");
        verticalTurn = Input.GetAxis("Mouse Y");

        if (invertVertical)
            verticalTurn = -verticalTurn;

        Mathf.Clamp(verticalTurn, 80, 120f);

        turnDirection = new Vector3(verticalTurn, horizontalTurn, 0f);

        // Stop the camera from tilting
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0f));
    }

    void FixedUpdate()
    {
        transform.Translate(moveDirection * (Time.deltaTime * moveSpeed));
        transform.Rotate(turnDirection * (Time.deltaTime * turnSpeed));
    }
}
