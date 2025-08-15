using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Collections; // Make sure this is included for coroutines

public struct DmgInfo
{
    public int dmgValue;
    public Color textColor;
    public Vector3 dmgDir;

    public DmgInfo(int dmgv, Color tcolor, Vector3 dmgd)
    {
        dmgValue = dmgv;
        textColor = tcolor;
        dmgDir = dmgd;
    }
}


public class AISimple : MonoBehaviour
{
    NavMeshAgent agent;
    Animator anim;
    SoundManager soundMan;
    AnimatorEventsEn animEv;
    public Transform player;

    public enum STATE { IDLE, PATROL, CHASE, MELEEATTACK, HIT }
    public STATE currState = STATE.IDLE;

    public List<GameObject> patrolPoints = new List<GameObject>();
    int curPatrolIndex = -1;

    private float waitTimer = 0;
    public float attackTime = 1.0f;

    // ✅ NEW: Coroutine reference to manage the attack sequence
    private Coroutine attackCoroutine;

    private bool isInvincible = false;

    [Header("Detection Settings")]
    public float visDist = 20.0f;
    public float visAngle = 120.0f;
    public float meleeDist = 2.5f;

    public GameObject damageTextPrefab;
    public Transform damageTextPos;

    [Header("Hit Settings")]
    public int maxHits = 3;
    private int hitCount = 0;

    [Header("Enemy Drops")]
    public List<GameObject> dropPrefabs; // Assign loot prefabs in Inspector
    public float dropChance = 1.0f; // 1.0 = 100% chance, 0.5 = 50% chance

    [Header("Enemy Attack Settings")]
    // ✅ Damage value is now set to 2
    public int enemyDamage = 20;

    // ✅ Delay before the damage is applied in the attack animation
    public float attackDamageDelay = 0.5f;

    // ✅ The total length of your attack animation clip
    public float attackAnimDuration = 1.5f;

    private PlayerController playerController;

