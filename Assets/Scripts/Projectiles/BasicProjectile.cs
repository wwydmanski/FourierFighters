using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Equations;
using Assets.Scripts.Projectiles;
using UnityEngine;

class BasicProjectile : Projectile
{

    private float _energyCoeff = 0.1f;

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider.name);
        Destroy(gameObject, 5);
        Destroy(GetComponent<BoxCollider>());
        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<ParticleSystem>());
        Alive = false;

        collision.rigidbody.AddForce(new Vector3(1,1)*this.Equation.GetEnergy()*_energyCoeff, ForceMode.Impulse);
    }
}
