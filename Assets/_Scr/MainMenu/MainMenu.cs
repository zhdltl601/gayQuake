using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartMainMenu(string stage1)
    {
        //ming
        SceneManager.LoadScene(stage1);
        Debug.Log("ÀÀ¾û¾Æ¤Ó¤µ");
    }
    public void Quit()
    {
        //ming
        Application.Quit();
    }
}
