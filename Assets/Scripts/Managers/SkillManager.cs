using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour, ISaveManager
{
    public static SkillManager instance;

    public Dash_Skill dash;
    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        dash = GetComponent<Dash_Skill>();
    }

    public void LoadData(GameData _data)
    {
        // Sau khi UI load xong, đồng bộ skill
        StartCoroutine(SyncSkillsAfterUILoad());
    }

    private IEnumerator SyncSkillsAfterUILoad()
    {
        yield return new WaitForEndOfFrame(); // Đợi UI load xong

        if (dash != null && dash.dashUnlockButton != null)
        {
            dash.dashUnlocked = dash.dashUnlockButton.unlocked;
            //Debug.Log($"Synced Dash skill: {dash.dashUnlocked}");
        }
    }

    public void SaveData(ref GameData _data)
    {
        // Không cần save gì thêm, UI đã save rồi
    }
}
