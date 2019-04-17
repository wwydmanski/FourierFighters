using System;
using System.Collections;
using Assets.Scripts.Equations;
using Assets.Scripts.Projectiles;
using UnityEngine;

namespace Assets.Scripts
{
    public class SignalCaster : MonoBehaviour
    {
        public GameObject Projectile;
        public GameObject ExplosionEffect;
        private Projectile _currentProjectile;

        private Transform _transform;

        private Vector3 _direction;

        // Start is called before the first frame update
        void Start()
        {
            _transform = GetComponent<Transform>();
            StartCoroutine(Cast(30, 0));

            StartCoroutine(CastExploding(60, 2));
        }

        // Update is called once per frame
        void Update()
        {
            //Vector3 polar = new Vector3((float) Math.Cos(transform.rotation.z), (float) Math.Sin(transform.rotation.z));
            _direction = Vector3.right * (float) (Math.Cos(transform.rotation.y*Math.PI));
        }

        IEnumerator Cast(float freq, int waitTime)
        {
            yield return new WaitForSecondsRealtime(waitTime);
            Debug.Log(_direction);

            _currentProjectile = Instantiate(Projectile, transform.position + _direction, Quaternion.identity).GetComponent<Projectile>();
            yield return new WaitForSecondsRealtime(0.01f);
            _currentProjectile.SetEquation(new SinusEquation(freq));
            _currentProjectile.SetDirection(_direction);
        }

        IEnumerator CastExploding(float freq, int waitTime)
        {
            yield return new WaitForSecondsRealtime(waitTime);

            _currentProjectile = Instantiate(Projectile, transform.position + _direction, Quaternion.identity).GetComponent<Projectile>();
            _currentProjectile = _currentProjectile.ChangeType<ExplodingProjectile>();
            yield return new WaitForSecondsRealtime(0.01f);
            _currentProjectile.SetEquation(new SinusEquation(freq));
            _currentProjectile.SetDirection(_direction);

            (_currentProjectile as ExplodingProjectile)?.AddExplosionEffect(ExplosionEffect);
        }
    }
}
