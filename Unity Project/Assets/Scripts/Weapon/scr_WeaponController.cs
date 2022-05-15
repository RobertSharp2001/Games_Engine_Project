using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SettingsModel;

public class scr_WeaponController : MonoBehaviour{
    // Start is called before the first frame update
    [Header("Animator")]
    public Animator weaponAnimator;
    [Header("Settings")]
    public weaponSettings settings;

    private scr_PlayerController playerController;

    public bool isInitialized;

    Vector3 newWeaponRotation;
    Vector3 newWeaponRotationVelocity;
    Vector3 targetWeaponRotation;
    Vector3 targetWeaponRotationVelocity;

    Vector3 newWeaponMoveRotation;
    Vector3 newWeaponMoveRotationVelocity;
    Vector3 targetWeaponMoveRotation;
    Vector3 targetWeaponMoveRotationVelocity;

    private bool isGroundedTrigger;
    public float fallDelay;

    [SerializeField]
    private TrailRenderer BulletTrail;

    [Header("Weapon Breathing")]
    public Transform weaponSwayObject;
    public float swayAmountA = 1;
    public float swayAmountB = 2;
    public float swayScale = 600;
    public float swayLerpSpeed = 14;
    private float swayTime;
    private Vector3 swayPosition;


    [Header("Aiming")]
    public bool isAiming;
    public Transform sightTarget;
    public float sightOffset;
    public float aimInTime;
    private Vector3 weaponSwayPosition;
    private Vector3 weaponSwayPositionVelocity;

    [Header("Shooting")]
    public float rateOfFire;
    private float currentFireRate;
    private float lastShot = 0.0f;
    public List<weaponFireType> allowedFireTypes;
    public weaponFireType currentFireType;
    public GameObject bulletPrefab;
    public GameObject audioPrefab;
    public Transform bulletSpawn;
    public bool isShooting;

    [Header("MuzzleFlash")]
    public GameObject flash;

    #region start/update
    public void Start() {
        newWeaponRotation = transform.localRotation.eulerAngles;
        flash.SetActive(false);
        currentFireType = allowedFireTypes.First();
        
    }

    public void Update(){
        if (!isInitialized) {
            return;
        }
        if (playerController.canMove) {
            CalculateWeaponRotation();
            setWeaponAnimation();
            CalculateWeaponBreathe();
            CalculateAiming();
            CalculateShooting();
        }

        if (!isShooting){
            flash.SetActive(false);
        }
    }
    #endregion

    #region shooting

