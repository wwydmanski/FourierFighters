using UnityEngine;
using System.Collections;

namespace PlayerController {

    [System.Serializable]
    public class PhysicsParams {

        public float PlayerMass = 4f;
        public float GameGravity2 = -40f;
        public float JumpUpForce = 0.25f;
        public float JumpUpVel = 18.75f;
        [Tooltip("Additional gravity for jumping in case we're not holding down the jump button. Supposed to act as a penalty.")]
        public float JumpGravity = -22.5f;
        public float JumpWallVelVertical = 12;
        public float JumpWallVelHorizontal = 7.5f;
        [Tooltip("Maximum velocity sideways.")]
        public float InAirMaxVelHorizontal = 12.5f;
        [Tooltip("The force that acts when the players moves sideways in air.")]
        public float InAirMoveHorizontalForce = 12.5f;
        [Tooltip("The force that acts when the player is moving in one direction, but steering into the other.")]
        public float InAirMoveHorizontalForceReverse = 28.75f;
        public float OnGroundMaxVelHorizontal = 15;
        public float OnGroundMoveHorizontalForce = 80;
        public float OnGroundMoveHorizontalForceReverse = 165;
        public float GroundFriction = 35;
        [Tooltip("When velocity is lower than this value the player stops completely.")]
        public float GroundFrictionEpsilon = 0.75f;
        public float WallFriction = 11.25f;
        public float WallFrictionStrongVelThreshold = -10;
        [Tooltip("Gets applied when moving down along the wall too fast.")]
        public float WallFrictionStrong = 22.5f;

        public float ProjectileForce = 1000;
        public float ExplosionForce = 10;

        public float EnergyRechargeRate = 0.2f;
        public float EnergyRechargeShotDelay = 1f;
        public float FullDischargeDelay = 2f;


        #region serialization version
        [HideInInspector]
        public int Version = 1;
        #endregion
    }
}
