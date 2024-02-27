using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    // Start is called before the first frame update
    public Node node = new Node();

    public int height;

    public bool isBlock;

    public bool isOnUnit = false;

    public bool ImpossibleUnitSetTile = false;

    /// <summary>
    /// �ش� Ŭ���� �̴Ͼ��� ��ġ�� �� �ִ� Ÿ������ Ȯ��
    /// </summary>
    /// <returns>true�Ͻ� ��ġ ����</returns>
    public bool IsDeployableMinionTile()
    {
        return (!isBlock && !isOnUnit && !ImpossibleUnitSetTile);
    }

    /// <summary>
    /// ��ġ������ Ÿ�� ������� ǥ��
    /// </summary>
    /// <param name="isActive"></param>
    /// <param name="minionClass"></param>
    public void ShowDeployableTile(bool isActive)
    {
        if (IsDeployableMinionTile())
            GetComponent<Image>().sprite = BattleUIManager.Instance.DeployableTileSprite;
        else
            GetComponent<Image>().sprite = BattleUIManager.Instance.NotDeployableTileSprite;

        gameObject.SetActive(isActive);
    }
}
