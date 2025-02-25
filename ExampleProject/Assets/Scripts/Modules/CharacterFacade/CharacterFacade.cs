using GDTUtils;
using Modules.CharacterAbilitiesManager_Public;
using Modules.CharacterController_Public;
using Modules.CharacterFacade_Public;
using Modules.CharacterVisualController_Public;
using Modules.DamageManager_Public;
using Modules.ReferenceDb_Public;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.CharacterFacade
{
    /// <summary>
    /// Helps to init, update and to get access to all character components.
    /// </summary>
    public class CharacterFacade : LogicBase, ICharacterFacade
    {
        public int Id { get; set; }

        public ICharacterController         P_Controller => controller.Value;
        public ICharacterAbilitiesManager   P_AbilitiesMgr => abilitiesMgr.Value;
        public CATEGORY_CHARACTERS          P_ElementType { get; set; }
        public GameObject                   P_GameObjectAccess => gameObject;

        public SerializedInterface<ICharacterController>        controller;
        public SerializedInterface<ICharacterAbilitiesManager>  abilitiesMgr;
        public SerializedInterface<IDamageable>                 damageable;

        bool initialized    = false;
        bool isDisposed     = false;

        DamageSource damageSource = new();

        // *****************************
        // InitModule 
        // *****************************
        public void InitModule(ICharacterVisualController _visual)
        {
            if (initialized)
            {
                return;
            }

            controller.Value.InitModule();
            controller.Value.AttachVisual(_visual);
            abilitiesMgr.Value.InitModule();
            damageable.Value.OnCreated();
            damageable.Value.OnDamageApplied += OnDamage;

            initialized = true;

            // damageable
            damageSource.obj        = this;
            damageSource.faction    = damageable.Value.GetFaction();
        }


        // *****************************
        // SetupCharacter 
        // *****************************
        public void SetupCharacter(ConfigPlayerController configController, ConfigDamageable configDamageable)
        {
            if (initialized)
            {
                Debug.Assert(false, "SetupCharacter() can only be called BEFORE 'InitModule'! ");
            }

            controller.Value.SetupConfig(configController);
            damageable.Value.SetupConfig(configDamageable);
        }

        // *****************************
        // OnUpdate 
        // *****************************
        public void OnUpdate()
        {
            if (!initialized)
            {
                return;
            }

            controller.Value.OnUpdate(); // updates visual internally
        }

        // *****************************
        // GetDamageSource 
        // *****************************
        public DamageSource GetDamageSource()
        {
            return damageSource;
        }

        // *****************************
        // OnDamage 
        // *****************************
        void OnDamage(bool _isDead) {
            // TODO
        }

        // *****************************
        // MakeAIControlled 
        // *****************************
        public void MakeAIControlled()
        {
            return;
            // AI was removed from example project
        }


        // *****************************
        // MakePlayerControlled 
        // *****************************
        public void MakePlayerControlled()
        {
            P_Controller.SetDirectMovementMode();
        }

        // *****************************
        // IsDead 
        // *****************************
        public bool IsDead()
        {
            return damageable.Value.IsDead();
        }

        //------------------------------
        // Poolable:
        //------------------------------

        // *****************************
        // Activate 
        // *****************************
        public void Activate()
        {
            controller.Value.OnAwake();
            controller.Value.GetVisualController().OnAwake();
            abilitiesMgr.Value.OnAwake();
            damageable.Value.ToggleActive(true);
        }

        // *****************************
        // Deactivate 
        // *****************************
        public void Deactivate()
        {
            controller.Value.GetVisualController().OnSlept();
            controller.Value.OnSlept();
            abilitiesMgr.Value.OnSlept();
            damageable.Value.ResetDamageable();
            damageable.Value.ToggleActive(false);
        }

        // *****************************
        // OnAdded 
        // *****************************
        public void OnAdded()
        {
            controller.Value.OnAdded();
            controller.Value.GetVisualController().OnAdded();
            abilitiesMgr.Value.OnAdded();
        }

        // *****************************
        // Dispose 
        // *****************************
        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;
            controller.Value.GetVisualController().Dispose();
            controller.Value.Dispose();
            abilitiesMgr.Value.Dispose();
            damageable.Value.Dispose();
        }
    }
}