using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActive : MonoBehaviour
{
    [SerializeField, Header("Active��ݒ肷��I�u�W�F�N�g")]
    private GameObject[] GameObjects;

    /// <summary>
    /// �I�u�W�F�N�g���\���ɂ���
    /// </summary>
    public void SetActive_False()
    {
        for(int i= 0; i < GameObjects.Length; i++)
        {
            GameObjects[i].SetActive(false);
        }
    }

    /// <summary>
    /// �I�u�W�F�N�g��\������
    /// </summary>
    public void SetActive_True()
    {
        for (int i = 0; i < GameObjects.Length; i++)
        {
            GameObjects[i].SetActive(true);
        }
    }
}
