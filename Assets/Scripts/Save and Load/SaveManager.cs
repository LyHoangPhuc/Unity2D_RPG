using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;
public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private string fileName; //file chứa dữ liệu luư trữ của game 
    private GameData gameData;
    private List<ISaveManager> saveManagers;
    private FileDataHandler dataHandler;

    [ContextMenu("Delete save file")]

    public void DeleteSaveData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        dataHandler.Delete();
        //InvalidateCache();

    }
    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        //Application.persistentDataPath là một thuộc tính trong Unity, được sử dụng để xác định đường dẫn đến thư mục mà bạn có thể lưu trữ dữ liệu một cách bền vững
        //Đường dẫn cụ thể mà Application.persistentDataPath trả về phụ thuộc vào nền tảng mà ứng dụng đang chạy
        //Dữ liệu lưu trữ tại đường dẫn này sẽ tồn tại ngay cả khi ứng dụng được cập nhật hoặc khi người dùng xóa bộ nhớ cache
        
        //Debug.Log("[SaveManager] Start method called");
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        saveManagers = FindAllSaveManagers();

        //P Delay load để đảm bảo tất cả component đã được khởi tạo
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != "MainMenu")
        {
            StartCoroutine(DelayedLoad());
        }

        //LoadGame();
    }
    public void NewGame()
    {
        gameData = new GameData();
    }

    public bool HasSavedData()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, fileName);
        bool exists = File.Exists(fullPath);
        return exists;
    }
    public void LoadGame()
    {
        //game data = data from data handler
        gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            Debug.Log("No saved data not found");
            NewGame();
        }

        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }
    }

    public GameData LoadGameData()
    {
        if (dataHandler == null)
            dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);

        return dataHandler.Load();
    }

    public void SaveGame()
    {
        //data handler save gameData
        foreach (ISaveManager saveManager in saveManagers)
        {
            saveManager.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
        //InvalidateCache();
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }

    //private List<ISaveManager> FindAllSaveManagers()
    //{
    //    IEnumerable<ISaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>().OfType<ISaveManager>();
    //    return new List<ISaveManager>(saveManagers);
    //}

    //P 
    private List<ISaveManager> FindAllSaveManagers()
    {
        // Tìm tất cả objects implement ISaveManager, bao gồm cả inactive objects
        List<ISaveManager> saveManagers = new List<ISaveManager>();

        // Tìm trong active objects
        ISaveManager[] activeSaveManagers = FindObjectsOfType<MonoBehaviour>()
            .OfType<ISaveManager>().ToArray();
        saveManagers.AddRange(activeSaveManagers);

        // Tìm trong UI elements (có thể inactive)
        UI_SkillTreeSlot[] skillSlots = Resources.FindObjectsOfTypeAll<UI_SkillTreeSlot>();
        foreach (var slot in skillSlots)
        {
            if (slot.gameObject.scene.isLoaded) // Chỉ lấy objects trong scene hiện tại
                saveManagers.Add(slot);
        }

        return saveManagers;
    }
    private IEnumerator DelayedLoad()
    {
        yield return new WaitForSeconds(0.1f); // Đợi một chút
        LoadGame();
    }
}
