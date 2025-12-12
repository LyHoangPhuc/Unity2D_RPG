using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private Animator anim;
    [SerializeField] public string id;
    public bool activationStatus;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    [ContextMenu("Generate checkpoint id")]
    private void GenerateId()
    {
        id = System.Guid.NewGuid().ToString();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>() != null)
        {
            AudioManager.instance.PlaySFX(41, null);
            ActivateCheckpoint();
        }
    }

    public void ActivateCheckpoint()
    {
        activationStatus = true;
        anim.SetBool("active", true);
        //Debug.Log($"Checkpoint {id} đã được kích hoạt");
    }

    public void StopAnimation()
    {
        anim.speed = 0;
    }
}
