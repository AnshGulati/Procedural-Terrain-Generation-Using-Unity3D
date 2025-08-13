/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    private bool isInvincible = false;

    [Header("Detection Settings")]
    public float visDist = 20.0f;     // increased vision range
    public float visAngle = 120.0f;   // wider cone
    public float meleeDist = 2.5f;    // more forgiving melee distance

    public GameObject damageTextPrefab;
    public Transform damageTextPos;

    [Header("Hit Settings")]
    public int maxHits = 3;
    private int hitCount = 0;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        animEv = GetComponentInChildren<AnimatorEventsEn>();
        soundMan = GetComponent<SoundManager>();

        if (patrolPoints.Count != 0)
            ChangeState(STATE.PATROL);
    }

    void Update()
    {
        switch (currState)
        {
            case STATE.IDLE:
                if (CanSeePlayer())
                    ChangeState(STATE.CHASE);
                else if (Random.Range(0, 100) < 10)
                    ChangeState(STATE.PATROL);
                break;

            case STATE.PATROL:
                if (agent.remainingDistance < 1)
                {
                    curPatrolIndex = (curPatrolIndex + 1) % patrolPoints.Count;
                    agent.SetDestination(patrolPoints[curPatrolIndex].transform.position);
                }

                if (CanSeePlayer())
                    ChangeState(STATE.CHASE);
                break;

            case STATE.CHASE:
                agent.SetDestination(player.position);
                if (CanAttackPlayer())
                    ChangeState(STATE.MELEEATTACK);
                else if (CanStopChase())
                    ChangeState(STATE.PATROL);
                break;

            case STATE.MELEEATTACK:
                LookPlayer(5.0f);

                waitTimer += Time.deltaTime;
                if (waitTimer >= attackTime)
                {
                    // Loop back to CHASE if still in range
                    if (CanAttackPlayer())
                        waitTimer = 0; // attack again
                    else
                        ChangeState(STATE.CHASE);
                }
                break;

            case STATE.HIT:
                waitTimer += Time.deltaTime;
                if (waitTimer < 0.5f)
                    LookPlayer(5.0f);
                else if (waitTimer >= 0.5f && isInvincible)
                    isInvincible = false;
                else if (waitTimer >= 1.25f)
                {
                    if (CanAttackPlayer())
                        ChangeState(STATE.CHASE);
                    else
                        ChangeState(STATE.PATROL);
                }
                break;
        }
    }

    public bool CanSeePlayer()
    {
        Vector3 direction = player.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);

        if (direction.magnitude < visDist && angle < visAngle / 2f)
            return true;

        return false;
    }

    public bool CanAttackPlayer()
    {
        return Vector3.Distance(player.position, transform.position) < meleeDist;
    }

    public bool CanStopChase()
    {
        return Vector3.Distance(player.position, transform.position) > visDist * 1.2f;
    }

    public void ChangeState(STATE newState)
    {
        // Exit actions
        switch (currState)
        {
            case STATE.MELEEATTACK:
                animEv.isAttacking = false;
                anim.GetComponent<AnimatorEventsEn>().DisableWeaponColl();
                break;
        }

        // Enter actions
        switch (newState)
        {
            case STATE.PATROL:
                agent.speed = 2;
                agent.isStopped = false;
                anim.SetTrigger("isPatrolling");
                break;

            case STATE.CHASE:
                agent.speed = 3.5f;
                agent.isStopped = false;
                anim.SetTrigger("isChasing");
                break;

            case STATE.MELEEATTACK:
                agent.isStopped = true;
                waitTimer = 0;
                anim.SetTrigger("isMeleeAttacking");
                animEv.isAttacking = true;
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

                // Play enemy death sound
                soundMan.PlaySound("Death");

                Destroy(gameObject, 0.5f);
            }
        }
    }


    private void LookPlayer(float speedRot)
    {
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
*/

/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

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

    // Reference to the PlayerController script
    private PlayerController playerController;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        animEv = GetComponentInChildren<AnimatorEventsEn>();
        soundMan = GetComponent<SoundManager>();

        // Find the player's PlayerController script at the start
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }

        if (patrolPoints.Count != 0)
            ChangeState(STATE.PATROL);
    }

    void Update()
    {
        switch (currState)
        {
            case STATE.IDLE:
                if (CanSeePlayer())
                    ChangeState(STATE.CHASE);
                else if (Random.Range(0, 100) < 10)
                    ChangeState(STATE.PATROL);
                break;

            case STATE.PATROL:
                if (agent.remainingDistance < 1)
                {
                    curPatrolIndex = (curPatrolIndex + 1) % patrolPoints.Count;
                    agent.SetDestination(patrolPoints[curPatrolIndex].transform.position);
                }

                if (CanSeePlayer())
                    ChangeState(STATE.CHASE);
                break;

            case STATE.CHASE:
                agent.SetDestination(player.position);
                if (CanAttackPlayer())
                    ChangeState(STATE.MELEEATTACK);
                else if (CanStopChase())
                    ChangeState(STATE.PATROL);
                break;

            case STATE.MELEEATTACK:
                LookPlayer(5.0f);

                waitTimer += Time.deltaTime;
                if (waitTimer >= attackTime)
                {
                    if (CanAttackPlayer())
                        waitTimer = 0;
                    else
                        ChangeState(STATE.CHASE);
                }
                break;

            case STATE.HIT:
                waitTimer += Time.deltaTime;
                if (waitTimer < 0.5f)
                    LookPlayer(5.0f);
                else if (waitTimer >= 0.5f && isInvincible)
                    isInvincible = false;
                else if (waitTimer >= 1.25f)
                {
                    if (CanAttackPlayer())
                        ChangeState(STATE.CHASE);
                    else
                        ChangeState(STATE.PATROL);
                }
                break;
        }
    }

    public bool CanSeePlayer()
    {
        Vector3 direction = player.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);

        if (direction.magnitude < visDist && angle < visAngle / 2f)
            return true;

        return false;
    }

    public bool CanAttackPlayer()
    {
        return Vector3.Distance(player.position, transform.position) < meleeDist;
    }

    public bool CanStopChase()
    {
        return Vector3.Distance(player.position, transform.position) > visDist * 1.2f;
    }

    public void ChangeState(STATE newState)
    {
        switch (currState)
        {
            case STATE.MELEEATTACK:
                animEv.isAttacking = false;
                anim.GetComponent<AnimatorEventsEn>().DisableWeaponColl();
                break;
        }

        switch (newState)
        {
            case STATE.PATROL:
                agent.speed = 2;
                agent.isStopped = false;
                anim.SetTrigger("isPatrolling");
                break;

            case STATE.CHASE:
                agent.speed = 3.5f;
                agent.isStopped = false;
                anim.SetTrigger("isChasing");
                break;

            case STATE.MELEEATTACK:
                agent.isStopped = true;
                waitTimer = 0;
                anim.SetTrigger("isMeleeAttacking");
                animEv.isAttacking = true;
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

                // Call the player's method to register the kill
                if (playerController != null)
                {
                    playerController.EnemyKilled();
                }

                Destroy(gameObject, 0.5f);
            }
        }
    }


    private void LookPlayer(float speedRot)
    {
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
*/
/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

// Corrected DmgInfo struct with a constructor
public struct DmgInfo
{
    public int dmgValue;
    public Color textColor;
    public Vector3 dmgDir;

    // This constructor is required to fix the error
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

    [Header("Enemy Attack Settings")]
    public int enemyDamage = 5;

    private PlayerController playerController;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        animEv = GetComponentInChildren<AnimatorEventsEn>();
        soundMan = GetComponent<SoundManager>();

        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }

        if (patrolPoints.Count != 0)
            ChangeState(STATE.PATROL);
    }

    void Update()
    {
        switch (currState)
        {
            case STATE.IDLE:
                if (CanSeePlayer())
                    ChangeState(STATE.CHASE);
                else if (Random.Range(0, 100) < 10)
                    ChangeState(STATE.PATROL);
                break;

            case STATE.PATROL:
                if (agent.remainingDistance < 1)
                {
                    curPatrolIndex = (curPatrolIndex + 1) % patrolPoints.Count;
                    agent.SetDestination(patrolPoints[curPatrolIndex].transform.position);
                }

                if (CanSeePlayer())
                    ChangeState(STATE.CHASE);
                break;

            case STATE.CHASE:
                agent.SetDestination(player.position);
                if (CanAttackPlayer())
                    ChangeState(STATE.MELEEATTACK);
                else if (CanStopChase())
                    ChangeState(STATE.PATROL);
                break;

            case STATE.MELEEATTACK:
                LookPlayer(5.0f);
                if (!animEv.isAttacking)
                {
                    ChangeState(STATE.CHASE);
                }
                break;

            case STATE.HIT:
                waitTimer += Time.deltaTime;
                if (waitTimer < 0.5f)
                    LookPlayer(5.0f);
                else if (waitTimer >= 0.5f && isInvincible)
                    isInvincible = false;
                else if (waitTimer >= 1.25f)
                {
                    if (CanAttackPlayer())
                        ChangeState(STATE.CHASE);
                    else
                        ChangeState(STATE.PATROL);
                }
                break;
        }
    }

    public bool CanSeePlayer()
    {
        Vector3 direction = player.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);

        if (direction.magnitude < visDist && angle < visAngle / 2f)
            return true;

        return false;
    }

    public bool CanAttackPlayer()
    {
        return Vector3.Distance(player.position, transform.position) < meleeDist;
    }

    public bool CanStopChase()
    {
        return Vector3.Distance(player.position, transform.position) > visDist * 1.2f;
    }

    public void ChangeState(STATE newState)
    {
        switch (currState)
        {
            case STATE.MELEEATTACK:
                animEv.isAttacking = false;
                anim.GetComponent<AnimatorEventsEn>().DisableWeaponColl();
                break;
        }

        switch (newState)
        {
            case STATE.PATROL:
                agent.speed = 2;
                agent.isStopped = false;
                anim.SetTrigger("isPatrolling");
                break;

            case STATE.CHASE:
                agent.speed = 3.5f;
                agent.isStopped = false;
                anim.SetTrigger("isChasing");
                break;

            case STATE.MELEEATTACK:
                agent.isStopped = true;
                waitTimer = 0;
                anim.SetTrigger("isMeleeAttacking");
                animEv.isAttacking = true;
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

                Destroy(gameObject, 0.5f);
            }
        }
    }

    public void DealDamageToPlayer()
    {
        if (CanAttackPlayer())
        {
            playerController.TakeDamage(enemyDamage);
        }
    }

    public void AttackAnimDone()
    {
        animEv.isAttacking = false;
    }

    private void LookPlayer(float speedRot)
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * speedRot
            );
    }
}*/


