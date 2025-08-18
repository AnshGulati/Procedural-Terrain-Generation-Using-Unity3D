using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Animator Controller for Popup")]
    public Animator popupAnim;

    private bool isOpen=false;

    // Toggle popup open/close
    public void TogglePopup()
    {
        isOpen = !isOpen;
        popupAnim.SetBool("isOpen", isOpen);
    }
}