    private void Start()
    {
        // Patrol Points Logic

        for (int i = 1; i < 5; i++)
        {
            patrolPoints[i-1] = GameObject.FindGameObjectWithTag("PatrolPoint" + i);
        }
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        animEv = GetComponentInChildren<AnimatorEventsEn>();
        soundMan = GetComponent<SoundManager>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerController = playerObj.GetComponent<PlayerController>();
            }
        }
        else
        {
            playerController = player.GetComponent<PlayerController>();
        }

        if (patrolPoints.Count != 0) ChangeState(STATE.PATROL);
    }

    void Update()
    {
        if (player == null) { ChangeState(STATE.IDLE); return; }

        switch (currState)
        {
            case STATE.IDLE:
                if (CanSeePlayer()) ChangeState(STATE.CHASE);
                else if (Random.Range(0, 100) < 10) ChangeState(STATE.PATROL);
                break;

            case STATE.PATROL:
                if (agent.remainingDistance < 1)
                {
                    curPatrolIndex = (curPatrolIndex + 1) % patrolPoints.Count;
                    agent.SetDestination(patrolPoints[curPatrolIndex].transform.position);
                }
                if (CanSeePlayer()) ChangeState(STATE.CHASE);
                break;

            case STATE.CHASE:
                agent.SetDestination(player.position);
                if (CanAttackPlayer()) ChangeState(STATE.MELEEATTACK);
                else if (CanStopChase()) ChangeState(STATE.PATROL);
                break;

            case STATE.MELEEATTACK:
                LookPlayer(5.0f);
                // No more logic here. The coroutine handles the attack loop.
                break;

            case STATE.HIT:
                waitTimer += Time.deltaTime;
                if (waitTimer < 0.5f) LookPlayer(5.0f);
                else if (waitTimer >= 0.5f && isInvincible) isInvincible = false;
                else if (waitTimer >= 1.25f)
                {
                    if (CanAttackPlayer()) ChangeState(STATE.CHASE);
                    else ChangeState(STATE.PATROL);
                }
                break;
        }
    }

    // ✅ NEW: Coroutine to handle the attack sequence
    private IEnumerator AttackCoroutine()
    {
        // Wait for the wind-up of the attack animation
        yield return new WaitForSeconds(attackDamageDelay);

        // Deal damage at the correct moment

        Debug.Log("Attack Initiate");
        DealDamageToPlayer();
        Debug.Log("Attack Done");

        // Wait for the rest of the animation to finish
        yield return new WaitForSeconds(attackAnimDuration - attackDamageDelay);

        // Transition back to the chase state after the animation ends
        ChangeState(STATE.CHASE);
    }

    public bool CanSeePlayer()
    {
        if (player == null) return false;
        Vector3 direction = player.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);
        return (direction.magnitude < visDist && angle < visAngle / 2f);
    }

    public bool CanAttackPlayer()
    {
        if (player == null) return false;
        // Check if the enemy is in the correct range and is not already attacking
        return Vector3.Distance(player.position, transform.position) < meleeDist && currState != STATE.MELEEATTACK;
    }

    public bool CanStopChase()
    {
        if (player == null) return true;
        return Vector3.Distance(player.position, transform.position) > visDist * 1.2f;
    }

    public void ChangeState(STATE newState)
    {
        // Stop the coroutine if the state is changing away from MELEEATTACK
        if (currState == STATE.MELEEATTACK && attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }

        switch (newState)
        {
            case STATE.PATROL:
                agent.speed = 3f;
                agent.isStopped = false;
                anim.SetTrigger("isPatrolling");
                break;

            case STATE.CHASE:
                agent.speed = 4f;
                if (agent != null)
                {
                    agent.stoppingDistance = meleeDist - 0.1f;
                }
                agent.isStopped = false;
                anim.SetTrigger("isChasing");
                break;

            case STATE.MELEEATTACK:
                agent.isStopped = true;
                waitTimer = 0;
                anim.SetTrigger("isMeleeAttacking");
                // ✅ Start the attack coroutine here
                attackCoroutine = StartCoroutine(AttackCoroutine());
                break;

            case STATE.HIT:
                agent.isStopped = true;
                waitTimer = 0;
                anim.SetTrigger("isHited");
                break;

            case STATE.IDLE:
                anim.SetTrigger("isIdle");
                break;
        }
        currState = newState;
    }

    public void ApplyDmg(DmgInfo dmgInfo)
    {
        if (!isInvincible)
        {
            isInvincible = true;
            hitCount++;

            ChangeState(STATE.HIT);
            soundMan.PlaySound("Hit");

            GameObject dmgText = Instantiate(damageTextPrefab, damageTextPos.position, Quaternion.identity);
            dmgText.GetComponent<DamagePopup>().SetUp(
                dmgInfo.dmgValue + Random.Range(-10, 10),
                dmgInfo.textColor
            );

            if (hitCount >= maxHits)
            {
                foreach (Collider c in GetComponentsInChildren<Collider>())
                    c.enabled = false;

                if (agent != null) agent.isStopped = true;

                soundMan.PlaySound("Death");

                if (playerController != null)
                {
                    playerController.EnemyKilled();
                }

                // Try to drop loot
                if (dropPrefabs != null && dropPrefabs.Count > 0 && Random.value <= dropChance)
                {
                    int randomIndex = Random.Range(0, dropPrefabs.Count);
                    Instantiate(dropPrefabs[randomIndex], transform.position, Quaternion.identity);
                }

                Destroy(gameObject, 0.5f);
            }
        }
    }

    // This method is now called by the coroutine.
    public void DealDamageToPlayer()
    {
        if (playerController != null && player != null)
        {
            // Only deal damage if the player is still within attack range.
            //if (CanAttackPlayer())
            //{
                
            //}

            Debug.Log("Attack True");
            playerController.TakeDamage(enemyDamage);



        }
    }

    // This method is not needed when using a coroutine for attack timing.
    public void AttackAnimDone() => animEv.isAttacking = false;

    private void LookPlayer(float speedRot)
    {
        if (player == null) return;
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * speedRot
            );
    }
}