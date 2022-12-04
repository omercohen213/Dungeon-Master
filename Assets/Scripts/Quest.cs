using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public int id;
    public string name;
    public string description;
    public int goldReward;
    public int xpReward;
    public List<int> enemiesIds;
    public int requiredLvl;
    public int currentAmount;
    public int requiredAmount;

    public Quest(int id, string name, string description, int goldReward, int xpReward, bool isActive, int requiredLvl, int currenAmount, int requiredAmount)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.goldReward = goldReward;
        this.xpReward = xpReward;
        this.requiredLvl = requiredLvl;
        this.currentAmount = currenAmount;
        this.requiredAmount = requiredAmount;
    }

    public bool isCompleted()
    {
        return (currentAmount >= requiredAmount);
    }


}
