using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestExperinceMainMenu : MonoBehaviour
{


    public void BeginExperince(int a)
    {
        PlayerPrefs.SetInt("ordinal", a);
        SceneManager.LoadScene("GameARTest");
    }
}
