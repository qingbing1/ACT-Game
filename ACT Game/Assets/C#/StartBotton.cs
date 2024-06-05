using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBotton : MonoBehaviour
{

    public GameObject Loading;

    public GameObject start;

    void Start()
    {
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartGame()
    {
        Time.timeScale = 1f;
        Loading.SetActive(true);
        Invoke("DesLoading", 1.0f);
    }

    public void DesLoading()
    {
        SceneManager.LoadScene("Arab City 1");
    }
}
