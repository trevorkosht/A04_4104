using UnityEngine;
using System.Collections;

public class WandAnimation : MonoBehaviour
{
    [Header("Bobbing Settings")]
    public float idleBobbingSpeed = 1f;
    public float idleBobbingAmount = 0.05f;
    public float movingBobbingSpeed = 2f;
    public float movingBobbingAmount = 0.1f;

    [Header("Tilt Settings")]
    public float tiltAngle = 10f;
    public float tiltReturnSpeed = 5f;

    private Vector3 initialPosition;
    private Quaternion initialRotation; // Store this to reset after swings
    private bool isMoving;
    private float bobTimer;
    private float currentTilt;

    // NEW: State tracking
    public bool IsSwinging { get; private set; }

    private void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    private void Update()
    {
        // 1. If we are swinging, skip the idle/movement procedural animations
        if (IsSwinging) return;

        // Check for player movement
        isMoving = Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f;

        UpdateBobbing();
        UpdateTilt();
    }

    private void UpdateBobbing()
    {
        float speed = isMoving ? movingBobbingSpeed : idleBobbingSpeed;
        float amount = isMoving ? movingBobbingAmount : idleBobbingAmount;

        bobTimer += Time.deltaTime * speed;
        float verticalBob = Mathf.Sin(bobTimer) * amount;

        // Apply position
        transform.localPosition = initialPosition + new Vector3(0, verticalBob, 0);
    }

    private void UpdateTilt()
    {
        float targetTilt = -Input.GetAxis("Horizontal") * tiltAngle;
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, tiltReturnSpeed * Time.deltaTime);
        transform.localRotation = initialRotation * Quaternion.Euler(0, 0, currentTilt);
    }

    // NEW: Public API to trigger a procedural swing
    public IEnumerator PlaySwingRoutine(WandSwingSO swingData)
    {
        IsSwinging = true;
        float timer = 0f;

        Quaternion startRot = initialRotation * Quaternion.Euler(swingData.startRotation);
        Quaternion endRot = initialRotation * Quaternion.Euler(swingData.endRotation);

        while (timer < swingData.duration)
        {
            timer += Time.deltaTime;
            float progress = timer / swingData.duration;
            float curveValue = swingData.swingCurve.Evaluate(progress);

            // Interpolate Rotation
            transform.localRotation = Quaternion.Slerp(startRot, endRot, curveValue);

            // Optional: Add a slight forward "punch" or position offset
            Vector3 currentPunch = Vector3.Lerp(Vector3.zero, swingData.punchOffset, Mathf.Sin(progress * Mathf.PI));
            transform.localPosition = initialPosition + currentPunch;

            yield return null;
        }

        // Return to idle smoothly
        float resetDuration = 0.15f;
        float resetTimer = 0f;
        Quaternion finalRot = transform.localRotation;

        while (resetTimer < resetDuration)
        {
            resetTimer += Time.deltaTime;
            float t = resetTimer / resetDuration;
            transform.localRotation = Quaternion.Slerp(finalRot, initialRotation, t);
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, t);
            yield return null;
        }

        IsSwinging = false;
    }
}