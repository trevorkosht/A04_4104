using UnityEngine;

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
    private bool isMoving;
    private Vector3 smoothedPosition;
    private float bobTimer;
    private float currentTilt;

    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        // Check for player movement
        isMoving = Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f;

        UpdateBobbing();

        UpdateTilt();
    }


    private void UpdateBobbing()
    {
        // Select bobbing parameters based on movement
        float speed = isMoving ? movingBobbingSpeed : idleBobbingSpeed;
        float amount = isMoving ? movingBobbingAmount : idleBobbingAmount;

        // Calculate bobbing offset
        bobTimer += Time.deltaTime * speed;
        float verticalBob = Mathf.Sin(bobTimer) * amount;

        // Apply all position changes
        transform.localPosition = initialPosition + smoothedPosition + new Vector3(0, verticalBob, 0);
    }

    private void UpdateTilt()
    {
        // Calculate tilt based on horizontal movement
        float targetTilt = -Input.GetAxis("Horizontal") * tiltAngle;

        // Smooth the tilt
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, tiltReturnSpeed * Time.deltaTime);

        // Apply rotation
        transform.localRotation = Quaternion.Euler(0, 0, currentTilt);
    }
}
