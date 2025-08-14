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

//public struct dmginfo
//{
//    public int dmgvalue;
//    public color textcolor;
//    public vector3 dmgdir;

//    public dmginfo(int dmgv, color tcolor, vector3 dmgd)
//    {
//        dmgvalue = dmgv;
//        textcolor = tcolor;
//        dmgdir = dmgd;
//    }
//}

//public class AISimple : monobehaviour
//{
//    navmeshagent agent;
//    animator anim;
//    soundmanager soundman;
//    animatoreventsen animev;
//    public transform player;

//    public enum state { idle, patrol, chase, meleeattack, hit }
//    public state currstate = state.idle;

//    public list<gameobject> patrolpoints = new list<gameobject>();
//    int curpatrolindex = -1;

//    private float waittimer = 0;
//    public float attacktime = 1.0f;

//    private bool isinvincible = false;

//    [header("detection settings")]
//    public float visdist = 20.0f;
//    public float visangle = 120.0f;
//    public float meleedist = 2.5f;

//    public gameobject damagetextprefab;
//    public transform damagetextpos;

//    [header("hit settings")]
//    public int maxhits = 3;
//    private int hitcount = 0;

//    [header("enemy attack settings")]
//    public int enemydamage = 5;

//    private playercontroller playercontroller;

//    void awake()
//    {
//        agent = getcomponent<navmeshagent>();
//        anim = getcomponentinchildren<animator>();
//        animev = getcomponentinchildren<animatoreventsen>();
//        soundman = getcomponent<soundmanager>();

//        // always find player reference
//        if (player == null)
//        {
//            gameobject playerobj = gameobject.findgameobjectwithtag("player");
//            if (playerobj != null)
//            {
//                player = playerobj.transform;
//                playercontroller = playerobj.getcomponent<playercontroller>();
//            }
//        }

//        if (patrolpoints.count != 0) changestate(state.patrol);
//    }

//    void update()
//    {
//        if (player == null) { changestate(state.idle); return; }

//        switch (currstate)
//        {
//            case state.idle:
//                if (canseeplayer()) changestate(state.chase);
//                else if (random.range(0, 100) < 10) changestate(state.patrol);
//                break;

//            case state.patrol:
//                if (agent.remainingdistance < 1)
//                {
//                    curpatrolindex = (curpatrolindex + 1) % patrolpoints.count;
//                    agent.setdestination(patrolpoints[curpatrolindex].transform.position);
//                }
//                if (canseeplayer()) changestate(state.chase);
//                break;

//            case state.chase:
//                agent.setdestination(player.position);
//                if (canattackplayer()) changestate(state.meleeattack);
//                else if (canstopchase()) changestate(state.patrol);
//                break;

//            case state.meleeattack:
//                lookplayer(5.0f);
//                if (!animev.isattacking) changestate(state.chase);
//                break;

//            case state.hit:
//                waittimer += time.deltatime;
//                if (waittimer < 0.5f) lookplayer(5.0f);
//                else if (waittimer >= 0.5f && isinvincible) isinvincible = false;
//                else if (waittimer >= 1.25f)
//                {
//                    if (canattackplayer()) changestate(state.chase);
//                    else changestate(state.patrol);
//                }
//                break;
//        }
//    }

//    public bool canseeplayer()
//    {
//        if (player == null) return false;
//        vector3 direction = player.position - transform.position;
//        float angle = vector3.angle(direction, transform.forward);
//        return (direction.magnitude < visdist && angle < visangle / 2f);
//    }

//    public bool canattackplayer()
//    {
//        if (player == null) return false;
//        return vector3.distance(player.position, transform.position) < meleedist;
//    }

//    public bool canstopchase()
//    {
//        if (player == null) return true;
//        return vector3.distance(player.position, transform.position) > visdist * 1.2f;
//    }

//    public void changestate(state newstate)
//    {
//        if (currstate == state.meleeattack)
//        {
//            animev.isattacking = false;
//            anim.getcomponent<animatoreventsen>().disableweaponcoll();
//        }

