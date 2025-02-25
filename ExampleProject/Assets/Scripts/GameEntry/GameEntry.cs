using System.Collections;
using System.Collections.Generic;
using Modules.ModuleManager_Public;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEntry : MonoBehaviour
{
    public bool         makeDontDestroyOnLoad = false;
    public bool         loadDefaultScene = false;
    public string       defaultScene;
    public LogicBase    modules;

    // *****************************
    // Start
    // *****************************
    private void Start()
    {
        var     moduleMng   = (modules as IModuleManager);
        bool    error       = moduleMng == null;
        if (error)
        {
            throw new System.Exception("Modules manager not assigned or does not implement IModuleManager");
        }

        moduleMng.InitModule();

        if(makeDontDestroyOnLoad){
            SetupDontDestroyOnLoad();
        }

        if (loadDefaultScene)
        {
            SceneManager.LoadScene(defaultScene);
        }
    }

    // *****************************
    // SetupDontDestroyOnLoad
    // *****************************
    private void SetupDontDestroyOnLoad()
    {
        Transform root      = default;
        Transform parent    = transform.parent;

        root = parent is null ? transform : parent;

        Object.DontDestroyOnLoad(root);
    }
}
