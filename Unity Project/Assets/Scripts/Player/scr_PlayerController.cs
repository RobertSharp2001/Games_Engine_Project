using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SettingsModel;

public class scr_PlayerController : MonoBehaviour {
    // Start is called before the first frame update

    [HideInInspector]
    public Player_Input player_input;
    //[HideInInspector]
    public Vector2 input_movement;
    [HideInInspector]
    public Vector2 input_view;
    public Vector2 mouse_position;
    private CharacterController characterController;

    private Vector3 newCameraRotation;
    private Vector3 newCharacterRotation;
    [Header("References")]
    public Transform cameraHolder;
    public Camera camera;
    public Transform feetTransform;
    public PhysicMaterial physics;

    [Header("Settings")]
    public PlayerSettings playersettings;
    public LayerMask playerMask;
    public LayerMask groundMask;
    [Header("Gravity")]
    public float gravity;
    public float gravityMin;
    public float fallSpeed;
    public float playerGravity;
    
    private Vector3 jumpingForce;
    private Vector3 jumpingForceVelocity;

    [Header("Stance")]
    public PlayerStance stance;

    public CharacterStance playerStandStance;
    public CharacterStance playerCrouchStance;
    public CharacterStance playerProneStance;
    public float playerStanceSmoothing;
    private float stanceCheckErrorMargin = 0.05f;

    private float cameraHeight;
    private float cameraHeightVelocity;

    private Vector3 stanceCapsuleCenterVelocity;    
    private float stanceHeightCenterVelocity;

    public bool isSprinting;
    public bool isGrounded;
    public bool isFalling;

    private Vector3 newMovementSpeed;
    private Vector3 newMovementSpeedVelocity;

    [Header("Weapons")]
    public List<scr_WeaponController> weapons;
    public scr_WeaponController currentWaepon;
    public float weaponAnimationSpeed;

    [Header("Aiming")]
    public bool isAiming;

    [Header("Leaning")]
    public Transform LeanPivot;
    private float currentLean;
    private float targetLean;
    public float LeanAngle;
    public float LeanSmoothing;
    private float LeanVelocity;
    public bool isLeaning;


    [Header("Wallrunning")]
    public bool isWallrunning;
    public LayerMask whatIsWall;

    [Header("Wallrun Detection")]
    public float wallCheckDistance;
    public RaycastHit leftWallHit;
    public RaycastHit rightWallHit;
    public Vector3 WallNormal;
    public bool rightWall;
    public bool leftWall;
    public bool canWallJump;

    public bool canMove;
    public GameObject respawnMenu;

    public int weaponIndex;

    #region awake
    void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
        Physics.IgnoreLayerCollision(3, 8);

        player_input = new Player_Input();

        characterController.sharedMaterial = physics;

        player_input.Player.Movement.performed += e => input_movement = e.ReadValue<Vector2>();
        player_input.Player.View.performed += e => input_view = e.ReadValue<Vector2>();
        player_input.Player.MousePosition.performed += e => mouse_position = e.ReadValue<Vector2>();
        player_input.Player.Jump.performed += e => Jump();
        player_input.Player.Crouch.performed += e => Crouch();
        player_input.Player.Prone.performed += e => Prone();
        player_input.Player.Sprint.performed += e => Sprint();
        player_input.Player.SprintReleased.performed += e => Stop_Sprint();
        player_input.Player.ChangeGun.performed += e => SetItemByKeyValue(e);

        player_input.Weapon.Fire2Pressed.performed += e => AimingPressed();
        player_input.Weapon.Fire1Pressed.performed += e => ShootPressed();
        player_input.Weapon.Fire2Released.performed += e => AimingReleased();
        player_input.Weapon.Fire1Released.performed += e => ShootReleased();


        player_input.Player.LeanLeft.performed += e => LeanWithToggle(false);
        player_input.Player.LeanLeft.canceled += e => StopLean();
        player_input.Player.LeanRight.canceled += e => StopLean();
        player_input.Player.LeanRight.performed += e => LeanWithToggle(true);

        player_input.Enable();

        newCameraRotation = cameraHolder.localRotation.eulerAngles;
        newCharacterRotation = transform.localRotation.eulerAngles;

        cameraHeight = cameraHolder.localPosition.y;

        //HEY WHEN YOU ADD GUN SWITCHING PROPELY, YOU'll NEED THIS
        for(int i = 0; i < weapons.Count; i++){
            weapons[i].Initialize(this);
            weapons[i].gameObject.SetActive(false);
        }

