using UnityEngine;

public class QuestManager
{
    #region Fields

    private int questIndex;

    // Quest DB
    private QuestDamageUp DamageUp = new();
    private QuestHPUp HPUp = new();
    private QuestDefeatEnemy DefeatEnemy = new();
    private QuestReachStage ReachStage = new();

    #endregion

    #region Properties

    public int QuestNum { get; private set; }
    public QuestData[] QuestDB { get; private set; }
    public QuestData CurrentQuest { get; private set; }

    #endregion

    #region Init

    public void InitQuest()
    {
        // 데이터 불러오기
        QuestNum = Manager.Data.Profile.Quest_Complete;
        QuestDB = new QuestData[4];

        LoadQuestdataBase();
    }

    #endregion

    #region Save Load Json

    public void LoadQuestdataBase()
    {
        DamageUp.Init(QuestNum, QuestDB.Length);
        HPUp.Init(QuestNum, QuestDB.Length);
        DefeatEnemy.Init(QuestNum, QuestDB.Length);
        ReachStage.Init(QuestNum, QuestDB.Length);

        QuestDB[0] = DamageUp;
        QuestDB[1] = HPUp;
        QuestDB[2] = DefeatEnemy;
        QuestDB[3] = ReachStage;

        questIndex = QuestNum % QuestDB.Length;
        CurrentQuest = QuestDB[questIndex];
    }

    #endregion


    // 퀘스트 클리어 여부 확인
    public void CheckQuestCompletion()
    {
        if (CurrentQuest.objectiveValue <= CurrentQuest.currentValue)
        {
            CurrentQuest.isClear = true;
            EarnQuestReward();
            NextQuest();
        }
        else
        {
            CurrentQuest.isClear = false;
        }
    }

    // 다음 퀘스트로 넘어가기
    public void NextQuest()
    {
        CurrentQuest.ObjectiveValueUp();
        CurrentQuest.isClear = false;
        
        QuestNum++;
        questIndex++;

        if (questIndex >= QuestDB.Length)
        {
            questIndex = 0;
            QuestDB[2].currentValue = 0; // 몬스터 사냥 횟수 초기화
        }

        CurrentQuest = QuestDB[questIndex];
    }

    // 퀘스트 값 증가 
    public void QuestCurrentValueUp()
    {
        CurrentQuest.currentValue++;
    }

    public void EarnQuestReward()
    {
        Manager.Game.Player.RewardGem(500);
    }
}

# region Quest DataBase

[System.Serializable]
public class QuestDataBase
{
    public int QuestNum;
}

[System.Serializable]
public class QuestData
{
    public QuestType questType;
    public string questObjective;
    public int ValueUpRate;
    public int objectiveValue;
    public int currentValue;
    public bool isClear;

    // 추상클래스에서 변경...
    public virtual void Init(int questLevel, int questCount) { }

    public void ObjectiveValueUp()
    {
        objectiveValue *= ValueUpRate;
    }
}
#endregion