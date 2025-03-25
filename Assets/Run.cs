using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// 用于控制整个系统的场景切换
public class Run : MonoBehaviour
{
    // Use this for initialization
    public GameObject pottery;
    void Start()
    {
        //GameObject potterynext=Instantiate(pottery);
        //DontDestroyOnLoad(pottery);
    }
    public void exit()
    {
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
    public void OnClick()
    {
        SceneManager.LoadScene("changeMesh");
    }
    public void OnClick1()
    {
     
        SceneManager.LoadScene("shangyou");
    }

    public void OnClick2()
    {
        SceneManager.LoadScene("shaotao");
    }
    public void OnReturn()
    {
        SceneManager.LoadScene("Start");
    }
    // Update is called once per frame
    void Update()
    {

    }

}
