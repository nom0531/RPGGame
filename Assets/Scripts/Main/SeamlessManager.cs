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
    /// �V�[������������
    /// </summary>
    /// <param name="number">��������V�[���ԍ�</param>
    /// <returns>�V�[����</returns>
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
    /// �V�[������������
    /// </summary>
    /// <param name="number">��������V�[���ԍ�</param>
    public void AddScene(SceneNumber number)
    {
        SceneManager.LoadScene(SetSceneName(number), LoadSceneMode.Additive);
    }

    /// <summary>
    /// �V�[�����폜����
    /// </summary>
    /// <param name="number">�폜����V�[���ԍ�</param>
    public void DestroyScene(SceneNumber number)
    {
        SceneManager.UnloadSceneAsync(SetSceneName(number));
    }
}