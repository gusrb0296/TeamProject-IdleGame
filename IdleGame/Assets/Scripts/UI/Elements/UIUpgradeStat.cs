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

        textStatLevel.text = statInfo.Level.ToString();
        textStatValue.text = statInfo.GetString();
        textUpdateCost.text = statInfo.UpgradeCost.ToString();

        btnUpgradeStat.gameObject.SetEvent(UIEventType.Click, action);
        btnUpgradeStat.gameObject.SetEvent(UIEventType.PointerDown, PointerDown);
        btnUpgradeStat.gameObject.SetEvent(UIEventType.PointerUp, PointerUp);
    }

    public void UpdateUpgradeStat(StatInfo statInfo)
    {
        if (player.IsTrade(statInfo.UpgradeCost))
        {
            statInfo.AddModifier();
            statInfo.Level += 1;
            statInfo.UpgradeCost += 10;

            UpdateQuestObjective();

            textStatLevel.text = statInfo.Level.ToString();
            textStatValue.text = statInfo.GetString();
            textUpdateCost.text = statInfo.UpgradeCost.ToString();
        }
    }

    private void UpdateQuestObjective()
    {
        if (Manager.Quest.CuurntQuest.questType == questType)
        {
            Manager.Quest.QuestObjectiveValueUp();
            UISceneMain uiSceneMain = Manager.UI.CurrentScene as UISceneMain;
            uiSceneMain.UpdateQuestObjective();
        }
        else if (Manager.Quest.CuurntQuest.questType == questType)
        {
            Manager.Quest.QuestObjectiveValueUp();
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
