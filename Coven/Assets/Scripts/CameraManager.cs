using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    PlayerManager player;
    Camera mainCam;

    [Header("Camera Bounds")]
    public Transform minBound;
    public Transform maxBound;
    public float horizMin;
    public float horizMax;
    public float vertMin;
    public float vertMax;
    public float zOffset;

    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        player = FindObjectOfType<PlayerManager>();
        mainCam = GetComponent<Camera>();

        if(player != null)
        {
            this.transform.position = player.transform.position + new Vector3(0f, 0f, zOffset);
        }
    }

    public void FollowTarget(float timedelta)
    {
        if (player != null)
        {
            Vector3 playerPosition = new Vector3(Mathf.Clamp(player.transform.position.x, minBound.position.x, maxBound.position.x), Mathf.Clamp(player.transform.position.y, minBound.position.y, maxBound.position.y), 0f);

            Vector3 delta = playerPosition - mainCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, zOffset));
            Vector3 destination = transform.position + delta;
            destination.z = zOffset;
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, timedelta);
        }
    }
}
