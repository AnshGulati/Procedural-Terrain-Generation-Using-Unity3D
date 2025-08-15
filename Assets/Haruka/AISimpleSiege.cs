using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

// ✅ DmgInfo struct is added to be consistent with AISimple
public struct DmgInfoSiege
{
    public int dmgValue;
    public Color textColor;
    public Vector3 dmgDir;

    public DmgInfoSiege(int dmgv, Color tcolor, Vector3 dmgd)
    {
        dmgValue = dmgv;
        textColor = tcolor;
        dmgDir = dmgd;
    }
}

public class AISimpleSiege : MonoBehaviour
{
    NavMeshAgent agent;
    Animator anim;
    SoundManager soundMan;

    public Transform baseTarget;

    public enum STATE { RUN, ATTACK }
    public STATE currState = STATE.RUN;

    private Coroutine attackCoroutine;

    [Header("Movement Settings")]
    public float runSpeed = 3.5f;

    [Header("Detection Settings")]
    public float meleeDist = 4.0f;

    // ✅ Hit and Damage Settings from AISimple
    [Header("Hit Settings")]
    public int maxHits = 2; // ✅ Enemy will be destroyed after 2 hits
    private int hitCount = 0;
    private bool isInvincible = false;
    public GameObject damageTextPrefab;
    public Transform damageTextPos;

    [Header("Enemy Attack Settings")]
    public int enemyDamage = 10;
    public float attackDamageDelay = 0.5f;
    public float attackAnimDuration = 1.5f;

    private PlayerController playerController;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        soundMan = GetComponent<SoundManager>();

        // Find the player so we can call the EnemyKilled method
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerController = playerObj.GetComponent<PlayerController>();
        }

        if (baseTarget != null)
        {
            ChangeState(STATE.RUN);
        }
        else
        {
            Debug.LogError("AISimpleSiege: No base target assigned! Enemy will be idle.");
        }
    }

    void Update()
    {
        if (baseTarget == null)
        {
            agent.isStopped = true;
            return;
        }

        switch (currState)
        {
            case STATE.RUN:
                agent.SetDestination(baseTarget.position);
                if (Vector3.Distance(transform.position, baseTarget.position) < meleeDist)
                {
                    ChangeState(STATE.ATTACK);
                }
                break;

            case STATE.ATTACK:
                LookAtTarget(baseTarget);
                break;
        }
    }

    public void ChangeState(STATE newState)
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        switch (newState)
        {
            case STATE.RUN:
                agent.isStopped = false;
                agent.speed = runSpeed;
                anim.SetBool("IsRunning", true);
                break;

            case STATE.ATTACK:
                agent.isStopped = true;
                anim.SetBool("IsRunning", false);
                anim.SetTrigger("Attack");
                attackCoroutine = StartCoroutine(AttackCoroutine());
                break;
        }
        currState = newState;
    }

    private IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(attackDamageDelay);

        if (baseTarget != null)
        {
            Shelter baseScript = baseTarget.GetComponent<Shelter>();
            if (baseScript != null)
            {
                baseScript.TakeDamage(enemyDamage);
            }
        }

        yield return new WaitForSeconds(attackAnimDuration - attackDamageDelay);

        ChangeState(STATE.RUN);
    }

    // ✅ Method to apply damage, copied from AISimple
    public void ApplyDmg(DmgInfo dmgInfo)
    {
        if (!isInvincible)
        {
            isInvincible = true;
            hitCount++;

            // Spawn damage text popup
            GameObject dmgText = Instantiate(damageTextPrefab, damageTextPos.position, Quaternion.identity);
            dmgText.GetComponent<DamagePopup>().SetUp(
                dmgInfo.dmgValue + Random.Range(-10, 10),
                dmgInfo.textColor
            );

            if (hitCount >= maxHits)
            {
                // Disable all colliders on death
                foreach (Collider c in GetComponentsInChildren<Collider>())
                    c.enabled = false;

                if (agent != null) agent.isStopped = true;

                // Play death sound and inform the player
                if (playerController != null)
                {
                    playerController.EnemyKilled();
                }

                Destroy(gameObject, 0.5f);
            }
        }
    }

    private void LookAtTarget(Transform target)
    {
        if (target == null) return;
        Vector3 direction = target.position - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * 5f
            );
    }
}