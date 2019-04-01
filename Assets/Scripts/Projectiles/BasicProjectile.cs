using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Equations;
using UnityEngine;

class BasicProjectile : MonoBehaviour
{
    public IEquation Equation;

    private Transform _transform;
    private int _frame;
    private Vector3 _origin;
    private bool _alive = true;
    private float _energyCoeff = 0.1f;


    void Start()
    {
        _transform = GetComponent<Transform>();
        _origin = transform.position;

        Equation = new SinusEquation();
    }

    void Update()
    {
        if (_alive) 
        {
            _transform.position = _origin + Equation.GetPosition(_frame);
            _transform.rotation =
                Quaternion.AngleAxis(Equation.GetDerivative(_frame) * 45, Vector3.forward);
        }
    }

    void FixedUpdate()
    {
        _frame++;
    }

    public void SetEquation(IEquation eq)
    {
        this.Equation = eq;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider.name);
        Destroy(gameObject, 5);
        Destroy(GetComponent<BoxCollider>());
        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<ParticleSystem>());
        _alive = false;

        collision.rigidbody.AddForce(new Vector3(1,1)*this.Equation.GetEnergy()*_energyCoeff, ForceMode.Impulse);
    }
}
