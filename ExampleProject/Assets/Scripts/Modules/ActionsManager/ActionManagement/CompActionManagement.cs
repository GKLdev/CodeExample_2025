using Modules.ActionsManger_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.ActionsManger
{
    public static class CompActionManagement
    {
        // *****************************
        // AddAction 
        // *****************************
        public static IAction AddAction(State _state, string _alias)
        {
            bool storageExists = ActionsStorageExists(_state, _alias);
            if (!storageExists)
            {
                CompInit.PrepareStorage(_state, _alias);
            }

            return _state.dynamic.aliasToAction[_alias].AddAction();
        }

        // *****************************
        // RemoveAction 
        // *****************************
        public static void RemoveAction(State _state, IAction _action)
        {
            var container = (_action as IActionInternal).GetStorage();

            Debug.Assert(container != null, $"RemoveAction: Trying to remove action from disposed action storage!");
            container.RemoveAction(_action as ActionBase);
        }

        // *****************************
        // ActionExists 
        // *****************************
        public static bool ActionExists(State _state, string _alias, ActionBase _action)
        {
            bool result = _state.dynamic.aliasToAction.TryGetValue(_alias, out IActionsStorage cont);
            if (!result)
            {
                return result;

            }

            result = cont.CheckIfContains(_action);
            return result;
        }


        // *****************************
        // ActionsStorageExists 
        // *****************************
        public static bool ActionsStorageExists(State _state, string _alias)
        {
            bool result = _state.dynamic.aliasToAction.ContainsKey(_alias);
            return result;
        }
    }
}