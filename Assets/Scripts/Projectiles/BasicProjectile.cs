using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Equations;
using UnityEngine;

class BasicProjectile : MonoBehaviour
{
    public IEquation equation;

    private Transform _transform;
    private int _frame;
    private Vector3 _origin;


    void Start()
    {
        _transform = GetComponent<Transform>();
        _origin = transform.position;

        equation = new SinusEquation();
    }

    void Update()
    {
        Vector3 offset = 

        _transform.position = _origin + equation.GetPosition(_frame);
        _transform.rotation =
            Quaternion.AngleAxis(equation.GetDerivative(_frame)*45, Vector3.forward);
    }

    void FixedUpdate()
    {
        _frame++;
    }
}
