using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Projectiles;
using UnityEngine;

namespace Assets.Scripts
{
    public class TailColliderHandler : MonoBehaviour
    {
        private GameObject _parent;

        public void Assign(GameObject parent)
        {
            _parent = parent;
        }

        // Update is called once per frame
        void OnTriggerEnter(Collider other)
        {
            var parentProjectile = _parent.GetComponent<Projectile>();
            Guid parentUuid = parentProjectile.Uuid;

            if (other.tag == "projectile")
            {
                var projectile = other.GetComponent<Projectile>();
                if (projectile.Uuid != parentUuid && projectile.Alive && parentProjectile.Alive)
                {
                    parentProjectile.Die();
                    projectile.Die();
                }
            }
        }
    }

    public class TrailCollider : MonoBehaviour
    {
        private TrailRenderer _trail;
        private GameObject _cube;

        // Start is called before the first frame update
        void Start()
        {
            this._trail = GetComponent<TrailRenderer>();
            _cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _cube.GetComponent<BoxCollider>().isTrigger = true;
            _cube.GetComponent<MeshRenderer>().enabled = false;

            TailColliderHandler handler = _cube.AddComponent<TailColliderHandler>();
            handler.Assign(transform.parent.gameObject);

            _cube.transform.position = gameObject.transform.position;
            _cube.transform.localScale = new Vector3(1,1,1);
        }

        void Update()
        {
            var positions = new Vector3[500];
            var verticesCount = _trail.GetPositions(positions);

            if (verticesCount > 2)
            {
                positions = new Vector3[verticesCount];
                _trail.GetPositions(positions);
                var size = CalculateSizeAndPosition(positions, out var transformPosition);
                _cube.transform.localScale = size;
                _cube.transform.position = transformPosition;
            }
        }

        private Vector3 CalculateSizeAndPosition(IList<Vector3> positions, out Vector3 transformPosition)
        {
            var maxX = positions.Max(vertex => vertex.x);
            var minX = positions.Min(vertex => vertex.x);
            var maxY = positions.Max(vertex => vertex.y);
            var minY = positions.Min(vertex => vertex.y);

            var size = _cube.transform.lossyScale;
            size.x = (maxX - minX);
            size.y = (maxY - minY);
            size.z = 0.5f;

            transformPosition = gameObject.transform.position;
            transformPosition.x += (size.x / 2) * Math.Sign(positions[0].x - positions[1].x);
            transformPosition.y = (maxY + minY) / 2;
            return size;
        }

        public void Destroy()
        {
            Destroy(_cube);
            Destroy(this);
            Debug.Log("Executed");
        }
    }
}
