/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RaiderAI : MonoBehaviour
{
    [Header("Movement & Targets")]
    public Transform shelter;
    public List<Transform> standingPoints;
    private Transform targetPoint;

    [Header("Attack Settings")]
    public float meleeDist = 2.5f;
    public float attackAnimDuration = 1.5f;
    public float attackDamageDelay = 0.5f;
    public int enemyDamage = 20;

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

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        soundMan = GetComponent<SoundManager>();

        if (shelter != null)
        {
            targetPoint = shelter;
            shelterScript = shelter.GetComponent<Shelter>();
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
                if (!agent.pathPending && agent.remainingDistance <= meleeDist)
                {
                    if (targetPoint == shelter && standingPoints != null && standingPoints.Count > 0)
                    {
                        targetPoint = standingPoints[Random.Range(0, standingPoints.Count)];
                        agent.SetDestination(targetPoint.position);
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

            if (shelterScript != null && shelterScript.currentHP > 0)
            {
                shelterScript.TakeDamage(enemyDamage);
                Debug.Log(gameObject.name + " attacked the shelter for " + enemyDamage + " damage!");
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
}*/

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RaiderAI : MonoBehaviour
{
    [Header("Movement & Targets")]
    // Public transform no longer needed, we find it by tag
    // public Transform shelter; 
    public List<Transform> standingPoints;
    private Transform targetPoint;

    [Header("Attack Settings")]
    public float meleeDist = 2.5f;
    public float attackAnimDuration = 1.5f;
    public float attackDamageDelay = 0.5f;
    public int enemyDamage = 20;

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

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        soundMan = GetComponent<SoundManager>();

        // Find the shelter using its tag
        GameObject shelterObject = GameObject.FindGameObjectWithTag("Shelter");

        if (shelterObject != null)
        {
            targetPoint = shelterObject.transform;
            shelterScript = shelterObject.GetComponent<Shelter>();
            agent.isStopped = false;
            agent.SetDestination(targetPoint.position);
            anim.SetTrigger("isChasing");
        }
        else
        {
            Debug.LogError("RaiderAI cannot find an object with the 'Shelter' tag. Make sure your shelter is tagged correctly!");
        }
    }

    void Update()
    {
        switch (currState)
        {
            case STATE.MOVING:
                if (targetPoint == null) return;
                if (!agent.pathPending && agent.remainingDistance <= meleeDist)
                {
                    if (targetPoint.CompareTag("Shelter") && standingPoints != null && standingPoints.Count > 0)
                    {
                        targetPoint = standingPoints[Random.Range(0, standingPoints.Count)];
                        agent.SetDestination(targetPoint.position);
                    }
                    else
                    {
                        ChangeState(STATE.MELEEATTACK);
                    }
                }
                break;

            case STATE.MELEEATTACK:
                if (targetPoint != null) LookTarget(targetPoint.position, 5f);
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

            if (shelterScript != null && shelterScript.currentHP > 0)
            {
                shelterScript.TakeDamage(enemyDamage);
                Debug.Log(gameObject.name + " attacked the shelter for " + enemyDamage + " damage!");
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
*/

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

    // This method is called by the RaidManager to set up the raider
    public void Initialize(List<Transform> points)
    {
        standingPoints = points;

        GameObject shelterObject = GameObject.FindGameObjectWithTag("Shelter");
        if (shelterObject != null)
        {
            shelter = shelterObject.transform;
            shelterScript = shelterObject.GetComponent<Shelter>();

            // **NEW LOGIC:** Set the initial target to a random standing point
            if (standingPoints != null && standingPoints.Count > 0)
            {
                targetPoint = standingPoints[Random.Range(0, standingPoints.Count)];
                Debug.Log(gameObject.name + " set initial target to standing point.");
            }
            else
            {
                // Fallback to the shelter itself if no standing points are assigned
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
                    // Check if we've reached our target (a standing point)
                    // We no longer need the shelter check here, as we only target standing points now
                    if (targetPoint != shelter)
                    {
                        ChangeState(STATE.MELEEATTACK);
                    }
                    else // This is the fallback for the warning case
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
                shelterScript.TakeDamage(enemyDamage);
                Debug.Log(gameObject.name + " attacked the shelter for " + enemyDamage + " damage!");
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
