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
    /// 해당 클래스 미니언을 배치할 수 있는 타일인지 확인
    /// </summary>
    /// <returns>true일시 배치 가능</returns>
    public bool IsDeployableMinionTile()
    {
        return (!isBlock && !isOnUnit && !ImpossibleUnitSetTile);
    }

    /// <summary>
    /// 배치가능한 타일 녹색으로 표시
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
