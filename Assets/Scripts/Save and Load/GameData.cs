using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable] de danh dau 1 class co the duoc Serializable.
//Một lớp được đánh dấu là Serializable có thể được chuyển đổi thành một định dạng có thể lưu trữ (như JSON hoặc XML).

//Deserialize là quá trình chuyển đổi một chuỗi dữ liệu (thường là JSON hoặc XML) trở lại thành một đối tượng trong mã.
//Điều này cho phép bạn phục hồi trạng thái của một đối tượng từ dữ liệu đã lưu.
//Sử dụng các thư viện như JsonUtility trong Unity để thực hiện quá trình này.

//-> Cho phep cac doi tuong cua class co the duoc luu tru va tai lai tu cac nguon luu tru du lieu nhu file, database hoac qua mang

[System.Serializable] 
public class GameData
{
    //inventory
    public int currency;

    public SerializableDictionary<string, bool> skillTree;
    public SerializableDictionary<string, int> inventory;   //là một cấu trúc dữ liệu cho phép lưu trữ các cặp khóa-giá trị (key-value pairs).
    public List<string> equipmentID;

    //Checkpoint
    public SerializableDictionary<string, bool> checkpoints;
    public string closestCheckpointId;
    public string closestCheckpointScene;

    //lost Currency
    public float lostCurrencyX;
    public float lostCurrencyY;
    public int lostCurrencyAmount;

    public SerializableDictionary<string, float> volumeSettings;

    //lv
    public int playerLevel;
    public int playerExp;

    // Story System
    public List<string> completedStorySequences;

    //Quest System
    public List<string> activeQuestIds = new List<string>();
    public List<string> completedQuestIds = new List<string>();
    public List<QuestProgressData> questProgressData = new List<QuestProgressData>();

 


    public GameData()
    {
        this.lostCurrencyX = 0;
        this.lostCurrencyY = 0;
        this.lostCurrencyAmount = 0;


        this.currency = 0;
        skillTree = new SerializableDictionary<string, bool>();
        inventory = new SerializableDictionary<string, int>();
        equipmentID = new List<string>();


        closestCheckpointId = string.Empty;
        closestCheckpointScene = string.Empty;
        checkpoints = new SerializableDictionary<string, bool>();


        

        this.playerLevel = 1;
        this.playerExp = 0;

        completedStorySequences = new List<string>();


        volumeSettings = new SerializableDictionary<string, float>();
    }
}
