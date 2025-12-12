using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CheckpointData
{
    public string checkpointId;
    public string sceneName;
    public Vector3 position;
}

public class GameManager : MonoBehaviour, ISaveManager 
{
    public static GameManager instance;

    private Transform player;
    [SerializeField] private string currentSceneName;

    [SerializeField] private CheckPoint[] checkpoints;
    [SerializeField] private string closestCheckpointId;

    [Header("Lost currency")]
    [SerializeField] private GameObject lostCurrencyPrefeb;
    public int lostCurrencyAmount;
    [SerializeField] private float lostCurrencyX;
    [SerializeField] private float lostCurrencyY;




    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }
    private void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;

        checkpoints = FindObjectsOfType<CheckPoint>();
        player = PlayerManager.instance.player.transform;
        Inventory.instance = FindObjectOfType<Inventory>();  

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (InputManager.instance.CanReceiveInput())
            {
                RestartSceen();
            }           
        }
    }

    public void RestartSceen()
    {
        SaveManager.instance.SaveGame();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void LoadData(GameData _data)
    {
        StartCoroutine(LoadWithDelay(_data));
        //Debug.Log("game manager loaded");
    }

    private void LoadCheckpoints(GameData _data)
    {
        foreach (KeyValuePair<string, bool> pair in _data.checkpoints)
        {
            foreach (CheckPoint checkpoint in checkpoints)
            {
                if (checkpoint.id == pair.Key && pair.Value == true)
                    checkpoint.ActivateCheckpoint();
            }
        }
    }

    private void LoadlostCurrency(GameData _data)
    {
        lostCurrencyAmount = _data.lostCurrencyAmount;
        lostCurrencyX = _data.lostCurrencyX;
        lostCurrencyY = _data.lostCurrencyY;

        if(lostCurrencyAmount > 0)
        {
            GameObject newLostCurrency = Instantiate(lostCurrencyPrefeb, new Vector3(lostCurrencyX, lostCurrencyY), Quaternion.identity);
            newLostCurrency.GetComponent<LostCurrencyController>().currency = lostCurrencyAmount;
        }

        lostCurrencyAmount = 0;
    }

    private IEnumerator LoadWithDelay(GameData _data)
    {
        yield return new WaitForSeconds(.1f);

        LoadCheckpoints(_data);
        LoadClosestCheckpoint(_data);
        LoadlostCurrency(_data);
    }

    public void SaveData(ref GameData _data)
    {
        _data.lostCurrencyAmount = lostCurrencyAmount;
        _data.lostCurrencyX = player.position.x;
        _data.lostCurrencyY = player.position.y;

        if (FindClosestCheckpoint() != null)
        {
            _data.closestCheckpointId = FindClosestCheckpoint().id;
            _data.closestCheckpointScene = currentSceneName;
        }

        _data.checkpoints.Clear();
        foreach(CheckPoint checkpoint in checkpoints)
        {
            _data.checkpoints.Add(checkpoint.id, checkpoint.activationStatus);
        }
    }


    private void LoadClosestCheckpoint(GameData _data)
    {
        if (_data.closestCheckpointId == null)
            return;

        // Kiểm tra xem có cần chuyển scene không
        string currentScene = SceneManager.GetActiveScene().name;
        if (!string.IsNullOrEmpty(_data.closestCheckpointScene) && currentScene != _data.closestCheckpointScene)
        {
            // Chuyển đến scene đúng trước khi spawn
            StartCoroutine(LoadSceneAndSpawn(_data));
            return;
        }

        closestCheckpointId = _data.closestCheckpointId;

        foreach (CheckPoint checkpoint in checkpoints)
        {
            if (closestCheckpointId == checkpoint.id)
            {
                player.position = checkpoint.transform.position;
                break;

            }

        }
    }

    private IEnumerator LoadSceneAndSpawn(GameData _data)
    {
        // Load scene đúng
        SceneManager.LoadScene(_data.closestCheckpointScene);

        // Đợi scene load xong
        yield return new WaitForEndOfFrame();

        // Cập nhật references
        checkpoints = FindObjectsOfType<CheckPoint>();
        player = PlayerManager.instance.player.transform;

        // Load checkpoint
        LoadClosestCheckpoint(_data);
    }

    private CheckPoint FindClosestCheckpoint()
    {
        float closestDistance = Mathf.Infinity;
        CheckPoint closestCheckpoint = null;

        foreach (var checkpoint in checkpoints)
        {
            float distanceToCheckpoint = Vector2.Distance(player.position, checkpoint.transform.position);

            if (distanceToCheckpoint < closestDistance && checkpoint.activationStatus == true)
            {
                closestDistance = distanceToCheckpoint;
                closestCheckpoint = checkpoint;
            }
        }

        return closestCheckpoint;
    }

    public void PauseGame(bool _pause)
    {
        if (_pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
                
    }
}
