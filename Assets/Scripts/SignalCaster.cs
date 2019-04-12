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

        // Start is called before the first frame update
        void Start()
        {
            _currentProjectile = Instantiate(Projectile, transform.position+Vector3.right, Quaternion.identity).GetComponent<Projectile>();
            //_currentProjectile = _currentProjectile.ChangeType<ExplodingProjectile>();
            //(_currentProjectile as ExplodingProjectile)?.AddExplosionEffect(ExplosionEffect);

            StartCoroutine(Cast(60, 1));
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        IEnumerator Cast(float freq, int waitTime)
        {
            yield return new WaitForSecondsRealtime(waitTime);

            _currentProjectile = Instantiate(Projectile, transform.position + Vector3.right, Quaternion.identity).GetComponent<Projectile>();
            _currentProjectile = _currentProjectile.ChangeType<ExplodingProjectile>();
            yield return new WaitForSecondsRealtime(0.01f);
            _currentProjectile.SetEquation(new SinusEquation(freq));
            (_currentProjectile as ExplodingProjectile)?.AddExplosionEffect(ExplosionEffect);
        }
    }
}
