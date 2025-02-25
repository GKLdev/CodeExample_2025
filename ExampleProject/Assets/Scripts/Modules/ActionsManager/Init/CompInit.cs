using Modules.ActionsManger_Public;
using Modules.ModuleManager_Public;
using Modules.ReferenceDb_Public;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.ActionsManger
{
    public static class CompInit
    {
        // *****************************
        // Init 
        // *****************************
        public static void Init(State _state, IModuleManager _moduleMgr)
        {
            _state.dynamic.moduleMgr    = _moduleMgr;
            _state.dynamic.reference    = _moduleMgr.Container.Resolve<IReferenceDb>();
            InitContainers(_state);
            _state.initialized = true;
        }

        // *****************************
        // InitContainers 
        // *****************************
        static void InitContainers(State _state)
        {
            _state.dynamic.aliasToAction.Clear();

            // create default actions
            foreach (var item in ActionsTable.defaultActions)
            {
                var tuple   = ActionsTable.Actions[item];
                var storage = PrepareStorageInternal(_state, item, tuple.Item1, tuple.Item2, tuple.Item3);
                
                storage.P_IsDefaultAction = true;
            }
        }

        // *****************************
        // PrewarmActions 
        // *****************************
        public static bool PrepareStorage(State _state, string _alias)
        {
            bool aliasIsValid = ActionsTable.Actions.TryGetValue(_alias, out var tuple);
            Debug.Assert(aliasIsValid, $"PrepareStorage: Action with alias ={ _alias} not found!");

            bool alreadyExists = CompActionManagement.ActionsStorageExists(_state, _alias);
            if (alreadyExists)
            {
                return false;
            }

            PrepareStorageInternal( _state, _alias, tuple.Item1, tuple.Item2, tuple.Item3);

            return true;
        }

        // *****************************
        // PrepareStorage 
        // *****************************
        static IActionsStorage PrepareStorageInternal(State _state, string _key, Type _actionType, int _configDbId, int _prewarmElements)
        {
            // construct desired type name: ActionContainer<CUSTOM_TYPE>
            var strBuilder  = _state.dynamic.actionCreationData.stringBuilder;
            var config      = _state.dynamic.reference.GetEntry<ConfigActionBase>(_configDbId);

            Type configType = config.GetType();

            strBuilder.Clear();
            strBuilder.Append(typeof(ActionsStorage<Action_Example, ConfigActionExample>).ToString());
            strBuilder.Replace(_state.dynamic.actionCreationData.replacementPattern, _actionType.ToString());
            strBuilder.Replace(_state.dynamic.actionCreationData.replacementConfigPattern, configType.ToString());

            System.Type type = Type.GetType(strBuilder.ToString());

            // create instance of container type
            object obj = Activator.CreateInstance(type);
            IActionsStorage storage = obj as IActionsStorage;

            Debug.Assert(storage != null, $"Failed to create type for alias={_key}");

            storage.Setup(_state, config, _prewarmElements);
            _state.dynamic.aliasToAction.Add(_key, storage);

            return storage;
        }


        // *****************************
        // DisposeStorage 
        // *****************************
        public static void DisposeStorage(State _state, string _alias)
        {
            bool found = _state.dynamic.aliasToAction.TryGetValue(_alias, out var storage);
            Debug.Assert(found, $"DisposeStorage: Action with alias ={_alias} not found!");

            storage.Dispose();
            _state.dynamic.aliasToAction.Remove(_alias);
        }


        // *****************************
        // DisposeAll 
        // *****************************
        public static void DisposeAll(State _state, bool _exceptDefault)
        {
            List<KeyValuePair<string, IActionsStorage>> removeQueue = new();

            foreach (var item in _state.dynamic.aliasToAction)
            {
                bool skip = _exceptDefault && item.Value.P_IsDefaultAction;
                if (skip)
                {
                    break;
                }

                item.Value.Dispose();
                removeQueue.Add(item);
            }

            removeQueue.ForEach(x => _state.dynamic.aliasToAction.Remove(x.Key));
        }
    }
}