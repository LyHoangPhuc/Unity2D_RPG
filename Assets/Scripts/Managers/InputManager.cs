using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    [Header("Input States")]
    public bool isInputBlocked = false;
    public bool isDialogueActive = false;

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
        }
    }

    // Ki?m tra xem có th? nh?n input không
    public bool CanReceiveInput()
    {
        return !isInputBlocked && !isDialogueActive;
    }

    // Ki?m tra input cho dialogue
    public bool CanReceiveDialogueInput()
    {
        return isDialogueActive;
    }

    // Block/Unblock input
    public void SetInputBlocked(bool blocked)
    {
        isInputBlocked = blocked;
    }

    // Set dialogue state
    public void SetDialogueActive(bool active)
    {
        isDialogueActive = active;
    }
}
