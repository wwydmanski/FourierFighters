using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = System.Object;

namespace Assets.Scripts.Projectiles
{
    public class Explosion
    {
        private readonly LineRenderer _lineDrawer;
        private readonly Transform _transform;

        public Explosion(LineRenderer renderer, Transform transform)
        {
            _lineDrawer = renderer;
            _transform = transform;
        }

        public void DrawParticles(GameObject explosionEffect, float energy, Color color)
        {
            ParticleSystem system = UnityEngine.Object.Instantiate(explosionEffect, _transform.position, Quaternion.identity)
                .GetComponent<ParticleSystem>();

            var main = system.main;
            main.startColor = color;
            main.maxParticles = (int)(main.maxParticles * energy * 0.01);
            main.startSpeed = energy * 0.1f;
        }

        public void Explode(float realRadius, float energy)
        {
            //DrawRadius(realRadius);

            Collider[] colliders = Physics.OverlapSphere(_transform.position, realRadius);
            ApplyForce(colliders, _transform.position, realRadius, energy);
        }

        private void DrawRadius(float radius)
        {
            float theta = 0f;
            float thetaScale = 0.01f;

            int size = (int)((1f / thetaScale) + 1f);
            _lineDrawer.widthMultiplier = 0.1f;

            _lineDrawer.positionCount = size;
            for (int i = 0; i < size; i++)
            {
                theta += (2.0f * Mathf.PI * thetaScale);
                float x = radius * Mathf.Cos(theta);
                float y = radius * Mathf.Sin(theta);

                Vector3 obstruction =
                    ObstructionHelper.GetLinecastCollision(_transform.position, new Vector3(_transform.position.x + x, _transform.position.y + y, 0));

                _lineDrawer.SetPosition(i, obstruction);
            }
        }

        private void ApplyForce(Collider[] colliders, Vector3 explosionPos, float realRadius, float energy)
        {
            foreach (Collider hit in colliders)
            {
                var rb = hit.GetComponent<Rigidbody>();
                if (rb != null && !ObstructionHelper.WallObstruction(_transform.position, hit.transform.position))
                {
                    var controller = hit.gameObject.GetComponent<PlayerController.PlayerController>();
                    if (controller != null)
                        controller.AddExplosionForce(energy * 20, explosionPos, realRadius);
                    else
                        rb.AddExplosionForce(energy * 10, explosionPos, realRadius);

                    Debug.DrawLine(_transform.position, hit.transform.position, Color.red, 1);
                }
            }
        }
    }

    internal class ObstructionHelper
    {
        public static Vector3 GetLinecastCollision(Vector3 pos1, Vector3 pos2)
        {
            if (Physics.Linecast(pos1, pos2, out var hit))
            {
                if (hit.transform.tag == "wall")
                {
                    Debug.DrawLine(pos1, hit.point,
                        Color.yellow);
                    return hit.point;
                }

                var hits = Physics.RaycastAll(pos1, hit.point - pos1, (pos2 - pos1).magnitude);
                foreach (var raycastHit in hits)
                {
                    if (raycastHit.transform.tag == "wall")
                        return raycastHit.point;
                }
            }

            Debug.DrawLine(pos1, pos2, Color.white);
            return pos2;
        }

        public static bool WallObstruction(Vector3 pos1, Vector3 pos2)
        {
            if (GetLinecastCollision(pos1, pos2) == pos2)
                return false;

            return true;
        }
    }
}
