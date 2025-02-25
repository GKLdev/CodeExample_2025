using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LibModuleExceptions
{

    // *****************************
    // ExceptionIfNotInitialized
    // *****************************
    public static void ExceptionIfNotInitialized(bool _initialisedFlag)
    {
        if (!_initialisedFlag)
        {
            throw new System.Exception($"Module MUST be initialised before usage!");
        }
    }
}
