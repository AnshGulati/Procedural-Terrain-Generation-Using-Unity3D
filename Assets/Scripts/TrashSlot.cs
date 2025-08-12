using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrashSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject trashAlertUI;

    private Text textToModify;
    public Sprite trash_closed;
    public Sprite trash_opened;

    private Image imageComponent;
    Button YesBTN, NoBTN;

    GameObject draggedItem
    {
        get { return DragDrop.itemBeingDragged; }
    }

    GameObject itemToBeDeleted;

    public string itemName
    {
        get
        {
            string name = itemToBeDeleted.name;
            return name.Replace("(Clone)", "");
        }
    }

    void Start()
    {
        Transform backgroundTransform = transform.Find("background");
        if (backgroundTransform != null)
        {
            imageComponent = backgroundTransform.GetComponent<Image>();
        }
        else
        {
            Debug.LogError("Background child not found under " + gameObject.name);
        }

        if (trashAlertUI != null)
        {
            Transform textTransform = trashAlertUI.transform.Find("Text");
            if (textTransform != null)
            {
                textToModify = textTransform.GetComponent<Text>();
            }
            else
            {
                Debug.LogError("Text child not found under " + trashAlertUI.name);
            }

            Transform yesTransform = trashAlertUI.transform.Find("yes");
            if (yesTransform != null)
            {
                YesBTN = yesTransform.GetComponent<Button>();
                YesBTN.onClick.AddListener(delegate { DeleteItem(); });
            }
            else
            {
                Debug.LogError("Yes button child not found under " + trashAlertUI.name);
            }

            Transform noTransform = trashAlertUI.transform.Find("no");
            if (noTransform != null)
            {
                NoBTN = noTransform.GetComponent<Button>();
                NoBTN.onClick.AddListener(delegate { CancelDeletion(); });
            }
            else
            {
                Debug.LogError("No button child not found under " + trashAlertUI.name);
            }
        }
        else
        {
            Debug.LogError("trashAlertUI is not assigned in the Inspector for " + gameObject.name);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (draggedItem != null && draggedItem.GetComponent<InventoryItem>().isTrashable)
        {
            itemToBeDeleted = draggedItem.gameObject;
            StartCoroutine(notifyBeforeDeletion());
        }
    }

    IEnumerator notifyBeforeDeletion()
    {
        trashAlertUI.SetActive(true);
        textToModify.text = "Throw away this " + itemName + "?";
        imageComponent.sprite = trash_opened;
        yield return new WaitForSeconds(1f);
    }

    private void CancelDeletion()
    {
        imageComponent.sprite = trash_closed;
        trashAlertUI.SetActive(false);
    }

    private void DeleteItem()
    {
        imageComponent.sprite = trash_closed;
        DestroyImmediate(itemToBeDeleted.gameObject);
        InventorySystem.Instance.ReCalculateList();
        CraftingSystem.Instance.RefreshNeededItems();
        trashAlertUI.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (draggedItem != null && draggedItem.GetComponent<InventoryItem>().isTrashable)
        {
            imageComponent.sprite = trash_opened;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (draggedItem != null && draggedItem.GetComponent<InventoryItem>().isTrashable)
        {
            imageComponent.sprite = trash_closed;
        }
    }
}