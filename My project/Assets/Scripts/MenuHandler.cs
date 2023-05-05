using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuHandler : MonoBehaviour
{
    // Start is called before the first frame update

    public void StandartMode()
    {
        Debug.Log("LOve");
        SceneManager.LoadScene("StandartMode");
    }

    public void KingMode()
    {
        SceneManager.LoadScene("KingMode");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void Exit()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