        currentWaepon = weapons[0];
        weaponIndex = 0;

        if (currentWaepon){
            currentWaepon.Initialize(this);
            weapons[0].gameObject.SetActive(true);
        }
    }

    private void SetItemByKeyValue(UnityEngine.InputSystem.InputAction.CallbackContext ctx){

        int numKeyValue; // the number key value we want from this keypress

        int.TryParse(ctx.control.name, out numKeyValue);
        // Warning! If ctx.control.name can't parse as an int, numKeyValue will be 0

        Debug.Log("int value of keypress is: " + numKeyValue);

        // Now do something with the key value ...

        if (weapons[weaponIndex] != null){
            weapons[weaponIndex].gameObject.SetActive(false);
        }
        if (currentWaepon != null){
            currentWaepon.gameObject.SetActive(false);
        }
        

        for (int i = 0; i < weapons.Count; i++){

            weaponIndex = numKeyValue - 1;

            if (i != weaponIndex && weapons[i] !=null){
                weapons[i].gameObject.SetActive(false);
            }else if(i == weaponIndex && weapons[i] != null) {
                weapons[i].gameObject.SetActive(true);
            }

            currentWaepon = weapons[weaponIndex];


        }

    }

    private void Toggle_Gun(){

        if(currentWaepon.gameObject.activeSelf == true) {
            currentWaepon.gameObject.SetActive(false);
        } else{
            currentWaepon.gameObject.SetActive(true);
        }
        
       
    }
    #endregion

    #region update
    // Update is called once per frame
    void Update() {
        if (playersettings.health <= 0){
            canMove = false;
            respawnMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        } else {
            canMove = true;
        }
        
        if (canMove) {
            SetIsGrounded();
            SetIsFalling();
            CheckForWall();

            if (!leftWall && !rightWall && !isLeaning){
                isWallrunning = false;
                canWallJump = false;
                StopLean();
            }

            if (characterController.isGrounded){
                canWallJump = false;
            }

            CalculateView();
            CalculateMovement();

            CalculateJump();
            CalculateStance();
            CalculateLeaning();
            CalculateAiming();

            //test = stanceCheck(playerCrouchStance.StanceCollider.height);
        }

    }
    #endregion

    #region aiming
    public void AimingPressed(){
        if (!isSprinting && isGrounded) {
            isAiming = true;
        }
        //Debug.Log(isAiming);
    }

    public void AimingReleased(){
        isAiming = false;
        //Debug.Log(isAiming);
    }

    public void CalculateAiming(){
        if (!currentWaepon){
            return;
        }
        currentWaepon.isAiming = isAiming;
    }
    #endregion

    #region Leaning

    //Leaning with controls
    private void LeanWithToggle(bool negative){
        if (negative){
            targetLean = -LeanAngle;
        } else{
            targetLean = LeanAngle;  
        }

        isLeaning = true;
    }

    //leaning on walls
    private void Lean(bool negative){
        if (negative){
            targetLean = -LeanAngle;
        } else {
            targetLean = LeanAngle;
        }
    }

    private void StopLean(){
        targetLean = 0;
        isLeaning = false;
    }

    private void CalculateLeaning(){
        currentLean = Mathf.SmoothDamp(currentLean, targetLean, ref LeanVelocity, LeanSmoothing);
        
        LeanPivot.localRotation = Quaternion.Euler(new Vector3(0,0,currentLean));
    }
    #endregion

    private void CheckForWall(){
        leftWall = Physics.Raycast(transform.position, -cameraHolder.right, out leftWallHit, wallCheckDistance, whatIsWall);
        rightWall = Physics.Raycast(transform.position, cameraHolder.right, out rightWallHit, wallCheckDistance, whatIsWall);
    }

    public void TakeDamage(int damage){
        playersettings.health -= damage;
    }


    #region fall/Ground

    private void SetIsGrounded() {
        isGrounded = Physics.CheckSphere(feetTransform.position, playersettings.isGroundedRadius, groundMask);
        fallSpeed = playersettings.FallSpeed;
    }

    private void SetIsFalling(){
        isFalling = (!isGrounded && characterController.velocity.magnitude > playersettings.FallSpeed);
        fallSpeed = playersettings.FallSpeed / 7;
    }

    #endregion

    #region shooting

    private void ShootPressed(){
        if (currentWaepon){
            isSprinting = false;
            currentWaepon.isShooting = true;
        }
    }

    private void ShootReleased(){
        if (currentWaepon) {
            currentWaepon.isShooting = false;
        }
    }

    #endregion

    #region movement
    private void CalculateMovement() {
        
        var verticalSpeed = playersettings.walkForwardSpeed;
        var horizontalSpeed = playersettings.walkStrafeSpeed;

        if (isSprinting){
            verticalSpeed = playersettings.runForwardSpeed;
            horizontalSpeed = playersettings.runStrafeSpeed;
        }
        //Set player stance speed.
        if (!isGrounded){
            playersettings.SpeedEffector = playersettings.FallSpeedEffector;
        } else if(stance == PlayerStance.Crouch){
            playersettings.SpeedEffector = playersettings.CrouchSpeedEffector;
        } else if (stance == PlayerStance.Prone){
            playersettings.SpeedEffector = playersettings.ProneSpeedEffector;
        } else if (isAiming) {
            playersettings.SpeedEffector = playersettings.AimSpeedEffector;
        }else if (isWallrunning){
            playersettings.SpeedEffector = playersettings.WallRunEffector;
        }
        else {
            playersettings.SpeedEffector = 1;
        }

        weaponAnimationSpeed = characterController.velocity.magnitude / (playersettings.walkForwardSpeed * playersettings.SpeedEffector);
        if (weaponAnimationSpeed > 1){ weaponAnimationSpeed = 1; }

        //Set player speed basewd on their stance
        verticalSpeed *= playersettings.SpeedEffector;
        horizontalSpeed *= playersettings.SpeedEffector;

        newMovementSpeed = Vector3.SmoothDamp(newMovementSpeed, new Vector3(horizontalSpeed * input_movement.x * Time.deltaTime, 0, verticalSpeed * input_movement.y * Time.deltaTime), ref newMovementSpeedVelocity, isGrounded ? playersettings.movementSmoothing : playersettings.FallSmoothing);
        newMovementSpeed = Vector3.ClampMagnitude(newMovementSpeed, 1f);
        var movementSpeed = transform.TransformDirection(newMovementSpeed);


        if (playerGravity > gravityMin) {
            playerGravity -= gravity * Time.deltaTime;
        }
        if (playerGravity < fallSpeed && isGrounded){
            playerGravity = fallSpeed;
        }

        if (jumpingForce.y < 0.001f) {
            jumpingForce.y = 0;
            jumpingForceVelocity.y = 0;
        }
        //Apply gravity
        movementSpeed.y += playerGravity;
        movementSpeed += jumpingForce * Time.deltaTime;
        //Clamp the values to prevent the player from randomnly speeding up
        movementSpeed.x = Mathf.Clamp(movementSpeed.x, -playersettings.runForwardSpeed, playersettings.runForwardSpeed);
        movementSpeed.z = Mathf.Clamp(movementSpeed.z, -playersettings.runStrafeSpeed, playersettings.runStrafeSpeed);
        //Move
        characterController.Move(movementSpeed);
    }
    #endregion

    #region view
    private void CalculateView() {

        newCameraRotation.x += ((isAiming ? playersettings.ViewYSensitivity * playersettings.AimSensitivity : playersettings.ViewYSensitivity) * (playersettings.ViewYinverted ? input_view.y : -input_view.y) * Time.deltaTime) * PlayerPrefs.GetFloat("Sensitivity");
        newCharacterRotation.y += ((isAiming ? playersettings.ViewXSensitivity * playersettings.AimSensitivity : playersettings.ViewXSensitivity) * (playersettings.ViewXinverted ? -input_view.x : input_view.x) * Time.deltaTime) * PlayerPrefs.GetFloat("Sensitivity");
        newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, -70f, 80f);
        //newCharacterRotation.y = Mathf.Clamp(newCharacterRotation.y, -360f, 360f);

        cameraHolder.localRotation = Quaternion.Euler(newCameraRotation);
        transform.localRotation = Quaternion.Euler(newCharacterRotation);
    }
    #endregion

    #region jump
    private void CalculateJump(){
        jumpingForce = Vector3.SmoothDamp(jumpingForce, Vector3.zero, ref jumpingForceVelocity, playersettings.JumpingFalloff);
    }
    private void Jump(){
        isSprinting = false;
        isAiming = false;

        if (!isGrounded && !canWallJump || stance == PlayerStance.Prone) {
            return;
        }

        if (stance == PlayerStance.Crouch){
            if (stanceCheck(playerStandStance.StanceCollider.height)){
                return;
            }

            stance = PlayerStance.Stand;
            return;
        }

        isGrounded = false;

        if (!canWallJump){
            jumpingForce = Vector3.up * playersettings.JumpingHeight;
            playerGravity = 0;
        } else {
            float gravImplus = playersettings.JumpingHeight;
            jumpingForce = WallNormal * (playersettings.JumpingHeight) + (Vector3.up * playersettings.JumpingHeight * 1.5f);
            playerGravity = 0;
            canWallJump = false;
        }


        currentWaepon.TriggerJump();
    }
    #endregion
    

    #region crouching
    public void Crouch(){
        isSprinting = false;
        if(playerCrouchStance.StanceCollider == null || playerCrouchStance.StanceCollider == null) {
            return;
        }
        if (stance == PlayerStance.Crouch) {
            if (stanceCheck(playerStandStance.StanceCollider.height)) {
                return;
            }
            stance = PlayerStance.Stand;
            return;
        }
        if (stanceCheck(playerCrouchStance.StanceCollider.height)){
            return;
        }
        stance = PlayerStance.Crouch;
        
    }
    private void CalculateStance(){
        var CurrentStance = playerStandStance;

        switch (stance){
            case PlayerStance.Stand:
                CurrentStance = playerStandStance;
                break;
            case PlayerStance.Crouch:
                CurrentStance = playerCrouchStance;
                break;
            case PlayerStance.Prone:
                CurrentStance = playerProneStance;
                break;
        }


        cameraHeight = Mathf.SmoothDamp(cameraHolder.localPosition.y, CurrentStance.CameraHeight, ref cameraHeightVelocity, playerStanceSmoothing);
        cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x, cameraHeight, cameraHolder.localPosition.z);

        characterController.height = Mathf.SmoothDamp(characterController.height, CurrentStance.StanceCollider.height, ref stanceHeightCenterVelocity, playerStanceSmoothing);
        characterController.center = Vector3.SmoothDamp(characterController.center, CurrentStance.StanceCollider.center, ref stanceCapsuleCenterVelocity, playerStanceSmoothing);
    }
    public void Prone(){
        isSprinting = false;

        if (playerStandStance.StanceCollider == null){
            return;
        }

        if (stance == PlayerStance.Prone){
            if (stanceCheck(playerStandStance.StanceCollider.height)){
                return;
            }
            stance = PlayerStance.Stand;
            return;
        }

        stance = PlayerStance.Prone;
    }
    private bool stanceCheck(float Stanceheight) {
        Vector3 start = new Vector3(feetTransform.position.x, feetTransform.position.y + characterController.radius + stanceCheckErrorMargin, feetTransform.position.z);
        Vector3 end = new Vector3(feetTransform.position.x, feetTransform.position.y - characterController.radius - stanceCheckErrorMargin + Stanceheight, feetTransform.position.z);

        return Physics.CheckCapsule(start, end, characterController.radius, playerMask);
    }
    #endregion

    #region sprint
    public void Sprint(){
        isAiming = false;
        if (input_movement.y <= 0.2f){
            isSprinting = false;
            return;
        }

        if(stance == PlayerStance.Crouch || stance == PlayerStance.Prone) {
            return;
        }

        isSprinting = true;
    }

    public void Stop_Sprint(){
        if (!playersettings.ToggleSprint){
            isSprinting = false;
        }
              
    }
    #endregion

    #region Gizmos
    public void OnDrawGizmos(){
        Gizmos.DrawWireSphere(feetTransform.position, playersettings.isGroundedRadius);
    }
    #endregion

    void OnControllerColliderHit(ControllerColliderHit hit){
        if (hit.gameObject.tag == ("Wall") && (leftWall||rightWall)){
            playerGravity = 0f;
            jumpingForceVelocity.y = 0;

            WallNormal = hit.normal;
            jumpingForce.y = 0;
            isWallrunning = true;
            canWallJump = true;

            //Add something to push off with.

            //addd camera tilt here
            if (leftWall){
                Lean(true);
            }

            if (rightWall){
                Lean(false);
            }
        }

        if (hit.gameObject.tag == ("KillPlane")){
            playersettings.health = 0;
            Cursor.lockState = CursorLockMode.None;
            respawnMenu.SetActive(true);
        }
    }

}
