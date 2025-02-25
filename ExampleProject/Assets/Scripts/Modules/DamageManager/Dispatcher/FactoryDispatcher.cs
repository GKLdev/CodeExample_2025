using GDTUtils;
using GDTUtils.Patterns.Factory;
using Modules.ReferenceDb_Public;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace Modules.DamageDispatcher_Public
{
    // *****************************
    // FactoryDamageDispatcher
    // *****************************
    public class FactoryDamageDispatcher : IConcreteFactory<IDamageDispatcher>
    {
        Zenject.DiContainer container;
        LogicBase           prototype;

        // *****************************
        // FactoryDamageDispatcher
        // *****************************
        public FactoryDamageDispatcher(Zenject.DiContainer _container, LogicBase _prototype)
        {
            container = _container;
            prototype = _prototype;
        }

        // *****************************
        // Produce
        // *****************************
        public IDamageDispatcher Produce()
        {
            var dispatcher = container.InstantiatePrefabForComponent<IDamageDispatcher>(prototype);

            return dispatcher;
        }

        IFactoryProduct IFactory.Produce()
        {
            return Produce();
        }
    }

    // *****************************
    // PbmFactoryDamageDispatcher
    // *****************************
    public class PbmFactoryDamageDispatcher : PbmFactoryBase
    {
        DbEntryDamageDispatcher     entryCasted;
        CATEGORY_DAMAGEDISPATCHERS  type;

        // *****************************
        // Init
        // *****************************
        public override void Init(DbEntryBase _entry, Zenject.DiContainer _container, Enum _category, Transform _parent)
        {
            base.Init(_entry, _container, _category, _parent);

            entryCasted = _entry as DbEntryDamageDispatcher;
            type        = (CATEGORY_DAMAGEDISPATCHERS)_category;
        }

        // *****************************
        // Produce
        // *****************************
        public override GDTUtils.IPoolable Produce()
        {
            IDamageDispatcher result    = container.InstantiatePrefabForComponent<IDamageDispatcher>(entryCasted.Dispatcher);
            
            result.Init(entryCasted.Config);
            result.P_ElementType                        = type;
            result.P_GameObjectAccess.transform.parent  = parent;

            return result;
        }
    }
}