//        switch (newstate)
//        {
//            case state.patrol:
//                agent.speed = 2;
//                agent.isstopped = false;
//                anim.settrigger("ispatrolling");
//                break;

//            case state.chase:
//                agent.speed = 3.5f;
//                agent.stoppingdistance = meleedist - 0.5f; // ✅ stopping distance added
//                agent.isstopped = false;
//                anim.settrigger("ischasing");
//                break;

//            case state.meleeattack:
//                agent.isstopped = true;
//                waittimer = 0;
//                anim.settrigger("ismeleeattacking");
//                animev.isattacking = true;
//                break;

//            case state.hit:
//                agent.isstopped = true;
//                waittimer = 0;
//                anim.settrigger("ishited");
//                break;

//            case state.idle:
//                anim.settrigger("isidle");
//                break;
//        }
//        currstate = newstate;
//    }

//    public void applydmg(dmginfo dmginfo)
//    {
//        if (!isinvincible)
//        {
//            isinvincible = true;
//            hitcount++;

//            changestate(state.hit);
//            soundman.playsound("hit");

//            gameobject dmgtext = instantiate(damagetextprefab, damagetextpos.position, quaternion.identity);
//            dmgtext.getcomponent<damagepopup>().setup(
//                dmginfo.dmgvalue + random.range(-10, 10),
//                dmginfo.textcolor
//            );

//            if (hitcount >= maxhits)
//            {
//                foreach (collider c in getcomponentsinchildren<collider>())
//                    c.enabled = false;

//                if (agent != null) agent.isstopped = true;

//                soundman.playsound("death");

//                if (playercontroller != null)
//                {
//                    playercontroller.enemykilled();
//                }

//                destroy(gameobject, 0.5f);
//            }
//        }
//    }

//    public void dealdamagetoplayer()
//    {
//        if (canattackplayer() && playercontroller != null)
//        {
//            playercontroller.takedamage(enemydamage);
//        }
//    }

//    public void attackanimdone() => animev.isattacking = false;

//    private void lookplayer(float speedrot)
//    {
//        if (player == null) return;
//        vector3 direction = player.position - transform.position;
//        direction.y = 0;
//        if (direction != vector3.zero)
//            transform.rotation = quaternion.slerp(
//                transform.rotation,
//                quaternion.lookrotation(direction),
//                time.deltatime * speedrot
//            );
//    }
//}

//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;

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
//    public float visDist = 20.0f;     // increased vision range
//    public float visAngle = 120.0f;   // wider cone
//    public float meleeDist = 2.5f;    // more forgiving melee distance

//    public GameObject damageTextPrefab;
//    public Transform damageTextPos;

//    [Header("Hit Settings")]
//    public int maxHits = 3;
//    private int hitCount = 0;

//    void Awake()
//    {
//        agent = GetComponent<NavMeshAgent>();
//        anim = GetComponentInChildren<Animator>();
//        animEv = GetComponentInChildren<AnimatorEventsEn>();
//        soundMan = GetComponent<SoundManager>();

//        if (patrolPoints.Count != 0)
//            ChangeState(STATE.PATROL);
//    }

//    void Update()
//    {
//        switch (currState)
//        {
//            case STATE.IDLE:
//                if (CanSeePlayer())
//                    ChangeState(STATE.CHASE);
//                else if (Random.Range(0, 100) < 10)
//                    ChangeState(STATE.PATROL);
//                break;

//            case STATE.PATROL:
//                if (agent.remainingDistance < 1)
//                {
//                    curPatrolIndex = (curPatrolIndex + 1) % patrolPoints.Count;
//                    agent.SetDestination(patrolPoints[curPatrolIndex].transform.position);
//                }

//                if (CanSeePlayer())
//                    ChangeState(STATE.CHASE);
//                break;

//            case STATE.CHASE:
//                agent.SetDestination(player.position);
//                if (CanAttackPlayer())
//                    ChangeState(STATE.MELEEATTACK);
//                else if (CanStopChase())
//                    ChangeState(STATE.PATROL);
//                break;

