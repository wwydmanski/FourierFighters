﻿using System;
using System.Collections;
using Assets.Scripts.Equations;
using Assets.Scripts.Projectiles;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts
{
    public class SignalCaster : MonoBehaviour
    {
        public GameObject Projectile;
        public GameObject ExplosionEffect;

        public bool Rotating;
        private Projectile _currentProjectile;
        private Transform _transform;
        private Vector3 _direction;
        private float _lastRotation;
        private float _offsetMult = 1;
        private float _y_offset = 0.3f;
        private Random _random = new Random();

        // ReSharper disable once UnusedMember.Local
        private void Start()
        {
            _transform = GetComponent<Transform>();
            _direction = Vector3.right;
            _direction.x = (float) (Math.Cos(_transform.rotation.y * Math.PI));

            //CastRight(30, true, 0);
            _lastRotation = Time.time;
            _offsetMult = GetComponent<Collider>().bounds.size.magnitude;
        }

        // ReSharper disable once UnusedMember.Local
        private void Update()
        {
            _offsetMult = GetComponent<Collider>().bounds.size.magnitude;
            if (Rotating)
            {
                if (Time.time - _lastRotation > 1)
                {
                    _lastRotation = Time.time;
                    _direction = Quaternion.Euler(0, 0, 90) * _direction;
                    Debug.DrawRay(_transform.position, _direction,
                        Color.yellow, 0.5f);

                    StartCoroutine(Cast(_random.Next(1, 3)*20, 0, _direction, _random.Next(2)==1));
                }
            }
        }

        public void CastRight(float freq, bool exploding=false, float waitTime=0)
        {
            StartCoroutine(Cast(freq, waitTime, Vector3.right, exploding));
        }

        public void CastLeft(float freq, bool exploding = false, float waitTime = 0)
        {
            StartCoroutine(Cast(freq, waitTime, Vector3.left, exploding));
        }

        public void CastUp(float freq, bool exploding = false, float waitTime = 0)
        {
            StartCoroutine(Cast(freq, waitTime, Vector3.up, exploding));
        }

        public void CastDown(float freq, bool exploding = false, float waitTime = 0)
        {
            StartCoroutine(Cast(freq, waitTime, Vector3.down, exploding));
        }

        public void CastAngle(float freq, Vector3 direction, bool exploding = false, float waitTime = 0)
        {
            StartCoroutine(Cast(freq, waitTime, direction, exploding));
        }

        private IEnumerator Cast(float freq, float waitTime, Vector3 direction, bool exploding)
        {
            yield return new WaitForSecondsRealtime(waitTime);

            var offset = direction*_offsetMult;
            offset.y += _y_offset;
            _currentProjectile = Instantiate(Projectile, transform.position + offset, Quaternion.identity).GetComponent<Projectile>();

            if (exploding)
                _currentProjectile = _currentProjectile.ChangeType<ExplodingProjectile>();
            else
                _currentProjectile.GetComponent<ParticleSystem>().Stop();

            yield return new WaitForSecondsRealtime(0.01f);
            _currentProjectile.SetEquation(new SinusEquation(freq));
            _currentProjectile.SetDirection(direction);

            if (exploding)
                (_currentProjectile as ExplodingProjectile)?.AddExplosionEffect(ExplosionEffect);
        }
    }
}
