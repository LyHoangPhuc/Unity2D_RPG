using UnityEngine;
using UnityEngine.UI;

public class Dash_Skill : Skill
{
    [Header("Dash")]
    public bool dashUnlocked;
    public UI_SkillTreeSlot dashUnlockButton;


    public override void UseSkill()
    {
        base.UseSkill();
        //Debug.Log("Dash");
    }


    protected override void Start()
    {
        base.Start();

        dashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDash);
    }
    protected override void Update()
    {
        base.Update();

        // Liên tục đồng bộ với UI state
        dashUnlocked = dashUnlockButton.unlocked;
    }
    protected override void CheckUnlock()
    {
        UnlockDash();
    }


    private void UnlockDash()
    {
        //Debug.Log("Attemtpt to unlock Dash");
        if (dashUnlockButton.unlocked)
        {
            //Debug.Log("Dash Unlock");
            PlayerManager.instance.player.fx.CreatePopUpText("Dash Unlock");
            dashUnlocked = true;
        }
    }
    

}
