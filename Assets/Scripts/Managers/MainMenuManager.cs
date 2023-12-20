using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Button btnPlay;



    private void OnEnable()
    {
        btnPlay.onClick.AddListener(BtnPlay);
    }
    private void OnDisable()
    {
        btnPlay.onClick.RemoveAllListeners();
    }

    void BtnPlay()
    {
        SceneManager.LoadScene(1);
    }
}
