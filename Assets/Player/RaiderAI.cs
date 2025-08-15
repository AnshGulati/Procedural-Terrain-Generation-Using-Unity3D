/*using UnityEngine;

public class RaiderAI : MonoBehaviour
{
    public string shelterTag = "Shelter";
    public float moveSpeed = 3f;
    public float attackRange = 2f;
    public float rotationSpeed = 5f;

    private Animator anim;
    private Transform shelterTarget;
    private bool isAttacking = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        FindShelter();
    }

    void Update()
    {
        if (shelterTarget == null)
        {
            FindShelter();
            return;
        }

        float distance = Vector3.Distance(transform.position, shelterTarget.position);

        if (distance > attackRange)
        {
            // Rotate towards shelter
            Vector3 dir = (shelterTarget.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            // Move forward
            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            // Play run animation
            anim.SetBool("isRunning", true);
            isAttacking = false;
        }
        else
        {
            // Attack
            anim.SetBool("isRunning", false);
            if (!isAttacking)
            {
                anim.SetTrigger("attackTrigger");
                isAttacking = true;
            }
        }
    }

    void FindShelter()
    {
        GameObject shelterObj = GameObject.FindGameObjectWithTag(shelterTag);
        if (shelterObj != null)
        {
            shelterTarget = shelterObj.transform;
        }
    }
}*/

using UnityEngine;

public class RaiderAI : MonoBehaviour
{
    public string shelterTag = "Shelter";
    public float moveSpeed = 3f;
    public float attackRange = 2f;
    public float rotationSpeed = 5f;

    private Animator anim;
    private Transform shelterTarget;
    private bool isAttacking = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        FindShelter();
    }

    void Update()
    {
        if (shelterTarget == null)
        {
            FindShelter();
            return;
        }

        // Calculate the distance and direction to the shelter.
        Vector3 directionToShelter = shelterTarget.position - transform.position;
        float distance = directionToShelter.magnitude;

        // Check if the enemy is within attack range.
        if (distance <= attackRange)
        {
            // Stop running and start attacking.
            anim.SetBool("isRunning", false);
            if (!isAttacking)
            {
                anim.SetTrigger("attackTrigger");
                isAttacking = true;
            }
        }
        else
        {
            // Move towards the shelter.
            // Rotate towards the shelter.
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToShelter.x, 0, directionToShelter.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            // Move the enemy.
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            // Play the run animation.
            anim.SetBool("isRunning", true);
            isAttacking = false;
        }
    }

    void FindShelter()
    {
        GameObject shelterObj = GameObject.FindGameObjectWithTag(shelterTag);
        if (shelterObj != null)
        {
            shelterTarget = shelterObj.transform;
        }
    }
}