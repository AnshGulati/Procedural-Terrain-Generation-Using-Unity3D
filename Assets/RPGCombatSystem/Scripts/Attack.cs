using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private Animator anim;

    private PlayerController playerCont;

    void Awake()
    {
        playerCont = GetComponent<PlayerController>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (playerCont.canMove)
            {
                if (playerCont.charCont.isGrounded && !playerCont.IsDashing())
                {
                    anim.Play("Attack");
                    playerCont.soundMan.PlaySound("Attack");
                }
            }
        }
    }
}
