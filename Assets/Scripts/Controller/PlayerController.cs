using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerController
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class PlayerController : MonoBehaviour
    {
        private Vector3 _currentVelocity = Vector3.zero;
        private bool _faceRight = true;

        private bool _keyJumpPressed;
        private bool _keyJumpRetrigger;

        private bool _shot;
        public bool enabledController = false;
        public string deathTag;
        private bool _playerState =  true;
        
        private SignalCaster _signalCaster;

        private float _timeRealLastGroundCollision;
        private float _timeRealLastWallLeftCollision;
        private float _timeRealLastWallRightCollision;

        [HideInInspector] public int PlayerNumber;
        private Rigidbody Rb { get; set; }
        private string _movementAxisName;
        private string _fireButtonName;
        private string _jumpButtonName;

        public GameObject AntennaIcon;

        public Vector3 Velocity => Rb.velocity;

        public Vector3 VelocityRelativeGround => Velocity / PhysicsParams.OnGroundMaxVelHorizontal;

        public bool IsState()
        {
            return _playerState;
        }

        public void SetState(bool state)
        {
            _playerState = state;
        }

        public void FreezeAllPosition(bool freeze)
        {
            if(freeze)
                Rb.constraints = RigidbodyConstraints.FreezeAll;
            else
                Rb.constraints = RigidbodyConstraints.FreezePositionZ;
        }
        
        
        public bool IsOnGround => GetIsColliding(_timeRealLastGroundCollision);

        public bool IsOnWallLeft => GetIsColliding(_timeRealLastWallLeftCollision);

        public bool IsOnWallRight => GetIsColliding(_timeRealLastWallRightCollision);

        public bool IsInAir { get; private set; }

        private float EntityMass => PhysicsParams.PlayerMass;

        [field: SerializeField] public PhysicsParams PhysicsParams { get; set; }

        public Vector3 CurrentForce { get; private set; } = Vector3.zero;

        public bool IsOnWall { get; private set; }

        public List<Renderer> AllRenderers { get; private set; }

        private int _energyLeft=3;
        private float lastShotTime;

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        private bool GetIsColliding(float timeLastCollision)
        {
            return Time.realtimeSinceStartup < timeLastCollision + 0.05f;
        }

        public void Awake()
        {
            Rb = GetComponent<Rigidbody>();
            AllRenderers = new List<Renderer>(GetComponentsInChildren<Renderer>(true));
            _signalCaster = GetComponent<SignalCaster>();
        }

        private void Start()
        {
            _movementAxisName = "Horizontal" + PlayerNumber;
            _fireButtonName = "Fire" + PlayerNumber;
            _jumpButtonName = "Jump" + PlayerNumber;

            GetComponent<AntennaIconManager>().SetBars(AntennaIcon.GetComponentsInChildren<Image>());
            //InvokeRepeating("IncreaseEnergy", 0.5f, 0.5f);
            StartCoroutine(IncreaseEnergy());
        }

        // Update is called once per frame
        public void Update()
        {
            //let's reset forces to 0 and then add regular gravitation
            SimResetForce();
            SimAddForce(new Vector3(0, PhysicsParams.GameGravity2, 0) * EntityMass);
            
            //process key input (like jumping key pressed, etc...)
            if (enabledController)
            {
                ProcessInput();
            }

            //simulate position and velocity based on all acting forces
            ComputeVelocity(Time.deltaTime);

            //collision detection with static world
            IsOnWall = IsOnWallLeft || IsOnWallRight;
            IsInAir = IsOnGround == false;

            GetComponent<AntennaIconManager>().UpdateIcon(_energyLeft);
            transform.rotation = Quaternion.identity;
        }

        private IEnumerator IncreaseEnergy()
        {
            float lastUpdateTime = Time.time;
            for (;;)
            {
                yield return new WaitForSeconds(0.1f);

                var time = Time.time;
                if (time - lastUpdateTime >= PhysicsParams.EnergyRechargeRate && time - lastShotTime >= PhysicsParams.EnergyRechargeShotDelay)
                {
                    if(_energyLeft==0)
                        if(time-lastShotTime < PhysicsParams.FullDischargeDelay)
                            continue;

                    _energyLeft = Math.Min(_energyLeft + 1, 5);
                    lastUpdateTime = time;
                }
            }
        }

        private void SimResetForce()
        {
            CurrentForce = Vector3.zero;
        }

        public void ProjectileHit(Vector3 force)
        {
            CurrentForce += force*PhysicsParams.ProjectileForce;
            ComputeVelocity(Time.deltaTime);
        }

        public void AddExplosionForce(float energy, Vector3 explosionPos, float radius)
        {
            Vector3 dir = Rb.position - explosionPos;
            Vector3 force = dir.normalized*energy / dir.magnitude;
            var forceGravity = force.y + PhysicsParams.GameGravity2;
            force.y = forceGravity > 0 ? forceGravity : 0;
            CurrentForce += force*PhysicsParams.ExplosionForce;
            CurrentForce += Vector3.up * PhysicsParams.ExplosionForce*15;
            ComputeVelocity(Time.deltaTime);
        }


        private void SimAddForce(Vector3 force)
        {
            CurrentForce += force;
        }

        private void ComputeVelocity(float dt)
        {
            _currentVelocity += CurrentForce / EntityMass * dt;

            //let's cap the speed in case its higher than the max
            if (IsInAir)
                _currentVelocity.x = Mathf.Clamp(_currentVelocity.x, -PhysicsParams.InAirMaxVelHorizontal,
                    PhysicsParams.InAirMaxVelHorizontal);
            else
                _currentVelocity.x = Mathf.Clamp(_currentVelocity.x, -PhysicsParams.OnGroundMaxVelHorizontal,
                    PhysicsParams.OnGroundMaxVelHorizontal);

            Rb.velocity = _currentVelocity;
        }

        private void ProcessInput()
        {
            var isKeyDownJump = Input.GetButton(_jumpButtonName);
            var isKeyDownAttack = Input.GetButton(_fireButtonName);
            var inputAxisX = Input.GetAxisRaw(_movementAxisName);
            var isKeyDownLeft = inputAxisX < -0.5f;
            var isKeyDownRight = inputAxisX > 0.5f;

            //JUMPING LOGIC:
            if (!IsInAir) JumpOnGround(isKeyDownJump);
            else if (IsOnWall) MoveOnWall(isKeyDownJump);
            HandleJumping(isKeyDownJump);

            //IN AIR SIDEWAYS:
            if (IsInAir) MoveInAir(isKeyDownLeft, isKeyDownRight);

            //ON GROUND SIDEWAYS:
            if (!IsInAir) MoveOnGround(isKeyDownLeft, isKeyDownRight);

            //PLAYER ATTACK
            if (isKeyDownAttack) Attack();
            else _shot = false;
        }

        private void HandleJumping(bool isKeyDownJump)
        {
            //did player lift the jump button?
            if (isKeyDownJump == false) _keyJumpPressed = false;

            //let's apply force in case we are holding the jump key during a jump.
            if (_keyJumpPressed) SimAddForce(new Vector3(0, PhysicsParams.JumpUpForce, 0) * EntityMass);
            //however let's stop doing that as soon as we fall down after the up-phase.
            if (_keyJumpPressed && _currentVelocity.y <= 0) _keyJumpPressed = false;

            //let's apply additional gravity in case we're in air moving up but not holding the jump button
            if (_currentVelocity.y > 0) SimAddForce(new Vector3(0, PhysicsParams.JumpGravity, 0) * EntityMass);
        }

        private void Attack()
        {
            if (!_shot && _energyLeft > 0)
            {
                if (_faceRight) _signalCaster.CastRight(30, true, 0);
                else if (!_faceRight) _signalCaster.CastLeft(30, true, 0);

                _energyLeft -= 1;
                _shot = true;
                lastShotTime = Time.time;
            }
        }

        private void MoveOnGround(bool isKeyDownLeft, bool isKeyDownRight)
        {
            //steering into moving direction (slow accel)
            if (isKeyDownLeft && _currentVelocity.x <= 0)
            {
                _faceRight = false;
                SimAddForce(new Vector3(-PhysicsParams.OnGroundMoveHorizontalForce, 0, 0) * EntityMass);
            }
            else if (isKeyDownRight && _currentVelocity.x >= 0)
            {
                _faceRight = true;
                SimAddForce(new Vector3(PhysicsParams.OnGroundMoveHorizontalForce, 0) * EntityMass);
            }
            //steering against moving direction (fast reverse accel)
            else if (isKeyDownLeft && _currentVelocity.x >= 0)
            {
                _faceRight = false;
                SimAddForce(new Vector3(-PhysicsParams.OnGroundMoveHorizontalForceReverse, 0, 0) * EntityMass);
            }
            else if (isKeyDownRight && _currentVelocity.x <= 0)
            {
                _faceRight = true;
                SimAddForce(new Vector3(PhysicsParams.OnGroundMoveHorizontalForceReverse, 0) * EntityMass);
            }
            //not steering -> brake due to friction.
            else if (isKeyDownLeft != true && isKeyDownRight != true && _currentVelocity.x > 0)
            {
                SimAddForce(new Vector3(-PhysicsParams.GroundFriction, 0, 0) * EntityMass);
            }
            else if (isKeyDownLeft != true && isKeyDownRight != true && _currentVelocity.x < 0)
            {
                SimAddForce(new Vector3(PhysicsParams.GroundFriction, 0, 0) * EntityMass);
            }

            //in case the velocity is close to 0 and no keys are pressed we should make the the player stop.
            //to do this let's first undo the prior friction force, and then set the velocity to 0.
            if (isKeyDownLeft != true && isKeyDownRight != true && _currentVelocity.x > 0 &&
                _currentVelocity.x < PhysicsParams.GroundFrictionEpsilon)
            {
                SimAddForce(new Vector3(PhysicsParams.GroundFriction, 0, 0) * EntityMass);
                _currentVelocity.x = 0;
            }
            else if (isKeyDownLeft != true && isKeyDownRight != true && _currentVelocity.x < 0 &&
                     _currentVelocity.x > -PhysicsParams.GroundFrictionEpsilon)
            {
                SimAddForce(new Vector3(-PhysicsParams.GroundFriction, 0, 0) * EntityMass);
                _currentVelocity.x = 0;
            }
        }

        private void MoveInAir(bool isKeyDownLeft, bool isKeyDownRight)
        {
//steering into moving direction (slow accel)
            if (isKeyDownLeft && _currentVelocity.x <= 0)
            {
                _faceRight = false;
                SimAddForce(new Vector3(-PhysicsParams.InAirMoveHorizontalForce, 0, 0) * EntityMass);
            }
            else if (isKeyDownRight && _currentVelocity.x >= 0)
            {
                _faceRight = true;
                SimAddForce(new Vector3(PhysicsParams.InAirMoveHorizontalForce, 0, 0) * EntityMass);
            }
            //steering against moving direction (fast reverse accel)
            else if (isKeyDownLeft && _currentVelocity.x >= 0)
            {
                _faceRight = false;
                SimAddForce(new Vector3(-PhysicsParams.InAirMoveHorizontalForceReverse, 0, 0) * EntityMass);
            }
            else if (isKeyDownRight && _currentVelocity.x <= 0)
            {
                _faceRight = true;
                SimAddForce(new Vector3(PhysicsParams.InAirMoveHorizontalForceReverse, 0, 0) * EntityMass);
            }
        }

        private void MoveOnWall(bool isKeyDownJump)
        {
            if (isKeyDownJump == false) _keyJumpRetrigger = true;
            if (_currentVelocity.y < 0) SimAddForce(new Vector3(0, PhysicsParams.WallFriction, 0) * EntityMass);
            if (_currentVelocity.y < PhysicsParams.WallFrictionStrongVelThreshold)
                SimAddForce(new Vector3(0, PhysicsParams.WallFrictionStrong, 0) * EntityMass);
            if (isKeyDownJump && _keyJumpRetrigger)
            {
                _keyJumpPressed = true;
                _keyJumpRetrigger = false;

                //in case we are moving down -> let's set the velocity directly
                //in case we are moving up -> sum up velocity
                if (IsOnWallLeft)
                {
                    if (_currentVelocity.y <= 0)
                        _currentVelocity = new Vector3(PhysicsParams.JumpWallVelHorizontal,
                            PhysicsParams.JumpWallVelVertical, 0);
                    else
                        _currentVelocity = new Vector3(PhysicsParams.JumpWallVelHorizontal,
                            _currentVelocity.y + PhysicsParams.JumpWallVelVertical, 0);
                }
                else if (IsOnWallRight)
                {
                    if (_currentVelocity.y <= 0)
                        _currentVelocity = new Vector3(-PhysicsParams.JumpWallVelHorizontal,
                            PhysicsParams.JumpWallVelVertical, 0);
                    else
                        _currentVelocity = new Vector3(-PhysicsParams.JumpWallVelHorizontal,
                            _currentVelocity.y + PhysicsParams.JumpWallVelVertical, 0);
                }
            }
        }

        private void JumpOnGround(bool isKeyDownJump)
        {
//in case the player is on ground and does not press the jump key, he
            //should be allowed to jump
            if (isKeyDownJump == false) _keyJumpRetrigger = true;

            //did player press down the jump button?
            if (isKeyDownJump && _keyJumpRetrigger)
            {
                _keyJumpPressed = true;
                _keyJumpRetrigger = false;

                //when pressing jump on ground we set the upwards velocity directly
                _currentVelocity = new Vector3(_currentVelocity.x, PhysicsParams.JumpUpVel, 0);
            }
        }

        public void ResetVelocity()
        {
            _currentVelocity = Vector3.zero;
        }

        public void OnCollisionStay(Collision collision)
        {
            foreach (var contactPoint in collision.contacts)
            {
                if (GetIsVectorClose(new Vector3(0, 1, 0), contactPoint.normal, 0.5f))
                {
                    _timeRealLastGroundCollision = Time.realtimeSinceStartup;
                    _currentVelocity.y = Mathf.Clamp(_currentVelocity.y, 0, Mathf.Abs(_currentVelocity.y));
                }

                if (GetIsVectorClose(new Vector3(1, 0, 0), contactPoint.normal, 0.05f))
                {
                    _timeRealLastWallLeftCollision = Time.realtimeSinceStartup;
                    _currentVelocity.x = Mathf.Clamp(_currentVelocity.x, 0, Mathf.Abs(_currentVelocity.x));
                }

                if (GetIsVectorClose(new Vector3(-1, 0, 0), contactPoint.normal, 0.05f))
                {
                    _timeRealLastWallRightCollision = Time.realtimeSinceStartup;
                    _currentVelocity.x = Mathf.Clamp(_currentVelocity.x, -Mathf.Abs(_currentVelocity.x), 0);
                }

                if (GetIsVectorClose(Vector3.down, contactPoint.normal, 0.05f))
                    _currentVelocity.y = Mathf.Clamp(_currentVelocity.y, -Mathf.Abs(_currentVelocity.y), 0);
            }
        }

        private bool GetIsVectorClose(Vector3 vectorA, Vector3 vectorB, float error)
        {
            if (Vector3.Distance(vectorA, vectorB) < error && Vector3.Distance(vectorA, vectorB) > -error)
                return true;
            return false;
        }

        public void OnLifeChanged(int life, Vector3 contactVector)
        {
            const float forceEnemyCollision = 15.0f;
            _currentVelocity = contactVector.normalized * forceEnemyCollision;
        }

        public void ResetPlayer()
        {
            _currentVelocity = Vector2.zero;
        }
        
        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag(deathTag))
            {
                _playerState = false;
                enabledController = false;
            }
        }

    }
}