using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButton : MonoBehaviour
{
    [SerializeField, Header("参照オブジェクト"), Tooltip("フェードを開始するキャンバス")]
    private GameObject FadeCanvas;
    [SerializeField, Header("追加でロードするシーン")]
    SceneNumber SceneNumber;

    private bool m_push = false;

    /// <summary>
    /// シーンを遷移する
    /// </summary>
    /// <param name="sceneName">遷移先のシーン名</param>
    public void SceneChange(string sceneName)
    {
        if(m_push == true)
        {
            return;
        }
        m_push = true;
        // フェードを開始する
        var fadeCanvas = Instantiate(FadeCanvas);
        fadeCanvas.GetComponent<FadeScene>().FadeStart(sceneName);
        // セーブ
        var gameManager = GameManager.Instance;
        if(sceneName == "OutGame_Main")
        {
            gameManager.SceneNumber = SceneNumber;      // 追加でロードするシーンを設定
        }
        gameManager.SaveDataManager.Save(false);
    }
}
