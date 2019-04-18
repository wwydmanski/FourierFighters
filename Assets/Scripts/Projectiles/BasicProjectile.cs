using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts;
using Assets.Scripts.Equations;
using Assets.Scripts.Projectiles;
using UnityEngine;

class BasicProjectile : Projectile
{

    private float _energyCoeff = 0.1f;

    // ReSharper disable once UnusedMember.Local
    private void OnCollisionEnter(Collision collision)
    {
        if (Alive)
        {
            Debug.Log(collision.collider.name);
            var colliderProjectile = collision.collider.gameObject.GetComponent<Projectile>();

            collision.rigidbody?.AddForce(new Vector3(1, 1) * this.Equation.GetEnergy() * _energyCoeff,
                ForceMode.Impulse);
            Die();
        }
    }

    public override void Die()
    {
        if (Alive)
        {
            Destroy();
        }
    }
}
