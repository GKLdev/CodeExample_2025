using GDTUtils;
using Modules.ActionsManger_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.Test
{
    public class TEST_ActionsManager : MonoBehaviour
    {
        public SerializedInterface<IActionsManager> target;
        public bool test        = false;
        public bool prewarmed   = false;
        public bool testImmediateRemoval    = false;
        public bool testInteruption         = false;

        //*****************************
        // Start
        //*****************************
        private void Start()
        {
            Debug.Assert(target.Value != null, "target is NULL");
            target.Value.InitModule();
        }

        //*****************************
        // Update
        //*****************************
        void Update()
        {
            if (target.Value == null)
            {
                return;
            }

            if (test)
            {
                test = false;
                Actions();
            }

            target.Value.OnUpdate();
        }


        //*****************************
        // Actions
        //*****************************
        void Actions()
        {
            IAction action;

            if (prewarmed)
            {
                action = target.Value.AddAction(ActionAliases.ExampleActionPrewarm);
            }
            else
            {
                action = target.Value.AddAction(ActionAliases.ExampleAction);
            }

            action.Start();
            action.Freeze(true);
            action.Freeze(false);
            action.TriggerFinishAction();

            if (testInteruption)
            {
                action.Interrupt();
            }

            if (testImmediateRemoval)
            {
                target.Value.RemoveAction(action);
            }
        }
    }
}