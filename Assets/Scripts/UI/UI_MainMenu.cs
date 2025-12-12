using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "stage 1";
    [SerializeField] private GameObject continueButton;
    [SerializeField] UI_FadeScreen fadeScreen;
    private void Start()
    {
        if(SaveManager.instance.HasSavedData() == false)
            continueButton.SetActive(false);
    }
    public void ContinueGame()
    {
        // Load saved data để biết scene nào cần load
        GameData savedData = SaveManager.instance.LoadGameData();
        string sceneToLoad = "stage 1"; // default

        if (savedData != null && !string.IsNullOrEmpty(savedData.closestCheckpointScene))
        {
            sceneToLoad = savedData.closestCheckpointScene;
        }

        StartCoroutine(LoadScreenWithFadeEffect(1.5f, sceneToLoad));
    }
    public void NewGame()
    {
        SaveManager.instance.DeleteSaveData();
        StartCoroutine(LoadScreenWithFadeEffect(1.5f, sceneName));
    }
    public void ExitGame()
    {
        Debug.Log("Exit game"); 
        Application.Quit();
    }

    private IEnumerator LoadScreenWithFadeEffect(float delay, string targetScene)
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(targetScene);
    }

}
