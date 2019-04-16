using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TailCollider : MonoBehaviour
{
    private TrailRenderer _trail;
    private GameObject _cube;

    // Start is called before the first frame update
    void Start()
    {
        this._trail = GetComponent<TrailRenderer>();
        _cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _cube.GetComponent<BoxCollider>().isTrigger = true;
        _cube.transform.position = gameObject.transform.position;
        _cube.transform.localScale = new Vector3(1,1,1);
    }

    void Update()
    {
        Vector3[] positions = new Vector3[500];
        int vertnum = _trail.GetPositions(positions);
        positions = new Vector3[vertnum];
        _trail.GetPositions(positions);

        var maxX = positions.Max(vertex => vertex.x);
        float minX = positions.Min(vertex => vertex.x);
        float maxY = positions.Max(vertex => vertex.y);
        float minY = positions.Min(vertex => vertex.y);

        Vector3 size = _cube.transform.lossyScale;
        size.x = (maxX - minX)*0.9f;
        size.y = (maxY - minY);
        size.z = 0.5f;

        _cube.transform.localScale = size;
        var transformPosition = gameObject.transform.position;
        transformPosition.x += (size.x / 2)*Math.Sign(positions[0].x - positions[1].x);
        transformPosition.y = (maxY + minY) / 2;
        _cube.transform.position = transformPosition;
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered");
    }
}
