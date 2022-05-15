using System;
using System.Collections.Generic;
using UnityEngine;

public class SettingsModel : MonoBehaviour{

    #region player
    public enum PlayerStance{
        Stand,
        Crouch,
        Prone
    }


    [Serializable]
    public class PlayerSettings {
        [Header("View settings")]
        public float ViewXSensitivity;
        public float ViewYSensitivity;
        public float AimSensitivity;

        public bool ViewXinverted;
        public bool ViewYinverted;

        [Header("Walking")]
        public float walkForwardSpeed;
        public float walkBackwardSpeed;
        public float walkStrafeSpeed;

        [Header("Movement")]
        public bool ToggleSprint;
        public float movementSmoothing;

        [Header("Sprinting")]
        public float runForwardSpeed;
        public float runBackwardSpeed;
        public float runStrafeSpeed;

        [Header("Jumping")]
        public float JumpingHeight;
        public float JumpingFalloff;
        public float FallSmoothing;

        [Header("Speed Effectors")]
        public float SpeedEffector = 1;
        public float CrouchSpeedEffector;
        public float ProneSpeedEffector;
        public float FallSpeedEffector;
        public float AimSpeedEffector;
        public float WallRunEffector;


        [Header("IsGrounded / Falling")]
        public float isGroundedRadius;
        public float FallSpeed;

        public float health;

    }
    [Serializable]
    public class CharacterStance{
        public float CameraHeight;
        public CapsuleCollider StanceCollider;

    }
    #endregion

    #region weapons
    public enum weaponFireType{
        SemiAuto,
        FullAuto
    }



    [Serializable]
    public class weaponSettings{
        [Header("Sway")]
        public float swayAmount;
        public float swaySmoothing;
        public float swayResetSmoothing;
        public bool SwayYInverted;
        public bool SwayXInverted;
        public float SwayClampX;
        public float SwayClampY;

        [Header("Movement Sway")]
        public float MoveSwayX;
        public float MoveSwayY;

        public bool MoveSwayYInverted;
        public bool MoveSwayXInverted;
        public float moveSwaySmoothing;

        [Header("Shooting")]
        public int damage;
        public float range;
    }
    #endregion

    [Serializable]
    public class entitySettings {
        public float health;
    }

    [Serializable]
        public class enemySettings{
            public float health;
            [Header("Attacking")]
            public float timeBetweenAttacks;
            public bool alreadyAttacked;

            public float sightRange, attackRange;

            [Header("States")]
            public bool alert = false;
            public bool playerinSightRange, playerinAttackRange;
    }
 }
