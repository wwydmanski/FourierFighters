using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

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

        private SignalCaster _signalCaster;

        private float _timeRealLastGroundCollision;
        private float _timeRealLastWallLeftCollision;
        private float _timeRealLastWallRightCollision;

        [HideInInspector] public int playerNumber;
        private Rigidbody Rb { get; set; }
        private string _movementAxisName;
        private string _fireButtonName;
        private string _jumpButtonName;

        public Vector3 Velocity => Rb.velocity;

        public Vector3 VelocityRelativeGround => Velocity / PhysicsParams.onGroundMaxVelHorizontal;

        public bool IsOnGround => GetIsColliding(_timeRealLastGroundCollision);

        public bool IsOnWallLeft => GetIsColliding(_timeRealLastWallLeftCollision);

        public bool IsOnWallRight => GetIsColliding(_timeRealLastWallRightCollision);

        public bool IsInAir { get; private set; }

        private float EntityMass => PhysicsParams.playerMass;

        [field: SerializeField] public PhysicsParams PhysicsParams { get; set; }

        public Vector3 CurrentForce { get; private set; } = Vector3.zero;

        public bool IsOnWall { get; private set; }

        public List<Renderer> AllRenderers { get; private set; }

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
            _movementAxisName = "Horizontal" + playerNumber;
            _fireButtonName = "Fire" + playerNumber;
            _jumpButtonName = "Jump" + playerNumber;
        }

        // Update is called once per frame
        public void Update()
        {
            //let's reset forces to 0 and then add regular gravitation
            SimResetForce();
            SimAddForce(new Vector3(0, PhysicsParams.gameGravity, 0) * EntityMass);

            //process key input (like jumping key pressed, etc...)
            ProcessInput();

            //simulate position and velocity based on all acting forces
            ComputeVelocity(Time.deltaTime);

            //collision detection with static world
            IsOnWall = IsOnWallLeft || IsOnWallRight;
            IsInAir = IsOnGround == false;
        }

        private void SimResetForce()
        {
            CurrentForce = Vector3.zero;
        }

        public void ProjectileHit(Vector3 force)
        {
            Debug.Log("Before hit: "+_currentVelocity);
            CurrentForce += force*PhysicsParams.projectileForce;
            ComputeVelocity(Time.deltaTime);
            Debug.Log("After hit: "+_currentVelocity);
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
                _currentVelocity.x = Mathf.Clamp(_currentVelocity.x, -PhysicsParams.inAirMaxVelHorizontal,
                    PhysicsParams.inAirMaxVelHorizontal);
            else
                _currentVelocity.x = Mathf.Clamp(_currentVelocity.x, -PhysicsParams.onGroundMaxVelHorizontal,
                    PhysicsParams.onGroundMaxVelHorizontal);

            Rb.velocity = _currentVelocity;
        }

        private void ProcessInput()
        {
            var isKeyDownJump = Input.GetButton(_jumpButtonName);
            var isKeyDownAttack = Input.GetButton(_fireButtonName);
            var inputAxisX = Input.GetAxisRaw(_movementAxisName);
            var isKeyDownLeft = inputAxisX < -0.5f;
            var isKeyDownRight = inputAxisX > 0.5f;

            //-----------------
            //JUMPING LOGIC:
            //player is on ground
            if (IsInAir == false)
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
                    _currentVelocity = new Vector3(_currentVelocity.x, PhysicsParams.jumpUpVel, 0);
                }
            }
            else if (IsOnWall)
            {
                //let's allow jumping again in case of being on the wall
                if (isKeyDownJump == false) _keyJumpRetrigger = true;
                if (_currentVelocity.y < 0) SimAddForce(new Vector3(0, PhysicsParams.wallFriction, 0) * EntityMass);
                if (_currentVelocity.y < PhysicsParams.wallFrictionStrongVelThreshold)
                    SimAddForce(new Vector3(0, PhysicsParams.wallFrictionStrong, 0) * EntityMass);
                if (isKeyDownJump && _keyJumpRetrigger)
                {
                    _keyJumpPressed = true;
                    _keyJumpRetrigger = false;

                    //in case we are moving down -> let's set the velocity directly
                    //in case we are moving up -> sum up velocity
                    if (IsOnWallLeft)
                    {
                        if (_currentVelocity.y <= 0)
                            _currentVelocity = new Vector3(PhysicsParams.jumpWallVelHorizontal,
                                PhysicsParams.jumpWallVelVertical, 0);
                        else
                            _currentVelocity = new Vector3(PhysicsParams.jumpWallVelHorizontal,
                                _currentVelocity.y + PhysicsParams.jumpWallVelVertical, 0);
                    }
                    else if (IsOnWallRight)
                    {
                        if (_currentVelocity.y <= 0)
                            _currentVelocity = new Vector3(-PhysicsParams.jumpWallVelHorizontal,
                                PhysicsParams.jumpWallVelVertical, 0);
                        else
                            _currentVelocity = new Vector3(-PhysicsParams.jumpWallVelHorizontal,
                                _currentVelocity.y + PhysicsParams.jumpWallVelVertical, 0);
                    }
                }
            }

            //did player lift the jump button?
            if (isKeyDownJump == false) _keyJumpPressed = false;

            //let's apply force in case we are holding the jump key during a jump.
            if (_keyJumpPressed) SimAddForce(new Vector3(0, PhysicsParams.jumpUpForce, 0) * EntityMass);
            //however let's stop doing that as soon as we fall down after the up-phase.
            if (_keyJumpPressed && _currentVelocity.y <= 0) _keyJumpPressed = false;

            //let's apply additional gravity in case we're in air moving up but not holding the jump button
            if (_currentVelocity.y > 0) SimAddForce(new Vector3(0, PhysicsParams.jumpGravity, 0) * EntityMass);

            //-----------------
            //IN AIR SIDEWAYS:
            if (IsInAir)
            {
                //steering into moving direction (slow accel)
                if (isKeyDownLeft && _currentVelocity.x <= 0)
                {
                    _faceRight = false;
                    SimAddForce(new Vector3(-PhysicsParams.inAirMoveHorizontalForce, 0, 0) * EntityMass);
                }
                else if (isKeyDownRight && _currentVelocity.x >= 0)
                {
                    _faceRight = true;
                    SimAddForce(new Vector3(PhysicsParams.inAirMoveHorizontalForce, 0, 0) * EntityMass);
                }
                //steering against moving direction (fast reverse accel)
                else if (isKeyDownLeft && _currentVelocity.x >= 0)
                {
                    _faceRight = false;
                    SimAddForce(new Vector3(-PhysicsParams.inAirMoveHorizontalForceReverse, 0, 0) * EntityMass);
                }
                else if (isKeyDownRight && _currentVelocity.x <= 0)
                {
                    _faceRight = true;
                    SimAddForce(new Vector3(PhysicsParams.inAirMoveHorizontalForceReverse, 0, 0) * EntityMass);
                }
            }

            //-----------------
            //ON GROUND SIDEWAYS:
            if (IsInAir == false)
            {
                //steering into moving direction (slow accel)
                if (isKeyDownLeft && _currentVelocity.x <= 0)
                {
                    _faceRight = false;
                    SimAddForce(new Vector3(-PhysicsParams.onGroundMoveHorizontalForce, 0, 0) * EntityMass);
                }
                else if (isKeyDownRight && _currentVelocity.x >= 0)
                {
                    _faceRight = true;
                    SimAddForce(new Vector3(PhysicsParams.onGroundMoveHorizontalForce, 0) * EntityMass);
                }
                //steering against moving direction (fast reverse accel)
                else if (isKeyDownLeft && _currentVelocity.x >= 0)
                {
                    _faceRight = false;
                    SimAddForce(new Vector3(-PhysicsParams.onGroundMoveHorizontalForceReverse, 0, 0) * EntityMass);
                }
                else if (isKeyDownRight && _currentVelocity.x <= 0)
                {
                    _faceRight = true;
                    SimAddForce(new Vector3(PhysicsParams.onGroundMoveHorizontalForceReverse, 0) * EntityMass);
                }
                //not steering -> brake due to friction.
                else if (isKeyDownLeft != true && isKeyDownRight != true && _currentVelocity.x > 0)
                {
                    SimAddForce(new Vector3(-PhysicsParams.groundFriction, 0, 0) * EntityMass);
                }
                else if (isKeyDownLeft != true && isKeyDownRight != true && _currentVelocity.x < 0)
                {
                    SimAddForce(new Vector3(PhysicsParams.groundFriction, 0, 0) * EntityMass);
                }

                //in case the velocity is close to 0 and no keys are pressed we should make the the player stop.
                //to do this let's first undo the prior friction force, and then set the velocity to 0.
                if (isKeyDownLeft != true && isKeyDownRight != true && _currentVelocity.x > 0 &&
                    _currentVelocity.x < PhysicsParams.groundFrictionEpsilon)
                {
                    SimAddForce(new Vector3(PhysicsParams.groundFriction, 0, 0) * EntityMass);
                    _currentVelocity.x = 0;
                }
                else if (isKeyDownLeft != true && isKeyDownRight != true && _currentVelocity.x < 0 &&
                         _currentVelocity.x > -PhysicsParams.groundFrictionEpsilon)
                {
                    SimAddForce(new Vector3(-PhysicsParams.groundFriction, 0, 0) * EntityMass);
                    _currentVelocity.x = 0;
                }
            }

            //-----------------
            //PLAYER ATTACK
            if (isKeyDownAttack)
            {
                if (!_shot)
                {
                    if (_faceRight)
                    {
                        _signalCaster.CastRight(30, false, 0);
                        print("Right");
                    }
                    else if (!_faceRight)
                    {
                        _signalCaster.CastLeft(20, false, 0);
                        print("Left");
                    }

                    _shot = true;
                }
            }
            else
            {
                _shot = false;
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
                if (GetIsVectorClose(new Vector3(0, 1, 0), contactPoint.normal, 0.05f))
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
    }
}