/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

// Corrected DmgInfo struct with a constructor
public struct DmgInfo
{
    public int dmgValue;
    public Color textColor;
    public Vector3 dmgDir;

    // This constructor is required to fix the error
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

    [Header("Enemy Attack Settings")]
    public int enemyDamage = 5;

    private PlayerController playerController;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        animEv = GetComponentInChildren<AnimatorEventsEn>();
        soundMan = GetComponent<SoundManager>();

        // Fix starts here: Find the player by its tag if not assigned
        if (player == null)
        {
            GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
            if (playerGameObject != null)
            {
                player = playerGameObject.transform;
                playerController = playerGameObject.GetComponent<PlayerController>();
            }
        }
        // Fix ends here

        if (patrolPoints.Count != 0)
            ChangeState(STATE.PATROL);
    }

    void Update()
    {
        // Add a null check to prevent the error if the player is not found
        if (player == null)
        {
            // Optional: you can change the state to idle or simply return
            ChangeState(STATE.IDLE);
            return;
        }

        switch (currState)
        {
            case STATE.IDLE:
                if (CanSeePlayer())
                    ChangeState(STATE.CHASE);
                else if (Random.Range(0, 100) < 10)
                    ChangeState(STATE.PATROL);
                break;

            case STATE.PATROL:
                if (agent.remainingDistance < 1)
                {
                    curPatrolIndex = (curPatrolIndex + 1) % patrolPoints.Count;
                    agent.SetDestination(patrolPoints[curPatrolIndex].transform.position);
                }

                if (CanSeePlayer())
                    ChangeState(STATE.CHASE);
                break;

            case STATE.CHASE:
                agent.SetDestination(player.position);
                if (CanAttackPlayer())
                    ChangeState(STATE.MELEEATTACK);
                else if (CanStopChase())
                    ChangeState(STATE.PATROL);
                break;

            case STATE.MELEEATTACK:
                LookPlayer(5.0f);
                if (!animEv.isAttacking)
                {
                    ChangeState(STATE.CHASE);
                }
                break;

            case STATE.HIT:
                waitTimer += Time.deltaTime;
                if (waitTimer < 0.5f)
                    LookPlayer(5.0f);
                else if (waitTimer >= 0.5f && isInvincible)
                    isInvincible = false;
                else if (waitTimer >= 1.25f)
                {
                    if (CanAttackPlayer())
                        ChangeState(STATE.CHASE);
                    else
                        ChangeState(STATE.PATROL);
                }
                break;
        }
    }

    public bool CanSeePlayer()
    {
        // Add a null check here as well to be safe
        if (player == null) return false;

        Vector3 direction = player.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);

        if (direction.magnitude < visDist && angle < visAngle / 2f)
            return true;

        return false;
    }

    public bool CanAttackPlayer()
    {
        // Add a null check here as well to be safe
        if (player == null) return false;

        return Vector3.Distance(player.position, transform.position) < meleeDist;
    }

    public bool CanStopChase()
    {
        // Add a null check here as well to be safe
        if (player == null) return true;

        return Vector3.Distance(player.position, transform.position) > visDist * 1.2f;
    }

    public void ChangeState(STATE newState)
    {
        switch (currState)
        {
            case STATE.MELEEATTACK:
                animEv.isAttacking = false;
                anim.GetComponent<AnimatorEventsEn>().DisableWeaponColl();
                break;
        }

        switch (newState)
        {
            case STATE.PATROL:
                agent.speed = 2;
                agent.isStopped = false;
                anim.SetTrigger("isPatrolling");
                break;

            case STATE.CHASE:
                agent.speed = 3.5f;
                agent.stoppingDistance = meleeDist - 0.5f;
                agent.isStopped = false;
                anim.SetTrigger("isChasing");
                break;

            case STATE.MELEEATTACK:
                agent.isStopped = true;
                waitTimer = 0;
                anim.SetTrigger("isMeleeAttacking");
                animEv.isAttacking = true;
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

                Destroy(gameObject, 0.5f);
            }
        }
    }

    public void DealDamageToPlayer()
    {
        if (CanAttackPlayer())
        {
            playerController.TakeDamage(enemyDamage);
        }
    }

    public void AttackAnimDone()
    {
        animEv.isAttacking = false;
    }

    private void LookPlayer(float speedRot)
    {
        // Add a null check here as well to be safe
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
*/

//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;

//public struct DmgInfo
//{
//    public int dmgValue;
//    public Color textColor;
//    public Vector3 dmgDir;

//    public DmgInfo(int dmgv, Color tcolor, Vector3 dmgd)
//    {
//        dmgValue = dmgv;
//        textColor = tcolor;
//        dmgDir = dmgd;
//    }
//}

//public class AISimple : MonoBehaviour
//{
//    NavMeshAgent agent;
//    Animator anim;
//    SoundManager soundMan;
//    AnimatorEventsEn animEv;
//    public Transform player;

//    public enum STATE { IDLE, PATROL, CHASE, MELEEATTACK, HIT }
//    public STATE currState = STATE.IDLE;

//    public List<GameObject> patrolPoints = new List<GameObject>();
//    int curPatrolIndex = -1;

//    private float waitTimer = 0;
//    public float attackTime = 1.0f;

//    private bool isInvincible = false;

//    [Header("Detection Settings")]
//    public float visDist = 20.0f;
//    public float visAngle = 120.0f;
//    public float meleeDist = 2.5f;

//    public GameObject damageTextPrefab;
//    public Transform damageTextPos;

//    [Header("Hit Settings")]
//    public int maxHits = 3;
//    private int hitCount = 0;

//    [Header("Enemy Attack Settings")]
//    public int enemyDamage = 5;

//    private PlayerController playerController;

//    void Awake()
//    {
//        agent = GetComponent<NavMeshAgent>();
//        anim = GetComponentInChildren<Animator>();
//        animEv = GetComponentInChildren<AnimatorEventsEn>();
//        soundMan = GetComponent<SoundManager>();

//        // Always find player reference
//        if (player == null)
//        {
//            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
//            if (playerObj != null)
//            {
//                player = playerObj.transform;
//                playerController = playerObj.GetComponent<PlayerController>();
//            }
//        }

//        if (patrolPoints.Count != 0) ChangeState(STATE.PATROL);
//    }

//    void Update()
//    {
//        if (player == null) { ChangeState(STATE.IDLE); return; }

//        switch (currState)
//        {
//            case STATE.IDLE:
//                if (CanSeePlayer()) ChangeState(STATE.CHASE);
//                else if (Random.Range(0, 100) < 10) ChangeState(STATE.PATROL);
//                break;

//            case STATE.PATROL:
//                if (agent.remainingDistance < 1)
//                {
//                    curPatrolIndex = (curPatrolIndex + 1) % patrolPoints.Count;
//                    agent.SetDestination(patrolPoints[curPatrolIndex].transform.position);
//                }
//                if (CanSeePlayer()) ChangeState(STATE.CHASE);
//                break;

//            case STATE.CHASE:
//                agent.SetDestination(player.position);
//                if (CanAttackPlayer()) ChangeState(STATE.MELEEATTACK);
//                else if (CanStopChase()) ChangeState(STATE.PATROL);
//                break;

//            case STATE.MELEEATTACK:
//                LookPlayer(5.0f);
//                if (!animEv.isAttacking) ChangeState(STATE.CHASE);
//                break;

//            case STATE.HIT:
//                waitTimer += Time.deltaTime;
//                if (waitTimer < 0.5f) LookPlayer(5.0f);
//                else if (waitTimer >= 0.5f && isInvincible) isInvincible = false;
//                else if (waitTimer >= 1.25f)
//                {
//                    if (CanAttackPlayer()) ChangeState(STATE.CHASE);
//                    else ChangeState(STATE.PATROL);
//                }
//                break;
//        }
//    }

//    public bool CanSeePlayer()
//    {
//        if (player == null) return false;
//        Vector3 direction = player.position - transform.position;
//        float angle = Vector3.Angle(direction, transform.forward);
//        return (direction.magnitude < visDist && angle < visAngle / 2f);
//    }

//    public bool CanAttackPlayer()
//    {
//        if (player == null) return false;
//        return Vector3.Distance(player.position, transform.position) < meleeDist;
//    }

//    public bool CanStopChase()
//    {
//        if (player == null) return true;
//        return Vector3.Distance(player.position, transform.position) > visDist * 1.2f;
//    }

//    public void ChangeState(STATE newState)
//    {
//        if (currState == STATE.MELEEATTACK)
//        {
//            animEv.isAttacking = false;
//            anim.GetComponent<AnimatorEventsEn>().DisableWeaponColl();
//        }

//        switch (newState)
//        {
//            case STATE.PATROL:
//                agent.speed = 2;
//                agent.isStopped = false;
//                anim.SetTrigger("isPatrolling");
//                break;

//            case STATE.CHASE:
//                agent.speed = 3.5f;
//                agent.stoppingDistance = meleeDist - 0.5f; // ✅ stopping distance added
//                agent.isStopped = false;
//                anim.SetTrigger("isChasing");
//                break;

//            case STATE.MELEEATTACK:
//                agent.isStopped = true;
//                waitTimer = 0;
//                anim.SetTrigger("isMeleeAttacking");
//                animEv.isAttacking = true;
//                break;

//            case STATE.HIT:
//                agent.isStopped = true;
//                waitTimer = 0;
//                anim.SetTrigger("isHited");
//                break;

//            case STATE.IDLE:
//                anim.SetTrigger("isIdle");
//                break;
//        }
//        currState = newState;
//    }

//    public void ApplyDmg(DmgInfo dmgInfo)
//    {
//        if (!isInvincible)
//        {
//            isInvincible = true;
//            hitCount++;

//            ChangeState(STATE.HIT);
//            soundMan.PlaySound("Hit");

//            GameObject dmgText = Instantiate(damageTextPrefab, damageTextPos.position, Quaternion.identity);
//            dmgText.GetComponent<DamagePopup>().SetUp(
//                dmgInfo.dmgValue + Random.Range(-10, 10),
//                dmgInfo.textColor
//            );

//            if (hitCount >= maxHits)
//            {
//                foreach (Collider c in GetComponentsInChildren<Collider>())
//                    c.enabled = false;

//                if (agent != null) agent.isStopped = true;

//                soundMan.PlaySound("Death");

//                if (playerController != null)
//                {
//                    playerController.EnemyKilled();
//                }

//                Destroy(gameObject, 0.5f);
//            }
//        }
//    }

//    public void DealDamageToPlayer()
//    {
//        if (CanAttackPlayer() && playerController != null)
//        {
//            playerController.TakeDamage(enemyDamage);
//        }
//    }

//    public void AttackAnimDone() => animEv.isAttacking = false;

//    private void LookPlayer(float speedRot)
//    {
//        if (player == null) return;
//        Vector3 direction = player.position - transform.position;
//        direction.y = 0;
//        if (direction != Vector3.zero)
//            transform.rotation = Quaternion.Slerp(
//                transform.rotation,
//                Quaternion.LookRotation(direction),
//                Time.deltaTime * speedRot
//            );
//    }
//}


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    [Header("Enemy Attack Settings")]
    public int enemyDamage = 5;

    private PlayerController playerController;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        animEv = GetComponentInChildren<AnimatorEventsEn>();
        soundMan = GetComponent<SoundManager>();

        // Always find player reference
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
                if (!animEv.isAttacking) ChangeState(STATE.CHASE);
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
        return Vector3.Distance(player.position, transform.position) < meleeDist;
    }

    public bool CanStopChase()
    {
        if (player == null) return true;
        return Vector3.Distance(player.position, transform.position) > visDist * 1.2f;
    }

    public void ChangeState(STATE newState)
    {
        if (currState == STATE.MELEEATTACK)
        {
            animEv.isAttacking = false;
            anim.GetComponent<AnimatorEventsEn>().DisableWeaponColl();
        }

        switch (newState)
        {
            case STATE.PATROL:
                agent.speed = 2;
                agent.isStopped = false;
                anim.SetTrigger("isPatrolling");
                break;

            case STATE.CHASE:
                agent.speed = 3.5f;
                // ✅ Adjust stopping distance so the enemy gets within meleeDist to attack
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
                animEv.isAttacking = true;
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

                // ✅ The EnemyKilled() method call should be here, and it will ONLY update the score
                if (playerController != null)
                {
                    playerController.EnemyKilled();
                }

                Destroy(gameObject, 0.5f);
            }
        }
    }

    // ✅ This is the method that an animation event calls to hit the player.
    // The player takes damage here, not when the enemy dies.
    public void DealDamageToPlayer()
    {
        if (playerController != null)
        {
            // Only deal damage if the player is still within attack range.
            if (CanAttackPlayer())
            {
                playerController.TakeDamage(enemyDamage);
            }
        }
    }

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