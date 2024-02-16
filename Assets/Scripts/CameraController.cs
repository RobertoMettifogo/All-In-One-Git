using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float smoothness = 0.1f;

    void LateUpdate()
    {
        if (player != null)
        {
            // Calculate the target position
            Vector3 targetPosition = new Vector3(player.position.x, player.position.y, -10f);

            // Smoothly interpolate towards the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothness);
        }
    }
}
