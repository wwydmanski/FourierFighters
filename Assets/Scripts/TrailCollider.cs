using System;
using System.Linq;
using Assets.Scripts.Projectiles;
using UnityEngine;

namespace Assets.Scripts
{
    public class TailColliderHandler : MonoBehaviour
    {
        private float _startTime;
        private Guid _parentUuid;

        public void Assign(Guid uuid)
        {
            _parentUuid = uuid;
        }

        void Start()
        {
            _startTime = Time.time;
        }

        // Update is called once per frame
        void OnTriggerEnter(Collider other)
        {
            if (!(Time.time - _startTime > 0.1)) return;

            if (other.tag == "projectile")
            {
                var projectile = other.GetComponent<Projectile>();
                if (projectile.Uuid != _parentUuid && projectile.Alive)
                {
                    Debug.Log("TailColliderHandler triggered");
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
            _cube.GetComponent<BoxCollider>().tag = "projectile";
            _cube.GetComponent<MeshRenderer>().enabled = false;

            TailColliderHandler handler = _cube.AddComponent<TailColliderHandler>();
            handler.Assign(GetComponentInParent<Projectile>().Uuid);

            _cube.transform.position = gameObject.transform.position;
            _cube.transform.localScale = new Vector3(1,1,1);
        }

        void Update()
        {
            var positions = new Vector3[500];
            var vertNum = _trail.GetPositions(positions);

            if (vertNum > 2)
            {
                positions = new Vector3[vertNum];
                _trail.GetPositions(positions);
                var maxX = positions.Max(vertex => vertex.x);
                float minX = positions.Min(vertex => vertex.x);
                float maxY = positions.Max(vertex => vertex.y);
                float minY = positions.Min(vertex => vertex.y);

                Vector3 size = _cube.transform.lossyScale;
                size.x = (maxX - minX);
                size.y = (maxY - minY);
                size.z = 0.5f;

                _cube.transform.localScale = size;
                var transformPosition = gameObject.transform.position;
                transformPosition.x += (size.x / 2) * Math.Sign(positions[0].x - positions[1].x);
                transformPosition.y = (maxY + minY) / 2;
                _cube.transform.position = transformPosition;
            }
        }

        public void Destroy()
        {
            Destroy(_cube);
            Destroy(this);
            Debug.Log("Executed");
        }
    }
}
