using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Progress : MonoBehaviour
{
    private float _value;

    public Text progress;

    public Image progressBar;

    [Header("Canvas elements For complete level")]

    public GameObject completeLevelBanner;
    public void Write(float val)
    {
        _value += (val);
        _value = Mathf.Clamp(_value, 0, 100);//ограничение
        progress.text = Mathf.Round(_value) + "%";
        progressBar.fillAmount = Mathf.Round(_value) * 0.01f;

        CompleteLevel();//проверяем завершена ли миссия
    }
    public void CompleteLevel()
    {
        if(_value >= 70)
        completeLevelBanner.SetActive(true);
    }
}

