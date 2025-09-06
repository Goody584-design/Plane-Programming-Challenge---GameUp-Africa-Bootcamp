using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement (tweak in Inspector)")]
    [Tooltip("Acceleration (units/s^2) applied along the plane's forward direction")]
    public float acceleration = 8f;
    [Tooltip("Maximum linear speed (units/s)")]
    public float maxSpeed = 12f;
    [Tooltip("If your model faces backward, set to -1 to invert forward direction")]
    public float forwardMultiplier = 1f;

    [Header("Pitch (tilt)")]
    [Tooltip("Maximum pitch angle in degrees (positive = nose down)")]
    public float maxPitchAngle = 20f;
    [Tooltip("Degrees per second when player presses key")]
    public float pitchSpeed = 120f;
    [Tooltip("Degrees per second when returning to level")]
    public float pitchReturnSpeed = 90f;

    private Rigidbody rb;
    private float targetPitch = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // Keep physics stable: freeze rotations so plane does not auto-tilt.
        // We'll drive pitch manually.
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

    }

    void Update()
    {
        // Only up and down arrow keys control tilt
        if (Input.GetKey(KeyCode.UpArrow))
        {
            // Up arrow => pitch up (nose up). In Unity X rotation: negative => nose up.
            targetPitch = -maxPitchAngle;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            // Down arrow => nose down
            targetPitch = maxPitchAngle;
        }
        else
        {
            targetPitch = 0f; // level out when not pressing keys
        }
    }

    void FixedUpdate()
    {
        // Only move when up or down arrow is pressed
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            // Always apply forward force
            Vector3 forwardForce = transform.forward * acceleration * forwardMultiplier;
            rb.AddForce(forwardForce, ForceMode.Acceleration);

            // Calculate pitch (nose up/down) for climb/descent
            float pitchRadians = transform.localEulerAngles.x;
            if (pitchRadians > 180f) pitchRadians -= 360f;
            pitchRadians = pitchRadians * Mathf.Deg2Rad;

            // If nose is up, add upward force to climb
            if (pitchRadians < 0f)
            {
                Vector3 upForce = transform.up * acceleration * Mathf.Abs(Mathf.Sin(pitchRadians));
                rb.AddForce(upForce, ForceMode.Acceleration);
            }
            // If nose is down, add downward force to descend
            else if (pitchRadians > 0f)
            {
                Vector3 downForce = -transform.up * acceleration * Mathf.Abs(Mathf.Sin(pitchRadians));
                rb.AddForce(downForce, ForceMode.Acceleration);
            }
        }

        // Slow down if too fast - clamp total velocity magnitude
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        // Apply pitch (visual tilt)
        ApplyPitch();
    }

    private void ApplyPitch()
    {
        Vector3 euler = transform.localEulerAngles;
        float curPitch = euler.x;
        if (curPitch > 180f) curPitch -= 360f; // map 0..360 -> -180..180

        // choose smoothing speed: return vs input
        float useSpeed = Mathf.Approximately(targetPitch, 0f) ? pitchReturnSpeed : pitchSpeed;
        float newPitch = Mathf.MoveTowards(curPitch, targetPitch, useSpeed * Time.fixedDeltaTime);

        transform.localEulerAngles = new Vector3(newPitch, euler.y, euler.z);
    }

    // Handy inspector button from other scripts if needed
    public void InvertForward()
    {
        forwardMultiplier *= -1f;
    }
}
