using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private const float minimalAcceptableVelocity = 0.01f;

    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [Space(10)]
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [Space(5)]
    [SerializeField, Range(0, 1f)] private float inertia;
    [SerializeField] private float friction;
    [Header("Stamina")]
    [SerializeField] private float maxStamina;
    [SerializeField] private float staminaConsumption;
    [SerializeField] private float staminaRegeneration;
    [Header("Import")]
    [SerializeField] private Slider staminaSlider;

    private Rigidbody2D physicalBody => GetComponent<Rigidbody2D>();

    private Vector2 movementDirection, frictionAmount;
    private Vector2 targetSpeed, actualForce, delta;
    private float stamina;

    void Update() {
        HandleMovement();
    }

    private void HandleMovement() {
        movementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && movementDirection.sqrMagnitude >= minimalAcceptableVelocity;
        stamina = Mathf.Clamp(stamina + (isRunning ? -staminaConsumption * Time.deltaTime : staminaRegeneration * Time.deltaTime), 0, maxStamina);
        staminaSlider.value = stamina / maxStamina;
        targetSpeed = movementDirection * ((stamina > 0 && isRunning) ? runSpeed : walkSpeed);
        #region Acceleration&Deceleration
        delta = targetSpeed - physicalBody.velocity;
        Vector2 accelerationRate = new Vector2(
            Mathf.Abs(targetSpeed.x) >= minimalAcceptableVelocity ? acceleration : deceleration,
            Mathf.Abs(targetSpeed.y) >= minimalAcceptableVelocity ? acceleration : deceleration);
        actualForce = new Vector2(
            Mathf.Pow(Mathf.Abs(delta.x) * accelerationRate.x, inertia) * Mathf.Sign(delta.x),
            Mathf.Pow(Mathf.Abs(delta.y) * accelerationRate.y, inertia) * Mathf.Sign(delta.y));
        #endregion
        #region Friction
        float frictionX = 0, frictionY = 0;
        if (Mathf.Abs(movementDirection.x) <= minimalAcceptableVelocity) {
            frictionX = Mathf.Min(Mathf.Abs(physicalBody.velocity.x), Mathf.Abs(friction));
        }
        if (Mathf.Abs(movementDirection.y) <= minimalAcceptableVelocity) {
            frictionY = Mathf.Min(Mathf.Abs(physicalBody.velocity.y), Mathf.Abs(friction));
        }
        frictionAmount = new Vector2(frictionX, frictionY) * physicalBody.velocity.normalized;
        #endregion
    }

    private void FixedUpdate() {
        physicalBody.AddForce(actualForce, ForceMode2D.Force);
        physicalBody.AddForce(-frictionAmount, ForceMode2D.Impulse);
    }

    private void Start() {
        stamina = maxStamina;
    }
}