//            case STATE.MELEEATTACK:
//                LookPlayer(5.0f);

//                waitTimer += Time.deltaTime;
//                if (waitTimer >= attackTime)
//                {
//                    // Loop back to CHASE if still in range
//                    if (CanAttackPlayer())
//                        waitTimer = 0; // attack again
//                    else
//                        ChangeState(STATE.CHASE);
//                }
//                break;

//            case STATE.HIT:
//                waitTimer += Time.deltaTime;
//                if (waitTimer < 0.5f)
//                    LookPlayer(5.0f);
//                else if (waitTimer >= 0.5f && isInvincible)
//                    isInvincible = false;
//                else if (waitTimer >= 1.25f)
//                {
//                    if (CanAttackPlayer())
//                        ChangeState(STATE.CHASE);
//                    else
//                        ChangeState(STATE.PATROL);
//                }
//                break;
//        }
//    }

//    public bool CanSeePlayer()
//    {
//        Vector3 direction = player.position - transform.position;
//        float angle = Vector3.Angle(direction, transform.forward);

//        if (direction.magnitude < visDist && angle < visAngle / 2f)
//            return true;

//        return false;
//    }

//    public bool CanAttackPlayer()
//    {
//        return Vector3.Distance(player.position, transform.position) < meleeDist;
//    }

//    public bool CanStopChase()
//    {
//        return Vector3.Distance(player.position, transform.position) > visDist * 1.2f;
//    }

//    public void ChangeState(STATE newState)
//    {
//        // Exit actions
//        switch (currState)
//        {
//            case STATE.MELEEATTACK:
//                animEv.isAttacking = false;
//                anim.GetComponent<AnimatorEventsEn>().DisableWeaponColl();
//                break;
//        }

//        // Enter actions
//        switch (newState)
//        {
//            case STATE.PATROL:
//                agent.speed = 2;
//                agent.isStopped = false;
//                anim.SetTrigger("isPatrolling");
//                break;

//            case STATE.CHASE:
//                agent.speed = 3.5f;
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

//                // Play enemy death sound
//                soundMan.PlaySound("Death");

//                Destroy(gameObject, 0.5f);
//            }
//        }
//    }


//    private void LookPlayer(float speedRot)
//    {
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
//    public int enemyDamage = 2; // ✅ Change this value to 2

//    private PlayerController playerController;

//    void Awake()
//    {
//        agent = GetComponent<NavMeshAgent>();
//        anim = GetComponentInChildren<Animator>();
//        animEv = GetComponentInChildren<AnimatorEventsEn>();
//        soundMan = GetComponent<SoundManager>();

//        if (player == null)
//        {
//            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
//            if (playerObj != null)
//            {
//                player = playerObj.transform;
//                playerController = playerObj.GetComponent<PlayerController>();
//            }
//        }
//        else
//        {
//            playerController = player.GetComponent<PlayerController>();
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
//                if (agent != null)
//                {
//                    agent.stoppingDistance = meleeDist - 0.1f;
//                }
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

//    // ✅ This method is now responsible for dealing damage to the player.
//    // It will be called from an animation event on the enemy's attack animation.
//    public void DealDamageToPlayer()
//    {
//        if (playerController != null && player != null)
//        {
//            if (CanAttackPlayer())
//            {
//                playerController.TakeDamage(enemyDamage);
//            }
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
    public int enemyDamage = 2;

    // ✅ Delay before the damage is applied in the attack animation
    public float attackDamageDelay = 0.5f;

    // ✅ The total length of your attack animation clip
    public float attackAnimDuration = 1.5f;

    private PlayerController playerController;

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
        DealDamageToPlayer();

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
                agent.speed = 2;
                agent.isStopped = false;
                anim.SetTrigger("isPatrolling");
                break;

            case STATE.CHASE:
                agent.speed = 3.5f;
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
            if (CanAttackPlayer())
            {
                playerController.TakeDamage(enemyDamage);
            }
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