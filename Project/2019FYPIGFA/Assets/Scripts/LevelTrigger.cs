﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTrigger : MonoBehaviour
{
    public bool quitScene = false;
    [DrawIf("quitScene", false)]
    public string nextScene = default;
    public Timer timer;

    public void Activate()
    {

        if (quitScene)
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
        else
            SceneManager.LoadScene(nextScene);
    }
}
