using GDTUtils;
using GDTUtils.Patterns.Factory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.ActionsManger_Public
{
    public interface IActionsManager : IModuleInit, IModuleUpdate
    {
        /// <summary>
        /// Add actionType by alias from 'ActionAliases';
        /// </summary>
        IAction AddAction(string _alias);

        /// <summary>
        /// Remove action
        /// </summary>
        /// <param name="_action"></param>
        void RemoveAction(IAction _action);

        /// <summary>
        /// Prepare storage to action type. Good practice would be to init this on loading. Storage will be automatically created upon 'AddAction' call if not yet exists. but this might be performance heavy operation, depending on prewarm count.
        /// </summary>
        /// <param name="_alias"></param>
        void InitializeActionsStorage(string _alias);
        void DisposeActionsStorage(string _alias);
        void DisposeAllActions(bool _exceptDefault = true);
    }

}