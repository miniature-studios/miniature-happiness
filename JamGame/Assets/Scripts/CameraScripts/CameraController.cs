using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Vector2 moveSensitivity;

    [SerializeField] float scrollSensitivity;
    [SerializeField] float minDistance;
    [SerializeField] float maxDistance;
    public float distance;

    [SerializeField] Transform lookAt;

    [SerializeField] float topLimiter;
    [SerializeField] float bottomLimiter;
    public Vector2 pitchYaw;

    void Start()
    {
        distance = (minDistance + maxDistance) * 0.5f;
        SetDistance();

        Vector3 lookAtVector = transform.position - lookAt.position;
        Vector3 lookAtVectorProjXZ = lookAtVector;
        lookAtVectorProjXZ.y = 0;
        Vector3 lookAtVectorTangent = new Vector3(lookAtVectorProjXZ.z, 0.0f, -lookAtVectorProjXZ.x);

        var pitch = Vector3.SignedAngle(lookAtVector, lookAtVectorProjXZ, lookAtVectorTangent);
        var yaw = Vector3.SignedAngle(new Vector3(0, 0, 1), lookAtVectorProjXZ, Vector3.up);

        pitchYaw = new Vector2(pitch, yaw);

        transform.LookAt(lookAt);
    }

    bool movingAround = false;
    Vector2 prevMousePosition;

    void Update()
    {
        if (movingAround)
        {
            var mouse_delta = (Vector2)Input.mousePosition - prevMousePosition;
            var delta_move = mouse_delta * moveSensitivity;
            prevMousePosition = (Vector2)Input.mousePosition;

            pitchYaw += new Vector2(-delta_move.y, delta_move.x);
            pitchYaw.x = Mathf.Clamp(pitchYaw.x, bottomLimiter, topLimiter);
            SetPitchYaw();

            if (Input.GetMouseButtonUp(2))
                movingAround = false;

            return;
        }

        var scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.001)
        {
            distance -= scrollSensitivity * scroll;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
            SetDistance();
            return;
        }

        if (Input.GetMouseButtonDown(2))
        {
            movingAround = true;
            prevMousePosition = Input.mousePosition;
        }

    }

    void SetDistance()
    {
        var dist = (transform.position - lookAt.position).magnitude;
        transform.position = lookAt.position + (transform.position - lookAt.position) / dist * distance;
    }

    void SetPitchYaw()
    {
        Vector3 offset = new Vector3(0, 0, 1) * distance;
        offset = Quaternion.AngleAxis(pitchYaw.x, Vector3.left) * offset;
        offset = Quaternion.AngleAxis(pitchYaw.y, Vector3.up) * offset;
        transform.position = lookAt.position + offset;
        transform.LookAt(lookAt.position);
    }
}
