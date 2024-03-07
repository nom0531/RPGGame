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
    [SerializeField, Header("表示用データ")]
    GameObject Data_QuestName,Data_QuestDetail,Data_QuestLevel;
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

    private GameManager m_gameManager;

    private void Start()
    {
        // エネミーの生成データを削除する
        for(int i = 0; i < LevelData.levelDataList.Count; i++)
        {
            // 要素が0なら実行しない
            if (LevelData.levelDataList[i].enemyDataList.Count > 0)
            {
                // 削除
                LevelData.levelDataList[i].enemyDataList.RemoveRange(0, LevelData.levelDataList[i].enemyDataList.Count);
            }
        }

        m_gameManager = GameManager.Instance;
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
            if (m_gameManager.SaveDataManager.SaveData.saveData.ClearStage[QuestNumber] == true)
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
        //LevelData.levelDataList[number].enemyDataList.Clear();
        for (int dataNumber = 0; dataNumber < EnemyData.enemyDataList.Count; dataNumber++)
        {
            // 難易度の設定
            if (LevelData.levelDataList[number].LevelState < EnemyData.enemyDataList[dataNumber].LevelState)
            {
                continue;
            }
            // 当てはまっているならデータを追加する
            var enemyData = new EnemyData();
            enemyData.ID = EnemyData.enemyDataList[dataNumber].ID;
            LevelData.levelDataList[number].enemyDataList.Add(enemyData);
        }
    }

    /// <summary>
    /// 入力されたデータを表示する処理
    /// </summary>
    /// <param name="number">クエストの番号</param>
    public void DisplaySetSValue(int number)
    {
        m_gameManager.LevelNumber = number;
        QuestStartButton.GetComponent<Button>().interactable = true;
        DestroyEnemySprites();
        // 値を更新
        Data_QuestName.GetComponent<TextMeshProUGUI>().text = $"「{LevelData.levelDataList[number].LevelName}」";
        Data_QuestDetail.GetComponent<TextMeshProUGUI>().text = LevelData.levelDataList[number].LevelDetail;
        SetLevel(number);
        SetEnemySprites(number);
    }

    /// <summary>
    /// 難易度を設定する
    /// </summary>
    private void SetLevel(int number)
    {
        switch (LevelData.levelDataList[number].LevelState)
        {
            case LevelState.enOne:
                Data_QuestLevel.GetComponent<TextMeshProUGUI>().text = $"難易度 <sprite=4><sprite=5><sprite=5><sprite=5><sprite=5>";
                return;
            case LevelState.enTwo:
                Data_QuestLevel.GetComponent<TextMeshProUGUI>().text = $"難易度 <sprite=4><sprite=4><sprite=5><sprite=5><sprite=5>";
                return;
            case LevelState.enThree:
                Data_QuestLevel.GetComponent<TextMeshProUGUI>().text = $"難易度 <sprite=4><sprite=4><sprite=4><sprite=5><sprite=5>";
                return;
            case LevelState.enFour:
                Data_QuestLevel.GetComponent<TextMeshProUGUI>().text = $"難易度 <sprite=4><sprite=4><sprite=4><sprite=4><sprite=5>";
                return;
            case LevelState.enFive:
                Data_QuestLevel.GetComponent<TextMeshProUGUI>().text = $"難易度 <sprite=4><sprite=4><sprite=4><sprite=4><sprite=4>";
                return;
        }
    }

    /// <summary>
    /// エネミーの画像を生成する処理
    /// </summary>
    /// <param name="number">クエストの番号</param>
    private void SetEnemySprites(int number)
    {
        for (int enemyNumber = 0; enemyNumber < LevelData.levelDataList[number].enemyDataList.Count; enemyNumber++)
        {
            for (int dataNumber = 0; dataNumber < EnemyData.enemyDataList.Count; dataNumber++)
            {
                // エネミーの画像を生成
                if (LevelData.levelDataList[number].enemyDataList[enemyNumber].ID != EnemyData.enemyDataList[dataNumber].ID)
                {
                    // 番号が異なるなら次のループに移行する
                    continue;
                }
                var instrantiateNumber = LevelData.levelDataList[number].enemyDataList[enemyNumber].ID;
                var enemyObject = Instantiate(EnemyImage);
                enemyObject.transform.SetParent(EnemyContent.transform);
                enemyObject.GetComponent<Image>().sprite = EnemyData.enemyDataList[instrantiateNumber].EnemySprite;
                // サイズを調整
                enemyObject.transform.localScale = Vector3.one;
                enemyObject.transform.localPosition = Vector3.zero;

                if (m_gameManager.SaveDataManager.SaveData.saveData.EnemyRegisters[enemyNumber] != true)
                {
                    // 発見していないならカラーを変更する
                    enemyObject.GetComponent<Image>().color = Color.black;
                }
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
