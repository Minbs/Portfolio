using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynergyManager : Singleton<SynergyManager>
{
    public List<Node> busterSynergyNodes = new List<Node>();
    public int busterCoefficient;
    public List<Node> guardianSynergyNodes = new List<Node>();
    public int guardianCoefficient;
    public List<Node> chaserSynergyNodes = new List<Node>();
    public int chaserCoefficient;
    public List<Node> rescueSynergyNodes = new List<Node>();
    public int rescueCoefficient;

    /// <summary>
    /// 클래스에 따른 효과 적용
    /// </summary>
    /// <param name="minion"></param>
    public void CheckClassSynergy(GameObject minion)
    {
        if (GameManager.Instance.minionsList.Count <= 0)
            return;

       MinionClass minionClass = minion.GetComponent<Minion>().minionClass;

         switch(minionClass)
            {
                case MinionClass.Buster:
                    ActiveBusterSynergy(minion);
                    break;
                case MinionClass.Guardian:
                    ActiveGuardianSynergy(minion);
                    break;
                case MinionClass.Chaser:
                    ActiveChaserSynergy(minion);
                    break;
                case MinionClass.Rescue:
                    ActiveRescueSynergy(minion);
                    break;
            }
    }

    void ActiveBusterSynergy(GameObject minion)
    {
        int synergyCount = 0;
        foreach (var target in GameManager.Instance.minionsList)
        {
            if (target.Equals(minion))
                continue;


            foreach (var node in busterSynergyNodes)
            {
                if (target.GetComponent<Unit>().onTile.node == minion.GetComponent<Unit>().onTile.node + node)
                {
                    synergyCount++;
                }
            }
        }


        Debug.Log((int)(minion.GetComponent<Unit>().atk * (float)(busterCoefficient * synergyCount) / 100));
        minion.GetComponent<Unit>().currentAtk = minion.GetComponent<Unit>().atk + (minion.GetComponent<Unit>().atk * (busterCoefficient * synergyCount) / 100);
        
        minion.GetComponent<Unit>().maxHp = minion.GetComponent<Unit>().maxHpStat + (minion.GetComponent<Unit>().maxHpStat * (busterCoefficient * synergyCount) / 100);
        minion.GetComponent<Unit>().currentHp = minion.GetComponent<Unit>().maxHp;
    }

    void ActiveGuardianSynergy(GameObject minion)
    {
        foreach (var target in GameManager.Instance.minionsList)
        {
            if (target.Equals(minion))
                continue;


            foreach (var node in guardianSynergyNodes)
            {
                if (target.GetComponent<Unit>().onTile.node == minion.GetComponent<Unit>().onTile.node + node)
                {
                    target.GetComponent<Unit>().damageRedution = guardianCoefficient;
                }
            }
        }
    }

    void ActiveChaserSynergy(GameObject minion)
    {
        int synergyCount = 0;
        foreach (var target in GameManager.Instance.minionsList)
        {
            if (target.Equals(minion))
                continue;


            foreach (var node in chaserSynergyNodes)
            {
                if (target.GetComponent<Unit>().onTile.node == minion.GetComponent<Unit>().onTile.node + node)
                {
                    synergyCount++;
                }
            }
        }

        minion.GetComponent<Unit>().attackSpeed = 1 + ((float)chaserCoefficient * synergyCount / 100);
    }

    void ActiveRescueSynergy(GameObject minion)
    {
        foreach (var target in GameManager.Instance.minionsList)
        {
            if (target.Equals(minion))
                continue;

            foreach (var node in rescueSynergyNodes)
            {
                if (target.GetComponent<Unit>().onTile.node == minion.GetComponent<Unit>().onTile.node + node)
                {
                    target.GetComponent<Unit>().healTakeAmount = rescueCoefficient;
                }
            }
        }
    }
}
