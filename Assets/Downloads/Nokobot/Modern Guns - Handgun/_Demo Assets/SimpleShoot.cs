using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[AddComponentMenu("Nokobot/Modern Guns/Simple Shoot")]
public class SimpleShoot : MonoBehaviour
{
    [Header("Prefab References")]
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;

    [Header("Location References")]
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private Transform casingExitLocation;

    [Header("Settings")]
    [Tooltip("Specify time to destroy the casing object")] [SerializeField] private float destroyTimer = 2f;
    [Tooltip("Bullet Speed")] [SerializeField] private float shotPower = 500f;
    [Tooltip("Casing Ejection Speed")] [SerializeField] private float ejectPower = 150f;
    [Tooltip("Time before bullet disappears")] [SerializeField] private float bulletLifetime = 5f;

    private InputAction triggerAction;

    [Header("Haptics")]
    [Tooltip("Duration of the haptic feedback")] [SerializeField] private float hapticDuration = 0.1f;
    [Tooltip("Intensity of the haptic feedback")] [SerializeField] private float hapticIntensity = 0.75f;
    private XRBaseController rightController;

    [Header("Recoil Settings")]
    [Tooltip("Recoil angle")] [SerializeField] private float recoilAngle = 10f;
    [Tooltip("Recoil duration")] [SerializeField] private float recoilDuration = 0.1f;

    private Quaternion originalRotation;

    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;

        if (gunAnimator == null)
            gunAnimator = GetComponentInChildren<Animator>();

        triggerAction = new InputAction(binding: "<XRController>/triggerPressed");
        triggerAction.performed += context => Shoot();
        triggerAction.Enable();

        rightController = GetRightController();

        // Save the original rotation
        originalRotation = transform.localRotation;
    }

    void Shoot()
    {
        if (muzzleFlashPrefab)
        {
            // Create the muzzle flash
            GameObject tempFlash;
            tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);

            // Destroy the muzzle flash effect
            Destroy(tempFlash, destroyTimer);
        }

        // Cancel if there's no bullet prefab
        if (!bulletPrefab)
        { return; }

        // Create a bullet and add force on it in direction of the barrel
        GameObject bullet = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.AddForce(barrelLocation.forward * shotPower);

        // Destroy the bullet after a certain time
        Destroy(bullet, bulletLifetime);

        // Attach a collision detection script to the bullet
        //bullet.AddComponent<BulletCollisionDetector>();

        // Trigger haptic feedback
        TriggerHapticFeedback();

        // Trigger recoil effect
        StartCoroutine(RecoilEffect());
    }

    void CasingRelease()
    {
        // Cancel function if ejection slot hasn't been set or there's no casing
        if (!casingExitLocation || !casingPrefab)
        { return; }

        // Create the casing
        GameObject tempCasing;
        tempCasing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
        // Add force on casing to push it out
        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower), (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
        // Add torque to make casing spin in random direction
        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);

        // Destroy casing after X seconds
        Destroy(tempCasing, destroyTimer);
    }

    private void TriggerHapticFeedback()
    {
        if (rightController != null)
        {
            rightController.SendHapticImpulse(hapticIntensity, hapticDuration);
        }
    }

    private XRBaseController GetRightController()
    {
        // Assumes XRController is part of the XR Interaction Toolkit setup
        XRBaseController rightController = null;
        var controllers = FindObjectsOfType<XRBaseController>();
        foreach (var controller in controllers)
        {
            if (controller.gameObject.name.Contains("Right"))
            {
                rightController = controller;
                break;
            }
        }
        return rightController;
    }

    private void OnDisable()
    {
        // Disable trigger button action when the script is disabled
        triggerAction?.Disable();
    }

    private IEnumerator RecoilEffect()
    {
        float elapsedTime = 0f;
        Quaternion recoilRotation = Quaternion.Euler(originalRotation.eulerAngles + new Vector3(-recoilAngle, 0, 0));

        // Rotate up
        while (elapsedTime < recoilDuration)
        {
            transform.localRotation = Quaternion.Slerp(originalRotation, recoilRotation, elapsedTime / recoilDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Rotate down
        elapsedTime = 0f;
        while (elapsedTime < recoilDuration)
        {
            transform.localRotation = Quaternion.Slerp(recoilRotation, originalRotation, elapsedTime / recoilDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = originalRotation;
    }
}
