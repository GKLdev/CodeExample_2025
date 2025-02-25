using System;
using System.Collections.Generic;
using Modules.ModuleManager_Public;
using UnityEngine;
using Zenject;

namespace Modules.ModuleManager
{
    public class CompInstaller : MonoInstaller
    {
        public List<State.ModuleContainer> initializationSequence = new ();

        public DiContainer RelatedContainer => Container;

        public IModuleManager self;
        
        // *****************************
        // BindSelf
        // *****************************
        public void BindSelf(IModuleManager _self)
        {
            Container.Bind<IModuleManager>().FromInstance(_self).AsSingle().NonLazy();
        }

        // *****************************
        // AddDependencies
        // *****************************
        public override void InstallBindings()
        {
            // bind self
            Container.Bind<IModuleManager>().FromInstance(self).AsSingle().NonLazy();

            foreach (var item in initializationSequence)
            {
                var type = Type.GetType(item.bindName);
                
                bool skip = type is null;
                if (skip)
                {
                    Debug.LogError($"Failed to get type='{item.bindName}'. Type was not found!");
                    continue;
                }
                
                Container.Bind(type).FromInstance(item.instance).AsSingle().NonLazy();
            }
        }
    }
}
