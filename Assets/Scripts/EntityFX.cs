using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    private SpriteRenderer sr;

    [Header("Pop Up Text")]
    [SerializeField] private GameObject popUpTextPrefab;

    [Header("Flash FX")]
    [SerializeField] private Material hitMat;       //vat lieu thay the hien thi hieu ung "bi danh"
    private Material originalMat;       //vat lieu ban dau cua doi tuong

    [Header("Ailment color")]
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color[] chillColor;
    [SerializeField] private Color[] shockColor;

    private GameObject myhealthBar;
    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMat = sr.material;

        myhealthBar = GetComponentInChildren<UI_HealthBar>().gameObject; 
    }

    public void CreatePopUpText(string _text)
    {
        float randomX = Random.Range(-1, 1);
        float randomY = Random.Range(1.5f, 3);

        Vector3 positionOffset = new Vector3(randomX, randomY, 0);

        GameObject newText = Instantiate(popUpTextPrefab, transform.position + positionOffset, Quaternion.identity);

        newText.GetComponent<TextMeshPro>().text = _text;
    }
    private IEnumerator FlashFX()
    {  
        sr.material = hitMat;
        Color currentColor = sr.color;
        sr.color = Color.white;

        yield return new WaitForSeconds(.2f);

        sr.color = currentColor;
        sr.material = originalMat;
    }

    private void  RedColorBlink()
    {
        if(sr.color != Color.white) 
            sr.color = Color.white;
        else
            sr.color = Color.red;
    }

    private void CancelColorChange()
    {
        CancelInvoke();
        sr.color = Color.white;
    }

    public void IgniteFxFor(float _second)      //hieu ung thieu dot
    {
        InvokeRepeating("IgniteColorFx", 0, .3f);
        Invoke("CancelColorChange", _second);
    }

    public void ChillFxFor(float _second)       //hieu ung lanh
    {
        InvokeRepeating("ChillColorFx", 0, .3f);
        Invoke("CancelColorChange", _second);
    }

    public void ShockFxFor(float _second)       //hieu ung dien giat
    {
        InvokeRepeating("ShockColorFx", 0, .3f);
        Invoke("CancelColorChange", _second);
    }

    private void IgniteColorFx()
    {
        if (sr.color != igniteColor[0])
            sr.color = igniteColor[0];
        else
            sr.color = igniteColor[1];
    }

    private void ChillColorFx()
    {
        if (sr.color != chillColor[0])
            sr.color = chillColor[0];
        else
            sr.color = chillColor[1];
    }

    private void ShockColorFx()
    {
        if (sr.color != shockColor[0])
            sr.color = shockColor[0];
        else
            sr.color = shockColor[1];
    }

    public void MakeTransprent(bool _transprent)
    {
        if (_transprent)
        {
            myhealthBar.SetActive(false);
            sr.color = Color.clear;
        }
        else
        {
            myhealthBar.SetActive(true);
            sr.color = Color.white;
        }
            
    }

    #region Jump Effects
    [Header("Jump Effects")]
    [SerializeField] private GameObject doubleJumpEffect;
    [SerializeField] private GameObject wallJumpEffect;

    // Effect cho double jump
    public void CreateDoubleJumpEffect()
    {
        if (doubleJumpEffect != null)
        {
            GameObject effect = Instantiate(doubleJumpEffect, transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        }
    }

    // Effect cho wall jump
    public void CreateWallJumpEffect()
    {
        if (wallJumpEffect != null)
        {
            Vector3 effectPos = transform.position + Vector3.right * GetComponent<Entity>().facingDir * 0.5f;
            GameObject effect = Instantiate(wallJumpEffect, effectPos, Quaternion.identity);
            Destroy(effect, 1f);
        }
    }
    #endregion




}
