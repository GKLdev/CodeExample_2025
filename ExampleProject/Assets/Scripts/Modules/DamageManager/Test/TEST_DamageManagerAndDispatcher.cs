using GDTUtils;
using Modules.DamageDispatcher_Public;
using Modules.DamageManager_Public;
using Modules.ReferenceDb_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.Test
{
    public class TEST_DamageManagerAndDispatcher : MonoBehaviour
    {
        public SerializedInterface<IDamageManager> damageMgr;

        public Transform spawnLocation;

        public bool test = false;
        public bool destroyDispatcher = false;
        public bool stopDpsDispatcher = false;
        public bool subscribeToOnFinished = false;

        public CATEGORY_DAMAGEDISPATCHERS   dispatcherType  = default;
        public Faction                      myFaction       = Faction.AI;

        [SerializeField]
        DispatcherDataSerialized data;

        IDamageDispatcher dispatcher;

        // *****************************
        // Start 
        // *****************************
        private void Start()
        {
            data.source         = new();
            data.source.faction = myFaction;
            data.source.obj     = this;

            if (subscribeToOnFinished)
            {
                data.onDispatcherFinished = OnDispatcherFinished;
            }
        }

        // *****************************
        // Update 
        // *****************************
        private void Update()
        {
            if (Input.GetKeyDown("v"))
            {
                test = true;
            }

            if (test)
            {
                test = false;
                RunTest();
            }

            if (stopDpsDispatcher)
            {
                stopDpsDispatcher = false;
                dispatcher.StopDispatcher();
            }

            if (destroyDispatcher)
            {
                destroyDispatcher = false;
                SleepDispacher();
            }
        }

        // *****************************
        // RunTest 
        // *****************************
        void RunTest()
        {
            dispatcher = damageMgr.Value.CreateDispatcher(dispatcherType);
            dispatcher.StartDispatcher(data);
        }

        // *****************************
        // OnDispatcherFinished 
        // *****************************
        void OnDispatcherFinished()
        {
            Debug.Log("Dispatcher finished!");
        }

        // *****************************
        // SleepDispather 
        // *****************************
        void SleepDispacher()
        {
            Debug.Log("destroying dispatcher...");
            damageMgr.Value.RemoveDispatcher(dispatcher);
        }

        [System.Serializable]
        class DispatcherDataSerialized : DamageDispatcherData
        {

        }
    }
}