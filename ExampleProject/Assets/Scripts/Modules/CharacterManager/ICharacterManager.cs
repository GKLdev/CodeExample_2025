using Modules.CharacterFacade_Public;
using Modules.ReferenceDb_Public;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.CharacterManager_Public
{
    // TODO: add an option to use hashcode instead of string aliases.
    public interface ICharacterManager : IModuleInit, IModuleUpdate, IDisposable
    {
        ICharacterFacade CreateCharacter(CATEGORY_CHARACTERS _type);
        ICharacterFacade CreateCharacter(string _alias);
        void RemoveCharacter(ICharacterFacade _character);

        /// <summary>
        /// Gets player character. If player not yet created, method will return null
        /// </summary>
        ICharacterFacade GetPlayer();
    }
}