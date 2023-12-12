using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCommandText : MonoBehaviour
{
    [SerializeField, Header("�Q�ƃf�[�^")]
    private SkillDataBase SkillData;

    /// <summary>
    /// �s���e�L�X�g�̕\������
    /// </summary>
    /// <param name="actionType">�s���p�^�[��</param>
    /// <param name="skillNumber">�g�p�X�L���̔ԍ�</param>
    public void SetCommandText(ActionType actionType, int skillNumber = 0)
    {
        string actionText = "";

        // �e�L�X�g�̕���
        switch (actionType)
        {
            case ActionType.enAttack:
                actionText = "�U��";
                break;
            case ActionType.enSkillAttack:
                actionText = SkillData.skillDataList[skillNumber].SkillName;
                break;
            case ActionType.enGuard:
                actionText = "�h��";
                break;
            case ActionType.enEscape:
                actionText = "����";
                break;
            case ActionType.enNull:
                actionText = "�l�q�����Ă���";
                break;
        }

        Debug.Log(actionText);
    }

    /// <summary>
    /// �o�t�����������Ƃ��̃e�L�X�g
    /// </summary>
    /// <param name="buffStatus">�o�t�̃^�C�v</param>
    public void SetStatusText(BuffStatus buffStatus)
    {
        string actionText = "";

        // ���Ƀo�t���������Ă���Ȃ��
        if (this.gameObject.GetComponent<BuffCalculation>().GetBuffFlag(buffStatus) == true)
        {
            actionText = "���ʎ��Ԃ��L�т�";
            Debug.Log(actionText);
            return;
        }

        switch (buffStatus)
        {
            case BuffStatus.enBuff_ATK:
                actionText = "�U���͂��オ����";
                break;
            case BuffStatus.enBuff_DEF:
                actionText = "�h��͂��オ����";
                break;
            case BuffStatus.enBuff_SPD:
                actionText = "�f�������オ����";
                break;
            case BuffStatus.enDeBuff_ATK:
                actionText = "�U���͂���������";
                break;
            case BuffStatus.enDeBuff_DEF:
                actionText = "�h��͂���������";
                break;
            case BuffStatus.enDeBuff_SPD:
                actionText = "�f��������������";
                break;
        }

        Debug.Log(actionText);
    }

    /// <summary>
    /// �o�t�̌��ʎ��Ԃ��I�������Ƃ��̃e�L�X�g
    /// </summary>
    public void ReSetStatusText()
    {
        string actionText = "���ʎ��Ԃ��I������";
        Debug.Log(actionText);
    }
}
