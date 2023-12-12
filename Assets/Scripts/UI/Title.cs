using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    [SerializeField, Header("参照オブジェクト"),Tooltip("フェードを行うキャンバス")]
    private GameObject FadeCanvas;
    [SerializeField, Tooltip("遷移先のシーン名")]
    private string SceneName;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            string sceneName = SceneName;

            // 空白の場合は現在のシーンの名前を使用する
            if (SceneName == "")
            {
                sceneName = SceneManager.GetActiveScene().name;
            }

            // フェードを開始する
            var fadeCanvas = Instantiate(FadeCanvas);
            fadeCanvas.GetComponent<FadeScene>().FadeStart(sceneName);
        }
    }
}
