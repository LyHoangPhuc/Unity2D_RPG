using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour, ISaveManager
{   
    [Header("End Screen")]
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTree_UI;
    [SerializeField] private GameObject Inventory_UI;
    [SerializeField] private GameObject Craft_UI;
    [SerializeField] private GameObject Option_UI;
    [SerializeField] private GameObject inGame_UI;


    public UI_SkillTooltip skillTooltip;
    public UI_ItemTooltip itemTooltip;
    public UI_StatTooltip statTooltip;
    public UI_CraftWindow craftWindow;

    [SerializeField] private UI_VolumeSlider[] volumeSettings;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject restartButton;

    private Dictionary<KeyCode, GameObject> keyToUIMap;
    // Start is called before the first frame update

    private void Awake()
    {
        SwitchTo(skillTree_UI);               // we need this asssign on skill tree slots before we assigning event on skill script 
        fadeScreen.gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        SwitchTo(inGame_UI);

        itemTooltip.gameObject.SetActive(false);
        statTooltip.gameObject.SetActive(false);

        //endText.SetActive(false);
        //restartButton.SetActive(false);

        InitializeKeyMappings();

        // Thêm dòng này để kết nối nút Save and Exit
        SetupSaveAndExitButton();
    }

    private void SetupSaveAndExitButton()
    {
        // Tìm nút Save and Exit (thay "SaveExitButton" bằng tên thật của nút)
        Button saveExitButton = GameObject.Find("SaveExitButton")?.GetComponent<Button>();

        if (saveExitButton != null)
        {
            saveExitButton.onClick.AddListener(SaveAndExitGame);
        }
    }
    private void InitializeKeyMappings()
    {
        // COMMENT: Centralize key-UI mapping
        keyToUIMap = new Dictionary<KeyCode, GameObject>
    {
        { KeyCode.E, characterUI },
        { KeyCode.R, skillTree_UI },
        { KeyCode.B, Inventory_UI },
        { KeyCode.N, Craft_UI },
        { KeyCode.Escape, Option_UI }
    };
    }
    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.E))
    //        SwitchWithKeyTo(characterUI);

    //    if (Input.GetKeyDown(KeyCode.R))
    //        SwitchWithKeyTo(skillTree_UI);

    //    if (Input.GetKeyDown(KeyCode.B))
    //        SwitchWithKeyTo(Inventory_UI);

    //    if (Input.GetKeyDown(KeyCode.N))
    //        SwitchWithKeyTo(Craft_UI);

    //    if (Input.GetKeyDown(KeyCode.Escape))
    //        SwitchWithKeyTo(Option_UI);
    //}
    void Update()
    {
        // COMMENT: Cleaner input handling
        foreach (var kvp in keyToUIMap)
        {
            if (Input.GetKeyDown(kvp.Key))
            {
                if (InputManager.instance.CanReceiveInput())
                {
                    SwitchWithKeyTo(kvp.Value);
                    break; // COMMENT: Chỉ xử lý 1 phím mỗi frame
                }
                
            }
        }
    }
    //public void SwitchTo(GameObject _menu)
    //{

    //    for (int i = 0; i < transform.childCount; i++)
    //    {
    //        bool fadeScreen = transform.GetChild(i).GetComponent<UI_FadeScreen>() != null;  //cần dòng này để màn hình chính có thể hoạt động sau khi chuyển từ Main Menu sang màn hình chính 

    //        if (fadeScreen == false)
    //            transform.GetChild(i).gameObject.SetActive(false);
    //    }

    //    if (_menu != null)
    //    {
    //        _menu.SetActive(true);
    //    }
    //}

    // SỬA HÀM SwitchTo() - THAY THẾ loop bằng:
    public void SwitchTo(GameObject _menu)
    {
        // COMMENT: Sử dụng centralized function
        //Debug.Log($"[UI] Switching to: {(_menu != null ? _menu.name : "null")}");
        DisableAllUIExceptFadeScreen();

        if (_menu != null)
        {

            _menu.SetActive(true);
            
        }
    }

    public void SwitchWithKeyTo(GameObject _menu)
    {
        //Debug.Log($"[UI] Key pressed for: {(_menu != null ? _menu.name : "null")}. Currently active: {(_menu != null ? _menu.activeSelf : false)}");
        if (_menu != null && _menu.activeSelf)
        {
            _menu.SetActive(false);
            CheckForInGameUI();
            return;
        }

        SwitchTo(_menu);
    }

    private void CheckForInGameUI()
    {
        for (int i = 0;i < transform.childCount; i++)
        {
            if(transform.GetChild(i).gameObject.activeSelf && transform.GetChild(i).GetComponent<UI_FadeScreen>() == null)
            return;
        }

        SwitchTo(inGame_UI);
    }
    private void DisableAllUIExceptFadeScreen()
    {
        // COMMENT: Centralize logic để tắt tất cả UI
        for (int i = 0; i < transform.childCount; i++)
        {
            bool fadeScreen = transform.GetChild(i).GetComponent<UI_FadeScreen>() != null;
            if (fadeScreen == false)
                transform.GetChild(i).gameObject.SetActive(false);
        }
    }


    public void SwitchOnEndScreen()
    {
        fadeScreen.FadeOut();
        StartCoroutine(EndScreenCorutione());
    }

    IEnumerator EndScreenCorutione()
    {
        yield return new WaitForSeconds(1);
        endText.SetActive(true);
        yield return new WaitForSeconds(1);
        restartButton.SetActive(true);
    }

    public void RestartGameButton() => GameManager.instance.RestartSceen();

    public void LoadData(GameData _data)
    {
        foreach (KeyValuePair<string,float> pair in _data.volumeSettings) 
        {
            foreach(UI_VolumeSlider item in volumeSettings)
            {
                if(item.parameter == pair.Key)
                {
                    item.LoadSlider(pair.Value);
                }
            }
        }
    }

    public void SaveData(ref GameData _data)
    {
        _data.volumeSettings.Clear();

        foreach(UI_VolumeSlider item in volumeSettings)
        {
            _data.volumeSettings.Add(item.parameter, item.slider.value);
        }
    }

    public void SaveAndExitGame()
    {
        Debug.Log("Saving game and exiting...");

        // Save game
        if (SaveManager.instance != null)
        {
            SaveManager.instance.SaveGame();
            Debug.Log("Game saved successfully!");
        }

        // Exit to main menu
        StartCoroutine(SaveAndExitCoroutine());
    }

    private IEnumerator SaveAndExitCoroutine()
    {
        // Fade effect (optional)
        if (fadeScreen != null)
        {
            fadeScreen.FadeOut();
            yield return new WaitForSeconds(1f);
        }

        // Load main menu - thay "MainMenu" bằng tên scene thật của bạn
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
