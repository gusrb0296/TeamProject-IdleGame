using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIUpgradeStat : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textStatLevel;
    [SerializeField] private TextMeshProUGUI textStatValue;
    [SerializeField] private TextMeshProUGUI textUpdateCost;
    [SerializeField] private Button btnUpgradeStat;
    [SerializeField] private QuestType questType;
    private Player player;

    public void SetUpgradeStat(Player player, StatInfo statInfo, Action<PointerEventData> action)
    {
        this.player = player;

        textStatLevel.text = $"Lv. {statInfo.Level}";
        textStatValue.text = statInfo.GetString();
        textUpdateCost.text = statInfo.UpgradeCost.ToString();

        btnUpgradeStat.gameObject.SetEvent(UIEventType.Click, action);
        btnUpgradeStat.gameObject.SetEvent(UIEventType.PointerDown, PointerDown);
        btnUpgradeStat.gameObject.SetEvent(UIEventType.PointerUp, PointerUp);
    }

    public void UpdateUpgradeStat(StatInfo statInfo)
    {
        if (player.IsTradeGold(statInfo.UpgradeCost))
        {
            statInfo.AddModifier();

            UpdateQuestObjective();

            textStatLevel.text = $"Lv. {statInfo.Level}";
            textStatValue.text = statInfo.GetString();
            textUpdateCost.text = statInfo.UpgradeCost.ToString();
        }
    }

    private void UpdateQuestObjective()
    {
        if (questType == QuestType.DamageUp)
        {
            Manager.Quest.QuestDB[0].currentValue++;
            UISceneMain uiSceneMain = Manager.UI.CurrentScene as UISceneMain;
            uiSceneMain.UpdateQuestObjective();
        }
        else if(questType == QuestType.HPUp)
        {
            Manager.Quest.QuestDB[1].currentValue++;
            UISceneMain uiSceneMain = Manager.UI.CurrentScene as UISceneMain;
            uiSceneMain.UpdateQuestObjective();
        }
    }

    private void PointerDown(PointerEventData eventData)
    {
        player.CheckClick(true);
    }

    private void PointerUp(PointerEventData eventData)
    {
        player.CheckClick(false);
    }
}
