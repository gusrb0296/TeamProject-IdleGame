using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class UIPopupEquipContainer : UIBase
{
    #region Fields

    public List<GameObject> itemSlots = new List<GameObject>();
    public GameObject itemInfoUI;
    private ScrollRect scrollRect;

    [SerializeField] private UIPopupEquipment MainPopupUI;

    #endregion

    #region Unity Flow

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }
    private void Start()
    {
        MainPopupUI = Manager.UI.CurrentPopup as UIPopupEquipment;
        InitSlot();
        MainPopupUI.RefreshReinforecEvent += SetSlotEquipUI;
        MainPopupUI.RefreshReinforecEvent += SetSlotReinforceUI;
    }
    #endregion

    #region Initial Method

    private void ResetOnScrollTop()
    {
        scrollRect.verticalNormalizedPosition = 1;
    }

    //JSON에서 파싱한 아이템 개수 만큼 슬롯을 생성합니다.
    public void InitSlot()
    {
        foreach (GameObject item in itemSlots)
        {
            Destroy(item);
        }

        itemSlots.Clear();
        if (MainPopupUI.EquipFillterType == EquipFillterType.Weapon)
        {
            foreach (var itemData in Manager.Inventory.WeaponItemList)
            {
                UIPopupEquipSlots slot = Manager.Asset.InstantiatePrefab("ItemSlot_Weapon", gameObject.transform).GetComponent<UIPopupEquipSlots>();
                itemSlots.Add(slot.gameObject);
                slot.InitSlotInfo(itemData);
                slot.InitSlotUI();
                slot.CheckEquipState();
                slot.SetReinforceUI();
            }
        }
        else if (MainPopupUI.EquipFillterType == EquipFillterType.Armor)
        {
            foreach (var itemData in Manager.Inventory.ArmorItemList)
            {
                UIPopupEquipSlots slot = Manager.Asset.InstantiatePrefab("ItemSlot_Armor", gameObject.transform).GetComponent<UIPopupEquipSlots>();
                itemSlots.Add(slot.gameObject);
                slot.InitSlotInfo(itemData);
                slot.InitSlotUI();
                slot.CheckEquipState();
                slot.SetReinforceUI();
            }
        }
        ResetOnScrollTop();
    }

    //슬롯에 장착 여부를 춫력하기 위한 메서드
    public void SetSlotEquipUI()
    {
        foreach (var slot in itemSlots)
        {
            slot.GetComponent<UIPopupEquipSlots>().CheckEquipState();
        }
    }
    public void SetSlotReinforceUI()
    {
        foreach (var slot in itemSlots)
        {
            slot.GetComponent<UIPopupEquipSlots>().SetReinforceUI();
        }
    }

    #endregion
}
