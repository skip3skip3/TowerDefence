﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NCMB;
using UniRx;
using System;

public class NCMBDatabase : MonoBehaviour
{

    private NCMBQuery<NCMBObject> queryPostStage;
    private NCMBQuery<NCMBObject> queryFetchAllStage;
    private NCMBQuery<NCMBObject> queryStageRanking;

    private Subject<List<StageData>> _StageDataList = new Subject<List<StageData>>();
    public IObservable<List<StageData>> StageDataObservable
    {
        get { return _StageDataList; }
    }

    // 上位ステージ
    private Subject<List<StageData>> _TopStageDataList = new Subject<List<StageData>>();
    public IObservable<List<StageData>> TopStageDataObservable
    {
        get { return _TopStageDataList; }
    }



    // TODO: Constファイルを作る...
    public static string ONLINE_STAGE_DATA = "OnlineStageData";
    void Start()
    {
        //FetchRankingData();
    }
    public void PostStageData(int slotNum, string stageName, string detailContent, DialogManager dialogManager)
    {
        PrefsManager prefs = new PrefsManager();
        Formation formation = prefs.GetFormation(slotNum);

        if (formation.formationDataExists)
        {
            queryPostStage = new NCMBQuery<NCMBObject>(ONLINE_STAGE_DATA);

            // 保存されているデータ件数を取得
            queryPostStage.CountAsync((int count, NCMBException e) =>
            {
                if (e != null)
                {
                    dialogManager.ShowDialog("送信に失敗しました..");
                    Debug.Log("OnlineStageDataの件数の取得に失敗しました");
                }
                else
                {
                    NCMBObject stageData = new NCMBObject(ONLINE_STAGE_DATA);

                    // OnlineStageDataに値を設定
                    stageData["ID"] = count + 1;
                    stageData["name"] = stageName;
                    stageData["detailContent"] = detailContent;

                    stageData["gridInfo"] = formation.gridinfo;
                    stageData["shipInfo"] = formation.shiptype;
                    stageData["difficulty"] = 1;

                    stageData["winCount"] = 0;
                    stageData["loseCount"] = 0;

                    // データストアへの登録
                    stageData.SaveAsync((NCMBException ee) =>
                    {
                        if (ee != null)
                        {
                            Debug.Log("bug");
                            Debug.Log(ee.ToString());
                        }
                        else
                        {
                            SceneManager.LoadScene("OnlineEntranceScene");
                        }
                    });
                }
            });
        }
        else
        {
            Debug.Log("buggg");
        }
    }
    public void FetchAllStageData()
    {
        var stageDataList = new List<StageData>();
        queryFetchAllStage = new NCMBQuery<NCMBObject>(ONLINE_STAGE_DATA);
        // TODO: クエリ条件の導入

        queryFetchAllStage.FindAsync((List<NCMBObject> fetchList, NCMBException e) =>
        {
            if (e != null)
            {
                //検索失敗時の処理
            }
            else
            {
                //TODO: 最大表示数を決める?ランダムにソートする?
                foreach (NCMBObject fetchStage in fetchList)
                {
                    stageDataList.Add(ParceStageData(fetchStage));
                }
                stageDataList.Shuffle();
                _StageDataList.OnNext(stageDataList);
            }
        });
    }
    public void FetchRankingData()
    {
        var stageDataList = new List<StageData>();
        queryStageRanking = new NCMBQuery<NCMBObject>(ONLINE_STAGE_DATA);

        // TODO: クエリ条件の見直し
        queryStageRanking.Limit = 10;
        queryStageRanking.WhereGreaterThanOrEqualTo("winCount", 1);
        queryStageRanking.OrderByDescending("winCount");
        queryStageRanking.OrderByDescending("winPercentage");

        queryStageRanking.Find((List<NCMBObject> fetchRankingList, NCMBException e) =>
        {
            if (e != null)
            {
                //検索失敗時の処理
            }
            else
            {
                foreach (NCMBObject fetchStage in fetchRankingList)
                {
                    stageDataList.Add(ParceStageData(fetchStage));
                }
                _TopStageDataList.OnNext(stageDataList);
            }
        });
    }

    private StageData ParceStageData(NCMBObject fetchStage)
    {
        StageData stageData = ScriptableObject.CreateInstance<StageData>();

        stageData.gridInfo = new int[100];
        var l = fetchStage["gridInfo"] as ArrayList;
        for (int i = 0; i < (fetchStage["gridInfo"] as ArrayList).Count; i++)
        {
            stageData.gridInfo[i] = System.Convert.ToInt32(l[i]);
        }

        stageData.ID = System.Convert.ToInt32(fetchStage["ID"]);
        stageData.uuid = fetchStage.ObjectId;
        stageData.name = fetchStage["name"].ToString();
        stageData.detailContent = fetchStage["detailContent"].ToString();
        stageData.shipInfo = System.Convert.ToInt32(fetchStage["shipInfo"]);
        stageData.difficulty = System.Convert.ToInt32(fetchStage["difficulty"]);
        stageData.winCount = System.Convert.ToInt32(fetchStage["winCount"]);
        stageData.loseCount = System.Convert.ToInt32(fetchStage["loseCount"]);
        return stageData;
    }
}

public static class Extensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            var tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }
}
