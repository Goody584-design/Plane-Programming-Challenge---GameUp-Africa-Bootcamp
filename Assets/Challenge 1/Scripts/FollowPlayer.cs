using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [Tooltip("Drag your plane (root transform) here")]
    public Transform player;

    [Tooltip("Local offset relative to player. X = right, Y = up, Z = forward. Use negative Z to sit behind.")]
    public Vector3 localOffset = new Vector3(3f, 1.5f, -2f);

    [Tooltip("Smoothing time for position (smaller = snappier)")]
    public float smoothTime = 0.12f;

    [Tooltip("How fast camera rotates to look at the player")]
    public float lookSpeed = 8f;

    private Vector3 velocityRef = Vector3.zero;

    void LateUpdate()
    {
        if (player == null) return;

        // Convert local offset to world space relative to player's orientation
        Vector3 worldOffset = player.right * localOffset.x + player.up * localOffset.y + player.forward * localOffset.z;
        Vector3 targetPos = player.position + worldOffset;

        // Smooth follow
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocityRef, smoothTime);

        // Smooth look at player
        Vector3 dir = (player.position - transform.position);
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * lookSpeed);
        }
    }
}

