using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Vector2 moveSensitivity;

    [SerializeField]
    private float scrollSensitivity;

    [SerializeField]
    private float minDistance;

    [SerializeField]
    private float maxDistance;
    public float Distance;

    [SerializeField]
    private Transform lookAt;

    [SerializeField]
    private float topLimiter;

    [SerializeField]
    private float bottomLimiter;
    public Vector2 PitchYaw;

    private void Start()
    {
        Distance = (minDistance + maxDistance) * 0.5f;
        SetDistance();

        Vector3 lookAtVector = transform.position - lookAt.position;
        Vector3 lookAtVectorProjXZ = lookAtVector;
        lookAtVectorProjXZ.y = 0;
        Vector3 lookAtVectorTangent = new(lookAtVectorProjXZ.z, 0.0f, -lookAtVectorProjXZ.x);

        float pitch = Vector3.SignedAngle(lookAtVector, lookAtVectorProjXZ, lookAtVectorTangent);
        float yaw = Vector3.SignedAngle(new Vector3(0, 0, 1), lookAtVectorProjXZ, Vector3.up);

        PitchYaw = new Vector2(pitch, yaw);

        transform.LookAt(lookAt);
    }

    private bool movingAround = false;
    private Vector2 prevMousePosition;

    private void Update()
    {
        if (movingAround)
        {
            Vector2 mouse_delta = (Vector2)Input.mousePosition - prevMousePosition;
            Vector2 delta_move = mouse_delta * moveSensitivity;
            prevMousePosition = (Vector2)Input.mousePosition;

            PitchYaw += new Vector2(-delta_move.y, delta_move.x);
            PitchYaw.x = Mathf.Clamp(PitchYaw.x, bottomLimiter, topLimiter);
            SetPitchYaw();

            if (Input.GetMouseButtonUp(2))
            {
                movingAround = false;
            }

            return;
        }

        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.001)
        {
            Distance -= scrollSensitivity * scroll;
            Distance = Mathf.Clamp(Distance, minDistance, maxDistance);
            SetDistance();
            return;
        }

        if (Input.GetMouseButtonDown(2))
        {
            movingAround = true;
            prevMousePosition = Input.mousePosition;
        }
    }

    private void SetDistance()
    {
        float dist = (transform.position - lookAt.position).magnitude;
        transform.position =
            lookAt.position + ((transform.position - lookAt.position) / dist * Distance);
    }

    private void SetPitchYaw()
    {
        Vector3 offset = new Vector3(0, 0, 1) * Distance;
        offset = Quaternion.AngleAxis(PitchYaw.x, Vector3.left) * offset;
        offset = Quaternion.AngleAxis(PitchYaw.y, Vector3.up) * offset;
        transform.position = lookAt.position + offset;
        transform.LookAt(lookAt.position);
    }
}
