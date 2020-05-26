﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterDataScript : MonoBehaviour
{
    public List<UnitData> unitDataList;
    public List<UnitData> pngnDataList;
    public List<UnitData> blockDataList;
    public List<ShipData> shipDataList;
    public List<StageData> stageDataList;
    public List<MissionItemData> missionItemDataList;
    public UnitData FindUnitData(int unitID)
    {
        foreach (UnitData data in unitDataList)
        {
            if (data.ID == unitID) return data;
        }
        return null;
    }

    public UnitData FindUnitData(string unitName)
    {
        foreach (UnitData data in unitDataList)
        {
            if (data.name == unitName) return data;
        }
        return null;
    }

    public UnitData FindPngnData(int unitID)
    {
        foreach (UnitData data in pngnDataList)
        {
            if (data.ID == unitID) return data;
        }
        return null;
    }

    public UnitData FindPngnData(string unitName)
    {
        foreach (UnitData data in pngnDataList)
        {
            if (data.name == unitName) return data;
        }
        return null;
    }

    
    public UnitData FindBlockData(int unitID)
    {
        foreach (UnitData data in blockDataList)
        {
            if (data.ID == unitID) return data;
        }
        return null;
    }

    public UnitData FindBlockData(string unitName)
    {
        foreach (UnitData data in blockDataList)
        {
            if (data.name == unitName) return data;
        }
        return null;
    }

    public ShipData FindShipData(int unitID)
    {

        foreach (ShipData shipData in shipDataList)
        {
            if (shipData.unitData.ID == unitID) return shipData;
        }
        return null;
    }

    public ShipData FindShipData(string unitName)
    {
        foreach (ShipData shipData in shipDataList)
        {
            if (shipData.unitData.name == unitName) return shipData;
        }
        return null;
    }

    public StageData FindStageData(int stageNum)
    {
        foreach (StageData data in stageDataList)
        {
            if (data.stageNum == stageNum) return data;
        }
        return null;
    }

    public Formation GetFormationFromStageNum(int stageNum)
    {
        Formation formation = new Formation();
        StageData data = FindStageData(stageNum);
        if (!data)
        {
            formation.formationDataExists = false;
            return formation;
        }

        //マジックナンバーすみません
        for (int i = 0; i < 10; i++)for(int j = 0;j<10;j++)
        {
                formation.gridinfo[i, j] = data.gridInfo[10 * i + j];
        }
        formation.shiptype = data.shipType;
        formation.formationDataExists = true;
        return formation;
    }

    public MissionItemData FindMissionItemData(int stageNum)
    {
        foreach (MissionItemData data in missionItemDataList)
        {
            if (data.stageNum == stageNum) return data;
        }
        return null;
    }
}