    private void Shoot(){
        //Basically this just makes a sound and then deletes itself.
        //Rigidbody bullet = Instantiate(bulletPrefab, bulletSpawn);
        var bulletClone = Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        var audioClone = Instantiate(audioPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        Rigidbody x = bulletClone.gameObject.GetComponent<Rigidbody>();
        bulletClone.gameObject.GetComponent<projectile>().type = BulletType.Player;
        bulletClone.gameObject.GetComponent<projectile>().damage = settings.damage;
        x.velocity = transform.forward * 20f;
    }

    private IEnumerator SpawnTrail(TrailRenderer Trail, RaycastHit Hit) {
        float time = 0;
        Vector3 startPosition = Trail.transform.position;

        while (time < 1){
            Trail.transform.position = Vector3.Lerp(startPosition, Hit.point, time);
            time += Time.deltaTime / Trail.time;

            yield return null;
        }
        //Animator.SetBool("IsShooting", false);
        Trail.transform.position = Hit.point;
        //Instantiate(ImpactParticleSystem, Hit.point, Quaternion.LookRotation(Hit.normal));

        Destroy(Trail.gameObject, Trail.time);
    }

    private void CalculateShooting(){
        
        if (isShooting && Time.time > rateOfFire + lastShot) {
            Shoot();
            flash.SetActive(true);
            lastShot = Time.time;
            if (currentFireType == weaponFireType.SemiAuto){
                isShooting = false;
                flash.SetActive(false);
                return;
            }
        }
    }


    #endregion

    #region init
    public void Initialize(scr_PlayerController Controller){
        playerController = Controller;
        isInitialized = true;
    }
    #endregion

    #region aiming
    private void CalculateAiming(){
        var targetPostition = transform.position;
        if (isAiming) {
            targetPostition = playerController.camera.transform.position + (weaponSwayObject.transform.position - sightTarget.position) + (playerController.camera.transform.forward * sightOffset);
        }

        weaponSwayPosition = weaponSwayObject.transform.position;
        weaponSwayPosition = Vector3.SmoothDamp(weaponSwayPosition, targetPostition, ref weaponSwayPositionVelocity, aimInTime);

        weaponSwayObject.transform.position = weaponSwayPosition + swayPosition;
    }
    #endregion

    #region jump
    public void TriggerJump() {
        if(weaponAnimator != null){
            weaponAnimator.SetTrigger("Jump");
        }
        
            isGroundedTrigger = false;
    }
    #endregion

    #region rotattion
    public void CalculateWeaponRotation(){
        //Adjust gun model when looking around
        weaponAnimator.speed = isAiming ? 0.1f : 1f;
        
        targetWeaponRotation.x += (isAiming ? settings.swayAmount / 3 : settings.swayAmount) * (settings.SwayYInverted ? playerController.input_view.y : -playerController.input_view.y) * Time.deltaTime;
        targetWeaponRotation.y += (isAiming ? settings.swayAmount / 3 : settings.swayAmount) * (settings.SwayXInverted ? playerController.input_view.x : -playerController.input_view.x) * Time.deltaTime;

        targetWeaponRotation.x = Mathf.Clamp(targetWeaponRotation.x, -settings.SwayClampX, settings.SwayClampX);
        targetWeaponRotation.y = Mathf.Clamp(targetWeaponRotation.y, -settings.SwayClampY, settings.SwayClampY);
        targetWeaponRotation.z = isAiming ? 0 : -(targetWeaponRotation.y * 2);

        targetWeaponRotation = Vector3.SmoothDamp(targetWeaponRotation, Vector3.zero, ref targetWeaponRotationVelocity, settings.swayResetSmoothing);
        newWeaponRotation = Vector3.SmoothDamp(newWeaponRotation, targetWeaponRotation, ref newWeaponRotationVelocity, settings.swaySmoothing);

        //Adjust gun model when moving
        targetWeaponMoveRotation.z = (isAiming ? settings.MoveSwayX / 3 : settings.MoveSwayX) * (settings.MoveSwayXInverted ? -playerController.input_movement.x : playerController.input_movement.x);
        targetWeaponMoveRotation.x = (isAiming ? settings.MoveSwayY / 3 : settings.MoveSwayY) * (settings.MoveSwayYInverted ? -playerController.input_movement.y : playerController.input_movement.y);

        targetWeaponMoveRotation = Vector3.SmoothDamp(targetWeaponMoveRotation, Vector3.zero, ref targetWeaponMoveRotationVelocity, settings.moveSwaySmoothing);
        newWeaponMoveRotation = Vector3.SmoothDamp(newWeaponMoveRotation, targetWeaponMoveRotation, ref newWeaponMoveRotationVelocity, settings.moveSwaySmoothing);

        transform.localRotation = Quaternion.Euler(newWeaponRotation + newWeaponMoveRotation);
    }
    #endregion

    #region animate
    public void setWeaponAnimation(){
        if (isGroundedTrigger){
            fallDelay = 0;
        }else{
            fallDelay += Time.deltaTime;
        }

        if (playerController.isGrounded && !isGroundedTrigger && fallDelay > 0.1f){
            //Trigger the landing animation
            weaponAnimator.SetTrigger("Landing");
            isGroundedTrigger = true;
        }else if (!playerController.isGrounded && isGroundedTrigger){
            //Trigger the falling animation
            weaponAnimator.SetTrigger("Falling");
            isGroundedTrigger = false;
        }

        weaponAnimator.SetBool("isSprinting", playerController.isSprinting);
        weaponAnimator.SetFloat("weaponAnimationSpeed", playerController.weaponAnimationSpeed);
    }
    #endregion

    #region sway
    public void CalculateWeaponBreathe() {
        var targetPosition = LissajousCurve(swayTime, swayAmountA, swayAmountB) / (isAiming ? swayScale * 2: swayScale);
        swayPosition = Vector3.Lerp(swayPosition, targetPosition, Time.smoothDeltaTime * swayLerpSpeed);

        swayTime += Time.deltaTime;
        //Cap the value to prevent overflow error.
        if(swayTime > 6.3f){
            swayTime = 0;
        }
        //weaponSwayObject.localPosition = swayPosition;

    }
    //math stuff that i don't really understand but it makes the breathing follow a curve.
    private Vector3 LissajousCurve(float Time, float a, float b){
        return new Vector3(Mathf.Sin(Time), a*Mathf.Sin(b * Time +Mathf.PI) );
    }
    #endregion

    IEnumerator wait(){
        yield return new WaitForSeconds(0.05f);
    }
}
