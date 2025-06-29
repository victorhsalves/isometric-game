using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float runningSpeed = 10f;
    [SerializeField] private float crouchingSpeed = 2f;
    private Vector3 _input;


    void Update()
    {
        GatherInput();
    }

    void FixedUpdate()
    {
        Move();
        Look();
    }

    void GatherInput()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    void Look()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // transform.LookAt(hit.point);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(hit.point - transform.position), rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
    }

    void Move()
    {
        // holding shift will run, holding control will crouch
        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl))
        {
            currentSpeed = runningSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = crouchingSpeed;
        }
        var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));  // Rotate 45 degrees around the Y axis
        var direction = matrix.MultiplyPoint3x4(_input);
        rb.MovePosition(transform.position + direction.normalized * currentSpeed * Time.deltaTime);
    }
}