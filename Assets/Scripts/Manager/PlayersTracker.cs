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
    public float m_DampTime = 0.2f;                 // Approximate time for the camera to refocus.
    public float m_ScreenEdgeBuffer = 4f;           // Space between the top/bottom most target and the screen edge.
    public float m_MinSize = 6.5f;                  // The smallest orthographic size the camera can be.
    public float m_MaxSize = 15;                  // The smallest orthographic size the camera can be.
    public float y_bottom_limit = 0;

    private Transform[] m_Targets;                  // All the targets the camera needs to encompass.

    private Camera m_Camera;                        // Used for referencing the camera.
    private float m_ZoomSpeed;                      // Reference speed for the smooth damping of the orthographic size.
    private Vector3 m_MoveVelocity;                 // Reference velocity for the smooth damping of the position.
    private Vector3 m_DesiredPosition;              // The position the camera is moving towards.
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
        m_Camera = GetComponent<Camera>();
    }

    private IEnumerator InitializeFields()
    {
        yield return new WaitForSecondsRealtime(0.01f);
        List<GameObject> players = new List<GameObject>(LevelManager.GetComponent<GameManager>().Players.ToList().Select(x => x.instance));
        m_Targets = new Transform[players.Count];

        for (int i = 0; i < players.Count; i++)
            m_Targets[i] = players.ElementAt(i).transform;
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
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }

    private void FindBounds()
    {
        m_DesiredPosition.y = Mathf.Max(m_DesiredPosition.y, y_bottom_limit);
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        // Go through all the targets and add their positions together.
        for (int i = 0; i < m_Targets.Length; i++)
        {
            // If the target isn't active, go on to the next one.
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            // Add to the average and increment the number of targets in the average.
            averagePos += m_Targets[i].position;
            numTargets++;
        }

        // If there are targets divide the sum of the positions by the number of them to find the average.
        if (numTargets > 0)
            averagePos /= numTargets;

        // Keep the same y value.
        averagePos.z = transform.position.z;

        // The desired position is the average position;
        m_DesiredPosition = averagePos;
        m_DesiredPosition.y += 1;
    }


    private void FindRequiredSize()
    {
        // Find the position the camera rig is moving towards in its local space.
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

        // Start the camera's size calculation at zero.
        float size = 0f;

        // Go through all the targets...
        for (int i = 0; i < m_Targets.Length; i++)
        {
            // ... and if they aren't active continue on to the next target.
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            // Otherwise, find the position of the target in the camera's local space.
            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);

            // Find the position of the target from the desired position of the camera's local space.
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            // Choose the largest out of the current size and the distance of the tank 'up' or 'down' from the camera.
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));

            // Choose the largest out of the current size and the calculated size based on the tank being to the left or right of the camera.
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) * m_Camera.aspect);
        }

        // Add the edge buffer to the size.
        size += m_ScreenEdgeBuffer;

        // Make sure the camera's size isn't below the minimum.
        size = Mathf.Clamp(size, m_MinSize, m_MaxSize) * m_Camera.aspect;

        m_DesiredPosition.z = m_Targets[0].position.z - size * 0.5f / Mathf.Tan(m_Camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
    }


    public void SetStartPositionAndSize()
    {
        // Find the desired position.
        FindAveragePosition();

        // Set the camera's position to the desired position without damping.
        transform.position = m_DesiredPosition;

        // Find and set the required size of the camera.
        //m_Camera.orthographicSize = FindRequiredSize();
    }
}

