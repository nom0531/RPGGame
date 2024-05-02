using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneNumber
{
    enTitle,
    enHome,
    enPictureBook,
    enPlayerStatus,
    enQuest,
}

public class SeamlessManager : SingletonMonoBehaviour<SeamlessManager>
{
    /// <summary>
    /// シーン名を教える
    /// </summary>
    /// <param name="number">合成するシーン番号</param>
    /// <returns>シーン名</returns>
    private string SetSceneName(SceneNumber number)
    {
        switch (number)
        {
            case SceneNumber.enTitle:
                return "OutGame_Title";
            case SceneNumber.enHome:
                return "OutGame_Home";
            case SceneNumber.enPictureBook:
                return "OutGame_PictureBook";
            case SceneNumber.enPlayerStatus:
                return "OutGame_PlayerStatus";
            case SceneNumber.enQuest:
                return "OutGame_Quest";
        }
        return "";
    }

    /// <summary>
    /// シーンを合成する
    /// </summary>
    /// <param name="number">合成するシーン番号</param>
    public void AddScene(SceneNumber number)
    {
        SceneManager.LoadScene(SetSceneName(number), LoadSceneMode.Additive);
    }

    /// <summary>
    /// シーンを削除する
    /// </summary>
    /// <param name="number">削除するシーン番号</param>
    public void DestroyScene(SceneNumber number)
    {
        SceneManager.UnloadSceneAsync(SetSceneName(number));
    }
}