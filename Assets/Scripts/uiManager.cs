using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Main Popup Animator")]
    [Tooltip("The parent object with the Animator that controls all resource popups.")]
    public Animator popupAnimator;

    public void ShowWoodPopup()
    {
        if (popupAnimator != null)
        {
            popupAnimator.SetTrigger("ShowWood");
        }
    }

    public void ShowCoinPopup()
    {
        if (popupAnimator != null)
        {
            popupAnimator.SetTrigger("ShowCoin");
        }
    }

    public void ShowStonePopup()
    {
        if (popupAnimator != null)
        {
            popupAnimator.SetTrigger("ShowStone");
        }
    }

    public void ShowBuildingUpgradePopup()
    {
        if (popupAnimator != null)
        {
            popupAnimator.SetTrigger("ShowUpgradeSuccess");
        }
    }

    public void ShowBuildingUpgradeFailPopup()
    {
        if (popupAnimator != null)
        {
            popupAnimator.SetTrigger("ShowUpgradeFail");
        }
    }

    public void ShowRaidPopup()
    {
        if (popupAnimator != null)
        {
            popupAnimator.SetTrigger("ShowRaid");
        }
    }

    public void ShowBuildingRepairPopup()
    {
        if (popupAnimator != null)
        {
            popupAnimator.SetTrigger("ShowRepair");
        }
    }
}