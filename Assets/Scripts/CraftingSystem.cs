using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    public GameObject craftingScreenUI;
    public GameObject toolsScreenUI;
    public List<string> inventoryItemList = new List<string>(); // ✅ Correct variable

    Button toolsBTN;
    Button craftAxeBTN;
    Text AxeReq1, AxeReq2;

    public bool isOpen;

    public Blueprint AxeBLP = new Blueprint("Axe", 2, "Stone", 3, "Stick", 3);
    public static CraftingSystem Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        isOpen = false;

        toolsBTN = craftingScreenUI.transform.Find("ToolsButton").GetComponent<Button>();
        toolsBTN.onClick.AddListener(delegate { OpenToolsCategory(); });

        AxeReq1 = toolsScreenUI.transform.Find("Axe").transform.Find("req1").GetComponent<Text>();
        AxeReq2 = toolsScreenUI.transform.Find("Axe").transform.Find("req2").GetComponent<Text>();

        craftAxeBTN = toolsScreenUI.transform.Find("Axe").transform.Find("Button").GetComponent<Button>();
        craftAxeBTN.onClick.AddListener(delegate { CraftAnyItem(AxeBLP); });
    }

    void OpenToolsCategory()
    {
        craftingScreenUI.SetActive(false);
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(true);
    }

    void CraftAnyItem(Blueprint bluePrintToCraft)
    {
        InventorySystem.Instance.AddToInventory(bluePrintToCraft.itemName);

        if (bluePrintToCraft.numOfRequirements == 1)
        {
            string Req1 = bluePrintToCraft.Req1;
            int Req1amount = bluePrintToCraft.Reg1amount;
            InventorySystem.Instance.RemoveItem(Req1, Req1amount);
        }
        else if (bluePrintToCraft.numOfRequirements == 2)
        {
            string Req1 = bluePrintToCraft.Req1;
            int Req1amount = bluePrintToCraft.Reg1amount;
            string Req2 = bluePrintToCraft.Req2;
            int Req2amount = bluePrintToCraft.Reg2amount;
            InventorySystem.Instance.RemoveItem(Req1, Req1amount);
            InventorySystem.Instance.RemoveItem(Req2, Req2amount);
        }

        StartCoroutine(calculate());

    }

    public IEnumerator calculate()
    {
        yield return 0;
        InventorySystem.Instance.ReCalculateList();
        RefreshNeededItems();
    }

    void Update()
    {
        RefreshNeededItems();

        if (Input.GetKeyDown(KeyCode.I) && !isOpen)
        {
            craftingScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            isOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.I) && isOpen)
        {
            craftingScreenUI.SetActive(false);
            toolsScreenUI.SetActive(false);

            if (!InventorySystem.Instance.isOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            isOpen = false;
        }
    }

    public void RefreshNeededItems()
    {
        int stone_count = 0;
        int stick_count = 0;

        // ✅ Fixed: correct reference to InventorySystem.ItemList
        inventoryItemList = InventorySystem.Instance.ItemList;

        foreach (string itemName in inventoryItemList)
        {
            switch (itemName)
            {
                case "Stone":
                    stone_count++;
                    break;
                case "Stick":
                    stick_count++;
                    break;
            }
        }

        AxeReq1.text = "3 Stone [" + stone_count + "]";
        AxeReq2.text = "3 Stick [" + stick_count + "]";

        craftAxeBTN.gameObject.SetActive(stone_count >= 3 && stick_count >= 3);
    }
}
