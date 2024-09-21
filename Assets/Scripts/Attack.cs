using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    // References to GameObjects
    public Rigidbody rightArm; 
    public Transform character;
    public Movement movement;
    public GameObject projectilePrefab;
    public Transform firePoint;
    private Vector3 originalArmPosition;
    private Vector3 attackPosition;

    // Variables that determine the attack aesthetic
    public float attackForce = 10f;
    public float returnSpeed = 5f; 
    public float maxArmHeight = 2f; 
    public float maxArmForwardDistance = 1f;
    public float fireCooldown = 0.3f;

    // Variables to handle attack logic
    private bool isAttacking = false;
    private bool isReturning = false;

    // Coroutine for handling holding attack
    private Coroutine fireCoroutine;

    // Start function: stores the attacking arms original position
    void Start()
    {
        originalArmPosition = rightArm.transform.position;
    }

    // Update function: Handles the players inputs and arm movement
    void Update()
    {
        // Handles left click down: start attacking and calls calculate attack position function
        if (Input.GetMouseButtonDown(0))
        {
            isAttacking = true;
            CalculateAttackPosition();

            if (fireCoroutine == null)
            {
                fireCoroutine = StartCoroutine(FireContinuously());
            }
        }

        // Handles left click release: stop attacking and start returning arm to original position
        if (Input.GetMouseButtonUp(0))
        {
            isAttacking = false;
            isReturning = true;

            if (fireCoroutine != null)
            {
                StopCoroutine(fireCoroutine);
                fireCoroutine = null;
            }
        }

        // Handle the arm movement when player is and is not attacking
        if (isAttacking)
        {
            HandleAttack();
        }
        else if (isReturning)
        {
            ReturnToOriginalPosition();
        }
    }

    // CalculateAttackPosition function: Calculate the forward direction based on the character's orientation, ignoring vertical component sets target attack position
    void CalculateAttackPosition()
    {
        Vector3 forwardDirection = character.forward;
        forwardDirection.y = 0f; 
        attackPosition = originalArmPosition + (Vector3.up * maxArmHeight) + (forwardDirection.normalized * maxArmForwardDistance);
    }

    // HandleAttack function: Apply upward and forward forces to the arm
    void HandleAttack()
    {
        Vector3 attackDirection = (Vector3.up * attackForce) + (character.forward.normalized * attackForce);
        rightArm.AddForce(attackDirection, ForceMode.Impulse);
    }

    // ReturnToOriginalPosition function: Smoothly returns the arm to the original position using lerp, with a threshold of .01f
    void ReturnToOriginalPosition()
    {
        rightArm.MovePosition(Vector3.Lerp(rightArm.position, originalArmPosition, Time.deltaTime * returnSpeed));

        if (Vector3.Distance(rightArm.position, originalArmPosition) < 0.01f)
        {
            isReturning = false;
        }
    }

    // Coroutine for firing continuously using the fire cooldown between projectiles
    IEnumerator FireContinuously()
    {
        while (true)
        {
            FireProjectile();
            yield return new WaitForSeconds(fireCooldown);
        }
    }

    // FireProjectile function: Instantiates a projectile at the firepoint position and sets its direction
    void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectileScript = projectile.GetComponent<Projectile>();

        if (movement.isRight) 
        {
            projectileScript.isRight = true;
        }
        else
        {
            projectileScript.isRight = false;
        }
    }
}
