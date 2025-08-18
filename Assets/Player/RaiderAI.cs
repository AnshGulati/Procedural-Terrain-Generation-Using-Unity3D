using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RaiderAI : MonoBehaviour
{
    [Header("Movement & Targets")]
    private Transform shelter;
    private List<Transform> standingPoints;
    private Transform targetPoint;

    [Header("Attack Settings")]
    public float meleeDist = 2.5f;
    public float attackAnimDuration = 1.5f;
    public float attackDamageDelay = 0.5f;
    public int enemyDamage = 20;

    [Header("Damage Info")]
    public GameObject damageTextPrefab;
    public Transform damageTextPos;

    [Header("Hit Settings")]
    public int maxHits = 2;
    private int hitCount = 0;
    private bool isInvincible = false;

    private NavMeshAgent agent;
    private Animator anim;
    private SoundManager soundMan;
    private Coroutine attackCoroutine;
    private Shelter shelterScript;

    public enum STATE { MOVING, MELEEATTACK, HIT }
    public STATE currState = STATE.MOVING;
    private STATE prevState;

    public void Initialize(List<Transform> points)
    {
        standingPoints = points;
        GameObject shelterObject = GameObject.FindGameObjectWithTag("Shelter");
        if (shelterObject != null)
        {
            shelter = shelterObject.transform;
            shelterScript = shelterObject.GetComponent<Shelter>();

            if (standingPoints != null && standingPoints.Count > 0)
            {
                targetPoint = standingPoints[Random.Range(0, standingPoints.Count)];
                Debug.Log(gameObject.name + " set initial target to standing point.");
            }
            else
            {
                targetPoint = shelter;
                Debug.LogWarning("RaiderAI initialized with no standing points. Defaulting to shelter target.");
            }

            if (agent != null)
            {
                agent.isStopped = false;
                agent.SetDestination(targetPoint.position);
                anim.SetTrigger("isChasing");
            }
        }
        else
        {
            Debug.LogError("RaiderAI cannot find an object with the 'Shelter' tag. Make sure your shelter is tagged correctly!");
        }
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        soundMan = GetComponent<SoundManager>();
    }

    void Update()
    {
        if (agent == null || targetPoint == null) return;

        switch (currState)
        {
            case STATE.MOVING:
                if (!agent.pathPending && agent.remainingDistance <= meleeDist)
                {
                    if (targetPoint != shelter)
                    {
                        ChangeState(STATE.MELEEATTACK);
                    }
                    else
                    {
                        ChangeState(STATE.MELEEATTACK);
                    }
                }
                break;

            case STATE.MELEEATTACK:
                if (shelter != null) LookTarget(shelter.position, 5f);
                break;

            case STATE.HIT:
                break;
        }
    }

    public void ChangeState(STATE newState)
    {
        if (currState == STATE.MELEEATTACK && attackCoroutine != null)
            StopCoroutine(attackCoroutine);

        if (newState == STATE.HIT)
            prevState = currState;

        switch (newState)
        {
            case STATE.MOVING:
                if (agent != null)
                {
                    agent.isStopped = false;
                    if (targetPoint != null) agent.SetDestination(targetPoint.position);
                    anim.SetTrigger("isChasing");
                }
                break;

            case STATE.MELEEATTACK:
                if (agent != null) agent.isStopped = true;
                anim.SetTrigger("isMeleeAttacking");
                attackCoroutine = StartCoroutine(AttackCoroutine());
                break;

            case STATE.HIT:
                if (agent != null) agent.isStopped = true;
                anim.SetTrigger("isHited");
                break;
        }
        currState = newState;
    }

    private IEnumerator AttackCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackDamageDelay);

            if (shelterScript != null && shelterScript.currentHP > 0)
            {
                // Deal damage to the shelter
                shelterScript.TakeDamage(enemyDamage);
                Debug.Log(gameObject.name + " attacked the shelter for " + enemyDamage + " damage!");

                // Instantiate and set up the damage text pop-up
                if (damageTextPrefab != null && damageTextPos != null)
                {
                    GameObject dmgText = Instantiate(damageTextPrefab, damageTextPos.position, Quaternion.identity);
                    // Use the DmgInfo struct to pass damage details
                    DmgInfo dmgInfo = new DmgInfo(enemyDamage, Color.red, Vector3.zero);
                    dmgText.GetComponent<DamagePopup>().SetUp(dmgInfo.dmgValue, dmgInfo.textColor);
                }
            }

            if (shelterScript != null && shelterScript.currentHP <= 0)
            {
                yield break;
            }

            yield return new WaitForSeconds(attackAnimDuration - attackDamageDelay);
        }
    }

    public void ApplyDmg()
    {
        if (isInvincible) return;

        isInvincible = true;
        hitCount++;

        if (soundMan != null) soundMan.PlaySound("Hit");

        ChangeState(STATE.HIT);

        StartCoroutine(ResetInvincibility(0.5f));

        if (hitCount >= maxHits) Die();
    }

    private IEnumerator ResetInvincibility(float duration)
    {
        yield return new WaitForSeconds(duration);
        isInvincible = false;
        ChangeState(prevState);
    }

    private void Die()
    {
        foreach (Collider c in GetComponentsInChildren<Collider>()) c.enabled = false;
        if (agent != null) agent.isStopped = true;
        if (soundMan != null) soundMan.PlaySound("Death");

        Destroy(gameObject, 0.5f);
    }

    private void LookTarget(Vector3 targetPos, float speedRot)
    {
        Vector3 dir = targetPos - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * speedRot);
    }
}