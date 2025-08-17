using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RaiderAI : MonoBehaviour
{
    [Header("Movement & Targets")]
    public Transform shelter;                // Reference to the shelter
    public List<Transform> standingPoints;   // Points near the shelter
    private Transform targetPoint;

    [Header("Attack Settings")]
    public float meleeDist = 2.5f;
    public float attackAnimDuration = 1.5f;
    public float attackDamageDelay = 0.5f;
    public int enemyDamage = 20;

    [Header("Hit Settings")]
    public int maxHits = 2; // Enemy Death Hit
    private int hitCount = 0;
    private bool isInvincible = false;

    private NavMeshAgent agent;
    private Animator anim;
    private SoundManager soundMan;
    private Coroutine attackCoroutine;

    public enum STATE { MOVING, MELEEATTACK, HIT }
    public STATE currState = STATE.MOVING;
    private STATE prevState;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        soundMan = GetComponent<SoundManager>();

        if (shelter != null)
        {
            targetPoint = shelter;
            agent.isStopped = false;
            agent.SetDestination(targetPoint.position);
            anim.SetTrigger("isChasing");
        }
    }

    void Update()
    {
        switch (currState)
        {
            case STATE.MOVING:
                if (targetPoint == null) return;

                // Wait until agent reaches current target
                if (!agent.pathPending && agent.remainingDistance <= meleeDist)
                {
                    // If target was shelter, now pick a random standing point
                    if (targetPoint == shelter && standingPoints != null && standingPoints.Count > 0)
                    {
                        targetPoint = standingPoints[Random.Range(0, standingPoints.Count)];
                        // Standing Points - targetPosition
                        agent.SetDestination(targetPoint.position);
                    }
                    // If target is already standing point, start attack
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
                //ApplyDmg();
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
                agent.isStopped = false;
                if (targetPoint != null) agent.SetDestination(targetPoint.position);
                anim.SetTrigger("isChasing");
                break;

            case STATE.MELEEATTACK:
                agent.isStopped = true;
                anim.SetTrigger("isMeleeAttacking");
                attackCoroutine = StartCoroutine(AttackCoroutine());
                break;

            case STATE.HIT:
                agent.isStopped = true;
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

            // Replace with your shelter damage logic
            Debug.Log(gameObject.name + " attacks the shelter!");

            yield return new WaitForSeconds(attackAnimDuration - attackDamageDelay);
        }
    }

    public void ApplyDmg()
    {
        if (isInvincible) return;

        isInvincible = true;
        hitCount++;
        // Moving -> Player Attack -> Enemy Attack
        //ChangeState(STATE.MELEEATTACK);

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
