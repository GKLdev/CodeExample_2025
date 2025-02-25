using System;
using System.Collections;
using UnityEngine;

namespace Modules.CharacterFacade_Public
{
    public interface ICharacterFacadeCallbacks : IDisposable
    {
        void OnAdded();
        void OnAwake();
        void OnSlept();
    }
}