using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class BasicProjectile : MonoBehaviour
{
    private Transform _transform;
    private int _frame = 0;
    private Vector3 _origin;
    private float amplitude = 1.0f;
    private float freq = 20;
    private float speed = 0.2f;

    private float freqReduction = 80;

    void Start()
    {
        _transform = GetComponent<Transform>();
        _origin = transform.position;
    }

    void Update()
    {
        float x = (float) (_frame*speed);
        float y = (float) (amplitude*Math.Sin(x * freq * Math.PI/ freqReduction));

        _transform.position = _origin + new Vector3(x, y);
        _transform.rotation =
            Quaternion.AngleAxis((float) Math.Cos(x * freq * Math.PI / freqReduction)*45, Vector3.forward);
    }

    void FixedUpdate()
    {
        _frame++;
    }
}
