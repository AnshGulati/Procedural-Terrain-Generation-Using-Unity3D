using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public CharacterController charCont;
    [HideInInspector] public Animator anim;
    public GameObject childPlayer;
    public Camera cam;
    public GameObject movIndicator;

    [HideInInspector] public SoundManager soundMan;

    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    private bool canJump = true;
    private bool wasGrounded = false;

    private float mass = 60.0f;
    private Vector3 impact = Vector3.zero;

    public bool airControl = true;
    private float fallTime = 0f;

    public float maxDashTime = 0.5f;
    private float currentDashTime;
    public float dashSpeed = 20;
    private Vector3 dashDir;
    private bool canDash = true;

    private Vector3 moveDirection = Vector3.zero;
    private float distToGround;
    private Vector3 groundNormal;

    [HideInInspector] public bool canMove = true;
    private bool hit = false;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float stamina;
    public float staminaRegenRate = 10f;
    public float dashStaminaCost = 25f;
    public float jumpStaminaCost = 15f;
    public float staminaRegenDelay = 1.5f;
    private float staminaRegenTimer;

    [Header("UI Connection")]
    public Slider healthSlider;
    public Slider staminaSlider;
    public TextMeshProUGUI enemiesKilledText;
    public Color damageColor;
    public Image damageImage;
    bool isTakingDamage = false;
    float colorSmoothing=6f;

    [Header("Score Settings")]
    public static int enemiesKilled = 0;

    public Transform respawnPoint;
    public bool isPlayerDead;

    [Header("Damage Effect Settings")]
    public float damageFlashDuration = 1f;
    private float damageFlashTimer=0f;

    void Awake()
    {
        charCont = GetComponent<CharacterController>();
        soundMan = GetComponent<SoundManager>();
        anim = GetComponentInChildren<Animator>();
        currentDashTime = maxDashTime;
        distToGround = charCont.bounds.extents.y;

        stamina = maxStamina;
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = stamina;
        }

        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        UpdateKillCountUI();
    }

    void Update()
    {
        HandleStaminaRegeneration();
        UpdateStaminaUI();

        if (charCont.isGrounded)
        {
            if (!wasGrounded)
            {
                canJump = true;
                anim.SetBool("Jump", false);
                if (fallTime > 0.2f)
                {
                    soundMan.PlaySound("Land");
                    if (!hit) anim.CrossFade("FallingEnd", 0.1f);
                }
                fallTime = 0f;
            }
        }
        else
        {
            anim.SetFloat("SpeedY", charCont.velocity.y);
            if (wasGrounded && canJump)
            {
                if (DistToGround() > 0.3f)
                {
                    moveDirection.y = 0f;
                    wasGrounded = false;
                    anim.SetBool("Jump", true);
                    anim.CrossFade("Falling", 0.2f);
                }
            }
            if (charCont.velocity.y < 0) fallTime += Time.deltaTime;
        }
        wasGrounded = charCont.isGrounded;

        if (!canMove)
        {
            if (hit)
            {
                moveDirection.y -= gravity * Time.deltaTime;
                Vector3 impactGrav = new Vector3(impact.x, impact.y + moveDirection.y, impact.z);
                if (impact.magnitude > 0.2f || !charCont.isGrounded) charCont.Move(impactGrav * Time.deltaTime);
                impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);

                if (charCont.isGrounded && impact.magnitude <= 0.2f)
                {
                    hit = false;
                    canMove = true;
                    anim.Play("Idle");
                }
            }
            return;
        }

        if (charCont.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (moveDirection.magnitude < 0.1f) moveDirection = Vector3.zero;
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1);
            anim.SetFloat("Speed", moveDirection.magnitude);
            movIndicator.transform.localPosition = moveDirection;

            if (Input.GetButtonDown("Dash") && canDash && stamina >= dashStaminaCost)
            {
                UseStamina(dashStaminaCost);
                currentDashTime = 0;
                canDash = false;
                anim.Play("Slide");
                soundMan.PlaySound("Dash");
                dashDir = (moveDirection != Vector3.zero) ?
                    transform.TransformDirection(moveDirection).normalized :
                    childPlayer.transform.forward;
            }
            if (currentDashTime < maxDashTime)
            {
                dashDir.y = -10f;
                currentDashTime += Time.deltaTime;
                charCont.Move(dashDir * Time.deltaTime * dashSpeed);
                return;
            }
            canDash = true;

            if (moveDirection.magnitude > 0)
            {
                charCont.transform.rotation = new Quaternion(charCont.transform.rotation.x, cam.transform.rotation.y, charCont.transform.rotation.z, cam.transform.rotation.w);
                Vector3 targetActPosition = new Vector3(movIndicator.transform.position.x, childPlayer.transform.position.y, movIndicator.transform.position.z);
                Quaternion rotation = Quaternion.LookRotation(targetActPosition - childPlayer.transform.position);
                childPlayer.transform.rotation = Quaternion.Slerp(childPlayer.transform.rotation, rotation, Time.deltaTime * 10);
            }

            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            moveDirection.y = -10f;

            if (Input.GetButtonDown("Jump") && canJump && stamina >= jumpStaminaCost)
            {
                UseStamina(jumpStaminaCost);
                moveDirection.y = jumpSpeed;
                canJump = false;
                anim.SetFloat("SpeedY", moveDirection.y);
                anim.Play("Falling");
                anim.SetBool("Jump", true);
                soundMan.PlaySound("Jump");
            }
        }
        else
        {
            if (currentDashTime < maxDashTime)
            {
                currentDashTime = maxDashTime;
                canDash = true;
            }
            if (airControl)
            {
                Vector3 moveDirectionTemp = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                moveDirectionTemp = Vector3.ClampMagnitude(moveDirectionTemp, 1);
                moveDirection = new Vector3(moveDirectionTemp.x, moveDirection.y, moveDirectionTemp.z);

                movIndicator.transform.localPosition = new Vector3(moveDirection.x, 0, moveDirection.z);

                if (moveDirectionTemp.magnitude > 0)
                {
                    charCont.transform.rotation = new Quaternion(charCont.transform.rotation.x, cam.transform.rotation.y, charCont.transform.rotation.z, cam.transform.rotation.w);
                    Vector3 targetActPosition = new Vector3(movIndicator.transform.position.x, childPlayer.transform.position.y, movIndicator.transform.position.z);
                    Quaternion rotation = Quaternion.LookRotation(targetActPosition - childPlayer.transform.position);
                    childPlayer.transform.rotation = Quaternion.Slerp(childPlayer.transform.rotation, rotation, Time.deltaTime * 10);
                }
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection = new Vector3(moveDirection.x * speed * 0.8f, moveDirection.y, moveDirection.z * speed * 0.8f);
            }
        }

        if (damageFlashTimer>0)
        {
            damageImage.color = damageColor;
            damageFlashTimer-=Time.deltaTime;
        }
        else
        {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, colorSmoothing * Time.deltaTime);

        }

        moveDirection.y -= gravity * Time.deltaTime;
        charCont.Move(moveDirection * Time.deltaTime);
    }

    public void PlayerDead()
    {
        isPlayerDead = true;
        StartCoroutine(RespawnAfterDelay(3f));
    }

    private IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        RespawnPlayer();
    }

    public void RespawnPlayer()
    {
        isPlayerDead = false;
        if (respawnPoint != null)
        {
            transform.position=respawnPoint.position;
        }
        currentHealth = maxHealth;
        stamina = maxStamina;
        UpdateHealthUI();
        UpdateStaminaUI();
    }

    void UseStamina(float amount)
    {
        stamina = Mathf.Max(0, stamina - amount);
        staminaRegenTimer = staminaRegenDelay;
    }

    void HandleStaminaRegeneration()
    {
        if (stamina < maxStamina)
        {
            if (staminaRegenTimer > 0) staminaRegenTimer -= Time.deltaTime;
            else stamina = Mathf.Min(stamina + staminaRegenRate * Time.deltaTime, maxStamina);
        }
    }

    void UpdateStaminaUI()
    {
        if (staminaSlider != null) staminaSlider.value = stamina;
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        UpdateHealthUI();

        damageFlashTimer = damageFlashDuration;

        if (currentHealth <= 0)
        {
            isTakingDamage = true;
            PlayerDead();
            Debug.Log("Player has been defeated!");
        }
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null) healthSlider.value = currentHealth;
    }

    // ✅ This method now correctly reduces health by 5 and starts a coroutine
    public void EnemyKilled()
    {
        enemiesKilled++;
        UpdateKillCountUI();

        // Start the coroutine to increase health by 1 after a delay
        // StartCoroutine(DelayedHealthIncrease());
    }

    // ✅ New coroutine to handle the timed health increase
    private IEnumerator DelayedHealthIncrease()
    {
        // Wait for 1 second
        yield return new WaitForSeconds(1f);

        // Increase health by 1
        currentHealth = Mathf.Min(maxHealth, currentHealth + 0.3f);
        UpdateHealthUI();
    }

    private void UpdateKillCountUI()
    {
        if (enemiesKilledText != null)
        {
            enemiesKilledText.text = enemiesKilled.ToString();
        }
    }

    public void AddImpact(Vector3 dir, float force)
    {
        moveDirection = Vector3.zero;
        anim.Play("Hit");

        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y;
        impact += dir.normalized * force / mass;
    }

    public void ApplyDMG(Vector3 dir, float force)
    {
        if (!hit)
        {
            hit = true;
            canMove = false;
            soundMan.PlaySound("Hit");
            currentDashTime = maxDashTime;
            anim.SetFloat("Speed", 0);
            anim.GetComponent<AnimatorEvents>().DisableWeaponColl();
            AddImpact(dir, force);
        }
    }

    float DistToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, distToGround + 999))
            return hit.distance - distToGround;
        return 999;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    void OnControllerColliderHit(ControllerColliderHit hit) => groundNormal = hit.normal;

    public bool IsDashing() => currentDashTime < maxDashTime;

    public void EnableMove(bool camMoveT)
    {
        if (!hit) canMove = camMoveT;
    }
}