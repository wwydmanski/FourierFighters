using System;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

namespace PlayerController
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody rb { get; set;}

        [SerializeField]
        private PhysicsParams physicsParams;

        private SignalCaster signalCaster = null;
        private bool faceRight = true;
        private bool shot = false;
        
        public Vector3 Velocity { get { return(rb.velocity); } }

        public Vector3 VelocityRelativeGround { get { return(Velocity / PhysicsParams.onGroundMaxVelHorizontal); } }

        private float timeRealLastGroundCollision = 0;
        private float timeRealLastWallLeftCollision = 0;
        private float timeRealLastWallRightCollision = 0;

        public bool IsOnGround {
            get {
                return GetIsColliding(timeRealLastGroundCollision);
            }
        }

        public bool IsOnWallLeft {
            get {
                return GetIsColliding(timeRealLastWallLeftCollision);
            }
        }

        public bool IsOnWallRight {
            get {
                return GetIsColliding(timeRealLastWallRightCollision);
            }
        }

        public bool IsInAir { get { return isPlayerInAir; } }

        private bool GetIsColliding(float timeLastCollision) {
            return(Time.realtimeSinceStartup < timeLastCollision + 0.05f);
        }

        private Vector3 currentVelocity = Vector3.zero;
        private Vector3 currentForce = Vector3.zero;

        private float EntityMass { get { return(PhysicsParams.playerMass); } }

        private bool isPlayerInAir = false;
        private bool keyJumpRetrigger = false;
        private bool keyJumpPressed = false;
        private bool isPlayerOnWall = false;

        public PhysicsParams PhysicsParams {
            get { return physicsParams; }
            set { physicsParams = value; }
        }

        public Vector3 CurrentForce { get { return currentForce; } }

        public bool IsOnWall { get { return isPlayerOnWall; } }

        private List<Renderer> allRenderers;

        public List<Renderer> AllRenderers { get { return allRenderers; } }

        public Vector3 Position {
            get {
                return transform.position;
            }
            set {
                transform.position = value;
            }
        }
        
        public void Awake()
        {
            rb = GetComponent<Rigidbody>();
            allRenderers = new List<Renderer>(GetComponentsInChildren<Renderer>(true));
            signalCaster = GetComponent<SignalCaster>();

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
            isPlayerOnWall = IsOnWallLeft || IsOnWallRight;
            isPlayerInAir = IsOnGround == false;
        }

        private void SimResetForce() {
            currentForce = Vector3.zero;
        }

        private void SimAddForce(Vector3 force) {
            currentForce += force;
        }

        private void ComputeVelocity(float dt) {

            currentVelocity += (currentForce / EntityMass) * dt;

            //let's cap the speed in case its higher than the max
            if (isPlayerInAir) {
                currentVelocity.x = Mathf.Clamp(currentVelocity.x, -PhysicsParams.inAirMaxVelHorizontal, PhysicsParams.inAirMaxVelHorizontal);
            } else {
                currentVelocity.x = Mathf.Clamp(currentVelocity.x, -PhysicsParams.onGroundMaxVelHorizontal, PhysicsParams.onGroundMaxVelHorizontal);
            }

            rb.velocity = currentVelocity;
        }

        private void ProcessInput()
        {
            bool isKeyDownJump = Input.GetButton("Jump");
            bool isKeyDownAttack = Input.GetButton("Fire1");
            float inputAxisX = Input.GetAxisRaw("Horizontal");
            bool isKeyDownLeft = inputAxisX < -0.5f;
            bool isKeyDownRight = inputAxisX > 0.5f;

            //-----------------
      //JUMPING LOGIC:
      //player is on ground
      if (isPlayerInAir == false)
      {
          //in case the player is on ground and does not press the jump key, he
          //should be allowed to jump
          if (isKeyDownJump == false)
          {
              keyJumpRetrigger = true;
          }

          //did player press down the jump button?
          if (isKeyDownJump == true && keyJumpRetrigger == true)
          {
              keyJumpPressed = true;
              keyJumpRetrigger = false;  
              
              //when pressing jump on ground we set the upwards velocity directly
              currentVelocity = new Vector3(currentVelocity.x, PhysicsParams.jumpUpVel, 0);
          }
          } else if (isPlayerOnWall == true) {
            //let's allow jumping again in case of being on the wall
            if (isKeyDownJump == false) {
              keyJumpRetrigger = true;
            }
            if (currentVelocity.y < 0) {//apply friction when moving downwards
              SimAddForce(new Vector3(0, PhysicsParams.wallFriction, 0) * EntityMass);
            }
            if (currentVelocity.y < PhysicsParams.wallFrictionStrongVelThreshold) {//apply even more friction when moving downwards fast
              SimAddForce(new Vector3(0, PhysicsParams.wallFrictionStrong, 0) * EntityMass);
            }
            if (isKeyDownJump == true && keyJumpRetrigger == true) {
              keyJumpPressed = true;
              keyJumpRetrigger = false;
    
              //in case we are moving down -> let's set the velocity directly
              //in case we are moving up -> sum up velocity
              if (IsOnWallLeft == true) {
                if (currentVelocity.y <= 0) {
                  currentVelocity = new Vector3(PhysicsParams.jumpWallVelHorizontal, PhysicsParams.jumpWallVelVertical, 0);
                } else {
                  currentVelocity = new Vector3(PhysicsParams.jumpWallVelHorizontal, currentVelocity.y + PhysicsParams.jumpWallVelVertical, 0);
                }
              } else if (IsOnWallRight == true) {
                if (currentVelocity.y <= 0)
                  currentVelocity = new Vector3(-PhysicsParams.jumpWallVelHorizontal, PhysicsParams.jumpWallVelVertical, 0);
                else
                  currentVelocity = new Vector3(-PhysicsParams.jumpWallVelHorizontal, currentVelocity.y + PhysicsParams.jumpWallVelVertical, 0);
              }
            }
          }
          //did player lift the jump button?
          if (isKeyDownJump == false) {
            keyJumpPressed = false;
          }
    
          //let's apply force in case we are holding the jump key during a jump.
          if (keyJumpPressed == true) {
            SimAddForce(new Vector3(0, PhysicsParams.jumpUpForce, 0) * EntityMass);
          }
          //however let's stop doing that as soon as we fall down after the up-phase.
          if (keyJumpPressed == true && currentVelocity.y <= 0) {
            keyJumpPressed = false;
          }
    
          //let's apply additional gravity in case we're in air moving up but not holding the jump button
          if (currentVelocity.y > 0) {
            SimAddForce(new Vector3(0, PhysicsParams.jumpGravity, 0) * EntityMass);
          }
          
          //-----------------
          //IN AIR SIDEWAYS:
          if (isPlayerInAir == true) {
            //steering into moving direction (slow accel)
            if (isKeyDownLeft == true && currentVelocity.x <= 0)
            {
                faceRight = false;
                SimAddForce(new Vector3(-PhysicsParams.inAirMoveHorizontalForce, 0, 0) * EntityMass);
            }
            else if (isKeyDownRight == true && currentVelocity.x >= 0)
            {
                faceRight = true;
                SimAddForce(new Vector3(PhysicsParams.inAirMoveHorizontalForce, 0, 0) * EntityMass);
            }
            //steering against moving direction (fast reverse accel)
            else if (isKeyDownLeft == true && currentVelocity.x >= 0)
            {
                faceRight = false;
                SimAddForce(new Vector3(-PhysicsParams.inAirMoveHorizontalForceReverse, 0, 0) * EntityMass);
            }
            else if (isKeyDownRight == true && currentVelocity.x <= 0)
            {
                faceRight = true;
                SimAddForce(new Vector3(PhysicsParams.inAirMoveHorizontalForceReverse, 0, 0) * EntityMass);
            }
          }
    
          //-----------------
          //ON GROUND SIDEWAYS:
          if (isPlayerInAir == false) {
            //steering into moving direction (slow accel)
            if (isKeyDownLeft == true && currentVelocity.x <= 0)
            {
                faceRight = false;
                SimAddForce(new Vector3(-PhysicsParams.onGroundMoveHorizontalForce, 0, 0) * EntityMass);
            }
            else if (isKeyDownRight == true && currentVelocity.x >= 0)
            {
                faceRight = true;
                SimAddForce(new Vector3(PhysicsParams.onGroundMoveHorizontalForce, 0) * EntityMass);
            }
            //steering against moving direction (fast reverse accel)
            else if (isKeyDownLeft == true && currentVelocity.x >= 0)
            {
                faceRight = false;
                SimAddForce(new Vector3(-PhysicsParams.onGroundMoveHorizontalForceReverse, 0, 0) * EntityMass);
            }
            else if (isKeyDownRight == true && currentVelocity.x <= 0)
            {
                faceRight = true;
                SimAddForce(new Vector3(PhysicsParams.onGroundMoveHorizontalForceReverse, 0) * EntityMass);
            }
            //not steering -> brake due to friction.
            else if (isKeyDownLeft != true && isKeyDownRight != true && currentVelocity.x > 0)
              SimAddForce(new Vector3(-PhysicsParams.groundFriction, 0, 0) * EntityMass);
            else if (isKeyDownLeft != true && isKeyDownRight != true && currentVelocity.x < 0)
              SimAddForce(new Vector3(PhysicsParams.groundFriction, 0, 0) * EntityMass);
    
            //in case the velocity is close to 0 and no keys are pressed we should make the the player stop.
            //to do this let's first undo the prior friction force, and then set the velocity to 0.
            if (isKeyDownLeft != true && isKeyDownRight != true && currentVelocity.x > 0 && currentVelocity.x < PhysicsParams.groundFrictionEpsilon) {
              SimAddForce(new Vector3(PhysicsParams.groundFriction, 0, 0) * EntityMass);
              currentVelocity.x = 0;
            } else if (isKeyDownLeft != true && isKeyDownRight != true && currentVelocity.x < 0 && currentVelocity.x > -PhysicsParams.groundFrictionEpsilon) {
              SimAddForce(new Vector3(-PhysicsParams.groundFriction, 0, 0) * EntityMass);
              currentVelocity.x = 0;
            }
          }
          //-----------------
          //PLAYER ATTACK
          if (isKeyDownAttack == true)
          {
              if (!shot)
              {
                  if (faceRight)
                  {
                      signalCaster.CastRight(30, false, 0);
                      print("Right");
                  }
                  else if (!faceRight)
                  {
                      signalCaster.CastLeft(20, false, 0);
                      print("Left");
                  }

                  shot = true;
              }
          }
          else
              shot = false;
        }

        public void ResetVelocity() {
            currentVelocity = Vector3.zero;
        }
 
        public void OnCollisionStay(Collision collision) {

            foreach (ContactPoint contactPoint in collision.contacts) {
                if (GetIsVectorClose(new Vector3(0, 1, 0), contactPoint.normal, 0.05f))
                {
                    
                    timeRealLastGroundCollision = Time.realtimeSinceStartup;
                    currentVelocity.y = Mathf.Clamp(currentVelocity.y, 0, Mathf.Abs(currentVelocity.y));
                }
                if (GetIsVectorClose(new Vector3(1, 0, 0), contactPoint.normal, 0.05f)) {
                    timeRealLastWallLeftCollision = Time.realtimeSinceStartup;
                    currentVelocity.x = Mathf.Clamp(currentVelocity.x, 0, Mathf.Abs(currentVelocity.x));
                }
                if (GetIsVectorClose(new Vector3(-1, 0, 0), contactPoint.normal, 0.05f)) {
                    timeRealLastWallRightCollision = Time.realtimeSinceStartup;
                    currentVelocity.x = Mathf.Clamp(currentVelocity.x, -Mathf.Abs(currentVelocity.x), 0);
                }
                if(GetIsVectorClose(Vector3.down, contactPoint.normal, 0.05f)) {
                    currentVelocity.y = Mathf.Clamp(currentVelocity.y, -Mathf.Abs(currentVelocity.y), 0);
                }
            }
        }

        private bool GetIsVectorClose(Vector3 vectorA, Vector3 vectorB, float error) {
            if (Vector3.Distance(vectorA, vectorB) < error && Vector3.Distance(vectorA, vectorB) > -error)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public void OnLifeChanged (int life, Vector3 contactVector) {
            const float forceEnemyCollision = 15.0f;
            currentVelocity = contactVector.normalized * forceEnemyCollision;
        }
      
        public void ResetPlayer() {
            currentVelocity = Vector2.zero;
        }
    }
}