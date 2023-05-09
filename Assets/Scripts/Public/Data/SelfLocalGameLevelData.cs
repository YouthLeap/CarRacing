using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelfLocalGameLevelData {

    public static string fileName = "SelfLocalGameLevelData.csv";
    int DataRow;
    //存数数据
    protected Hashtable DataTable;

    private SelfLocalGameLevelData()
    {

    }

    private static SelfLocalGameLevelData instance;
    public static SelfLocalGameLevelData Instance
    {
        get
        {
            if(instance == null)
                instance = new SelfLocalGameLevelData();

            return instance;
        }
    }

    public void CheckAndInitData()
    {
        if (!FileTool.IsFileExists(fileName))
        {
            string initStr = "Id,Note,Content,UseTime,CircleCount,OpponentList,GameLevelModel,SceneType,RoadModelName,RoadPointID";
            FileTool.createORwriteFile(fileName,initStr);
        }

        RefreshData();
    }

    void RefreshData()
    {
        if (DataTable == null)
            DataTable = new Hashtable();
        else
            DataTable.Clear();

        string content = FileTool.ReadFile(fileName, false);
        //读取每一行的内容
        string[] lineArray = content.Split ('\r');
        //创建二维数组
        string[][] levelArray = new string [lineArray.Length][];
        //把csv中的数据储存在二位数组中
        for(int i =0;i < lineArray.Length; i++)
        {
            levelArray[i] = lineArray[i].Split (',');
        }
        //将数据存储到哈希表中，存储方法：Key为name+id，Value为值
        int nRow = levelArray.Length;
        int nCol = levelArray[0].Length;

        DataRow = nRow - 1;

        for (int i = 1; i < nRow; ++i) 
        {
            if(levelArray[i][0]=="\n" || levelArray[i][0]=="")
            {
                nRow--;
                DataRow = nRow - 1;
                continue;
            }

            string id = levelArray[i][0].Trim();

            for (int j = 1; j < nCol; ++j) 
            {  
                DataTable.Add(levelArray[0][j] + "_" + id, levelArray[i][j]);
            }
        }

    }

    public int GetDataRow() {
        return DataRow;
    }

    //根据name和id获取相关属性，返回string类型
    protected virtual string GetProperty(string name, int id)
    {
        return GetProperty(name, id.ToString());
    }

    protected virtual string GetProperty(string name, string id)
    {
        string key = name + "_" + id;
        if(DataTable.ContainsKey(key))
            return DataTable[key].ToString();
        else
            return "";
    }

    public bool IsIDExist(int Id)
    {
        return !string.IsNullOrEmpty(GetProperty("Content", Id));
    }

    public string GetNode(int Id)
    {
        //Debug.Log (GetProperty("Note", Id));
        return GetProperty("Note", Id);
    }
    public string GetContent(int Id)
    {
        //Debug.Log (GetProperty("Content", Id));
        return GetProperty("Content", Id);
    }

    /// <summary>
    /// 关卡使用时间
    /// </summary>
    /// <returns>The use time.</returns>
    /// <param name="id">Identifier.</param>
    public int GetUseTime(int id)
    {
        int useTime=int.Parse( GetProperty ("UseTime",id) );
        return useTime;
    }
    public int GetCircleCount(int id)
    {
        int circleCount = int.Parse(GetProperty("CircleCount",id));
        return circleCount;
    }

    public string GetStrOpponent(int id)
    {
        string strOpponent= GetProperty("OpponentList",id);
        return strOpponent;
    }

    public string GetGameLevelModel(int id)
    {
        string gameModel= GetProperty("GameLevelModel",id);
        return gameModel;
    }

    public string GetSceneType(int id)
    {
        string typeStr = GetProperty ("SceneType",id);
        return typeStr;
    }

    public string GetRoadModelName(int id)
    {
        return GetProperty("RoadModelName",id);
    }

    public string GetRoadPointID(int id)
    {
        return GetProperty("RoadPointID",id);
    }
        
}
