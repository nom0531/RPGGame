using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestStatusSystem : MonoBehaviour
{
    [SerializeField, Header("参照データ")]
    LevelDataBase LevelData;
    [SerializeField]
    EnemyDataBase EnemyData;
    [SerializeField, Header("表示用データ"),Tooltip("クエスト名")]
    GameObject Data_QuestName;
    [SerializeField,Tooltip("クエストの詳細")]
    GameObject Data_QuestDetail;
    [SerializeField, Header("クエストリスト"), Tooltip("生成するオブジェクト")]
    GameObject QuestContent;
    [SerializeField, Tooltip("生成するボタン")]
    GameObject QuestButton;
    [SerializeField, Header("エネミーリスト"), Tooltip("生成するオブジェクト")]
    GameObject EnemyContent;
    [SerializeField, Tooltip("生成する画像")]
    GameObject EnemyImage;
    [SerializeField, Header("参照オブジェクト")]
    GameObject QuestStartButton;

    private const int MAX_INSTANTIATE_SUM = 3;  // エネミーの画像の最大数

    private SaveDataManager m_saveDataManager;
    private int m_selectQuestNumber = 0;        // 現在選択しているクエストの番号

    /// <summary>
    /// 現在選択しているクエストの番号を取得する
    /// </summary>
    /// <returns>クエストの番号</returns>
    public int GetSelectQyestNumber()
    {
        return m_selectQuestNumber;
    }

    private void Start()
    {
        // エネミーの生成データを削除する
        for(int i = 0; i < LevelData.levelDataList.Count; i++)
        {
            LevelData.levelDataList[i].enemyDataList.RemoveRange(0, LevelData.levelDataList[i].enemyDataList.Count);
        }

        m_saveDataManager = GameManager.Instance.SaveData;

        Data_QuestName.SetActive(false);
        Data_QuestDetail.SetActive(false);

        QuestStartButton.GetComponent<Button>().interactable = false;

        for (int QuestNumber = 0; QuestNumber < LevelData.levelDataList.Count; QuestNumber++)
        {
            // ボタンを生成して子オブジェクトにする
            var questObject = Instantiate(QuestButton);
            questObject.transform.SetParent(QuestContent.transform);
            // サイズを調整
            questObject.transform.localScale = Vector3.one;
            questObject.transform.localPosition = Vector3.zero;

            // 既にクリアしているなら
            if (m_saveDataManager.SaveData.saveData.ClearStage[QuestNumber] == true)
            {
                // Clearのテキストを表示する
                questObject.transform.GetChild(1).gameObject.SetActive(true);
            }

            var questButton = questObject.GetComponent<QuestButton>();
            questButton.SetQuestStatus(
                QuestNumber,
                LevelData.levelDataList[QuestNumber].LevelName,
                Color.black,
                this
                );

            SetLevelStatus(QuestNumber);
        }
    }

    /// <summary>
    /// クエストデータの初期化
    /// </summary>
    /// <param name="number">クエストの番号</param>
    private void SetLevelStatus(int number)
    {
        for (int dataNumber = 0; dataNumber < EnemyData.enemyDataList.Count; dataNumber++)
        {
            switch (LevelData.levelDataList[number].LocationType)
            {
                case LocationType.enHell:
                case LocationType.enForest:
                case LocationType.enSea:
                case LocationType.enVolcano:

                    if (EnemyData.enemyDataList[dataNumber].PopLocation == LocationType.enAllLocation)
                    {
                        break;
                    }
                    else if (LevelData.levelDataList[number].LocationType != EnemyData.enemyDataList[dataNumber].PopLocation)
                    {
                        continue;
                    }

                    break;
            }

            switch (LevelData.levelDataList[number].LocationTime)
            {
                case LocationTime.enMorning:
                case LocationTime.enTwilight:
                case LocationTime.enEvening:

                    if (EnemyData.enemyDataList[dataNumber].PopTime == LocationTime.enAllTime)
                    {
                        break;
                    }
                    else if (LevelData.levelDataList[number].LocationTime != EnemyData.enemyDataList[dataNumber].PopTime)
                    {
                        continue;
                    }

                    break;
            }

            // データを追加する
            var enemyData = new EnemyData();
            enemyData.EnemyNumber = EnemyData.enemyDataList[dataNumber].EnemyNumber;
            LevelData.levelDataList[number].enemyDataList.Add(enemyData);
        }
    }

    /// <summary>
    /// 入力されたデータを表示する処理
    /// </summary>
    /// <param name="number">クエストの番号</param>
    public void DisplaySetSValue(int number)
    {
        m_selectQuestNumber = number;

        QuestStartButton.GetComponent<Button>().interactable = true;
        DestroyEnemySprites();

        Data_QuestName.SetActive(true);
        Data_QuestDetail.SetActive(true);

        // 値を更新
        Data_QuestName.GetComponent<TextMeshProUGUI>().text =
            $"「{LevelData.levelDataList[number].LevelName}」";
        Data_QuestDetail.GetComponent<TextMeshProUGUI>().text =
            LevelData.levelDataList[number].LevelDetail;

        SetEnemySprites(number);
    }

    /// <summary>
    /// エネミーの画像を生成する処理
    /// </summary>
    /// <param name="number">クエストの番号</param>
    private void SetEnemySprites(int number)
    {
        int InstantiateSum = 0; // オブジェクトの生成数

        for (int enemyNumber = 0; enemyNumber < LevelData.levelDataList[number].enemyDataList.Count; enemyNumber++)
        {
            for (int dataNumber = 0; dataNumber < EnemyData.enemyDataList.Count; dataNumber++)
            {
                if(InstantiateSum >= MAX_INSTANTIATE_SUM)
                {
                    // 生成数が最大数を超えているなら終了する
                    break;
                }

                // エネミーの画像を生成
                if (LevelData.levelDataList[number].enemyDataList[enemyNumber].EnemyNumber != EnemyData.enemyDataList[dataNumber].EnemyNumber)
                {
                    // 番号が異なるなら次のループに移行する
                    continue;
                }

                int instrantiateNumber = LevelData.levelDataList[number].enemyDataList[enemyNumber].EnemyNumber;

                var enemyObject = Instantiate(EnemyImage);
                enemyObject.transform.SetParent(EnemyContent.transform);
                enemyObject.GetComponent<Image>().sprite = EnemyData.enemyDataList[instrantiateNumber].EnemySprite;
                // サイズを調整
                enemyObject.transform.localScale = Vector3.one;
                enemyObject.transform.localPosition = Vector3.zero;

                if (m_saveDataManager.SaveData.saveData.EnemyRegisters[enemyNumber] != true)
                {
                    // 発見していないならカラーを変更する
                    enemyObject.GetComponent<Image>().color = Color.black;
                }

                InstantiateSum++;

                break;
            }
        }
    }

    /// <summary>
    /// PopEnemyタグの付いたオブジェクトを全て削除する処理
    /// </summary>
    private void DestroyEnemySprites()
    {
        var popEnemys = GameObject.FindGameObjectsWithTag("PopEnemy");

        foreach (var sprite in popEnemys)
        {
            Destroy(sprite);
        }
    }
}
