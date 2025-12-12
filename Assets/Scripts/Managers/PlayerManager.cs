using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, ISaveManager
{
    public static PlayerManager instance;
    public Player player;
    public int currency;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        //if (instance != null)
        //    Destroy(instance.gameObject);
        //else
        //    instance = this;
    }
    private void Start()
    {
        // Tìm player trong scene hiện tại nếu chưa có
        if (player == null)
        {
            FindPlayerInScene();
        }
    }
    // ✅ THÊM METHOD NÀY  
    private void OnEnable()
    {
        // Subscribe to scene loaded event
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ✅ THÊM METHOD NÀY
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Tự động tìm player mới khi scene được load
        FindPlayerInScene();
    }

    // ✅ THÊM METHOD NÀY
    private void FindPlayerInScene()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
            if (player != null)
            {
                Debug.Log($"Found player in scene: {player.name}");
                // ✅ THÊM DÒNG NÀY - Thông báo cho PlayerLevel cập nhật reference
                PlayerLevel playerLevel = FindObjectOfType<PlayerLevel>();
                if (playerLevel != null)
                {
                    playerLevel.RefreshPlayerStatsReference();
                }
            }
            else
            {
                Debug.LogWarning("No Player found in current scene!");
            }
        }
    }

    // ✅ THÊM METHOD NÀY - Public method để force refresh
    public void RefreshPlayerReference()
    {
        player = FindObjectOfType<Player>();
    }

    public bool HaveEnoughMoney(int _price)
    {
        if(_price > currency)
        {
            //Debug.Log("Not enough money");
            PlayerManager.instance.player.fx.CreatePopUpText("Not enough money");

            return false;
        }

        currency = currency - _price;
        return true;
    }

    public void LoadData(GameData _data)
    {
        this.currency = _data.currency;
    }

    public void SaveData(ref GameData _data)
    {
        _data.currency = this.currency;
    }

    public int GetCurrency() => currency;
    
    

}
