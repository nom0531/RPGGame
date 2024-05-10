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

public class SeamlessManager : MonoBehaviour
{
    /// <summary>
    /// シーン名を教える
    /// </summary>
    /// <param name="number">合成するシーン番号</param>
    /// <returns>シーン名</returns>
    private string SetSceneName(SceneNumber number)
    {
        var addSceneName = "";
        switch (number)
        {
            case SceneNumber.enTitle:
                addSceneName = "OutGame_Title";
                break;
            case SceneNumber.enHome:
                addSceneName = "OutGame_Home";
                break;
            case SceneNumber.enPictureBook:
                addSceneName = "OutGame_PictureBook";
                break;
            case SceneNumber.enPlayerStatus:
                addSceneName = "OutGame_PlayerStatus";
                break;
            case SceneNumber.enQuest:
                addSceneName = "OutGame_Quest";
                break;
        }
        return addSceneName;
    }

    /// <summary>
    /// シーンを合成する
    /// </summary>
    /// <param name="number">合成するシーン番号</param>
    public void AddScene(SceneNumber number)
    {
        if (SceneManager.GetSceneByName(SetSceneName(number)).IsValid() == true)
        {
            return;
        }
        SceneManager.LoadSceneAsync(SetSceneName(number), LoadSceneMode.Additive);
    }

    /// <summary>
    /// シーンを削除する
    /// </summary>
    /// <param name="number">削除するシーン番号</param>
    public void DestroyScene(SceneNumber number)
    {
        if (SceneManager.GetSceneByName(SetSceneName(number)).IsValid() == false)
        {
            return;
        }
        SceneManager.UnloadSceneAsync(SetSceneName(number));
    }
}