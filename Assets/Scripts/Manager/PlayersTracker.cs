using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Manager;
using UnityEngine;

public class PlayersTracker : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject LevelManager;
    public float MDampTime = 0.2f;                 // Approximate time for the camera to refocus.
    public float MScreenEdgeBuffer = 4f;           // Space between the top/bottom most target and the screen edge.
    public float MMinSize = 6.5f;                  // The smallest orthographic size the camera can be.
    public float MMaxSize = 15;                  // The smallest orthographic size the camera can be.
    public float YMinBound = 0;
    public float YMaxBound = 11;
    public float XMaxBound = 20;
    public float XMinBound = -20;

    private Transform[] _mTargets;                  // All the targets the camera needs to encompass.

    private Camera _mCamera;                        // Used for referencing the camera.
    private float _mZoomSpeed;                      // Reference speed for the smooth damping of the orthographic size.
    private Vector3 _mMoveVelocity;                 // Reference velocity for the smooth damping of the position.
    private Vector3 _mDesiredPosition;              // The position the camera is moving towards.
    private List<GameObject> _players = new List<GameObject>();
    private float _ratio;
    private float _playerZPosition;
    private float _startingDistance;

    void Start()
    {
        StartCoroutine(InitializeFields());
    }

    private void Awake()
    {
        _mCamera = GetComponent<Camera>();
    }

    private IEnumerator InitializeFields()
    {
        yield return new WaitForSecondsRealtime(0.01f);
        List<GameObject> players = new List<GameObject>(LevelManager.GetComponent<GameManager>().Players.ToList().Select(x => x.instance));
        _mTargets = new Transform[players.Count];

        for (int i = 0; i < players.Count; i++)
            _mTargets[i] = players.ElementAt(i).transform;
    }

    private void FixedUpdate()
    {
        Move();
    }


    private void Move()
    {
        // Find the average position of the targets.
        FindAveragePosition();
        FindRequiredSize();

        FindBounds();

        // Smoothly transition to that position.
        transform.position = Vector3.SmoothDamp(transform.position, _mDesiredPosition, ref _mMoveVelocity, MDampTime);
    }

    private void FindBounds()
    {
        _mDesiredPosition.y = Mathf.Clamp(_mDesiredPosition.y, YMinBound, YMaxBound);
        _mDesiredPosition.x = Mathf.Clamp(_mDesiredPosition.x, XMinBound, XMaxBound);
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        // Go through all the targets and add their positions together.
        for (int i = 0; i < _mTargets.Length; i++)
        {
            // If the target isn't active, go on to the next one.
            if (!_mTargets[i].gameObject.activeSelf)
                continue;

            // Add to the average and increment the number of targets in the average.
            averagePos += _mTargets[i].position;
            numTargets++;
        }

        // If there are targets divide the sum of the positions by the number of them to find the average.
        if (numTargets > 0)
            averagePos /= numTargets;

        // Keep the same y value.
        averagePos.z = transform.position.z;

        // The desired position is the average position;
        _mDesiredPosition = averagePos;
        _mDesiredPosition.y += 1;
    }


    private void FindRequiredSize()
    {
        // Find the position the camera rig is moving towards in its local space.
        Vector3 desiredLocalPos = transform.InverseTransformPoint(_mDesiredPosition);

        // Start the camera's size calculation at zero.
        float size = 0f;

        // Go through all the targets...
        for (int i = 0; i < _mTargets.Length; i++)
        {
            // ... and if they aren't active continue on to the next target.
            if (!_mTargets[i].gameObject.activeSelf)
                continue;

            // Otherwise, find the position of the target in the camera's local space.
            Vector3 targetLocalPos = transform.InverseTransformPoint(_mTargets[i].position);

            // Find the position of the target from the desired position of the camera's local space.
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            // Choose the largest out of the current size and the distance of the tank 'up' or 'down' from the camera.
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));

            // Choose the largest out of the current size and the calculated size based on the tank being to the left or right of the camera.
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) * _mCamera.aspect);
        }

        // Add the edge buffer to the size.
        size += MScreenEdgeBuffer;

        // Make sure the camera's size isn't below the minimum.
        size = Mathf.Clamp(size, MMinSize, MMaxSize) * _mCamera.aspect;

        _mDesiredPosition.z = _mTargets[0].position.z - size * 0.5f / Mathf.Tan(_mCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
    }


    public void SetStartPositionAndSize()
    {
        // Find the desired position.
        FindAveragePosition();

        // Set the camera's position to the desired position without damping.
        transform.position = _mDesiredPosition;

        // Find and set the required size of the camera.
        //m_Camera.orthographicSize = FindRequiredSize();
    }
}

