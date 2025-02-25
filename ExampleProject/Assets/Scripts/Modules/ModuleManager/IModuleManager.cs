using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Modules.ModuleManager_Public {
    
    public interface IModuleManager : IModuleInit
    {
        void AddToUpdateSequence(IModuleUpdate entry);
        void RemoveFromUpdateSequence(IModuleUpdate entry);

        void AddToLateUpdateSequence(IModuleLateUpdate _entry);
        void RemoveFromLateUpdateSequence(IModuleLateUpdate _entry);

        DiContainer Container { get; }
    }
}