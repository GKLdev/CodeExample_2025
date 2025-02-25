using Modules.ActionsManger_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.ActionsManger
{
    // *****************************
    // Action_Example 
    // *****************************
    public class Action_Example : ActionBase
    {
        float timeDelta = 0f;
        string updatingString;
        string updatingStringFinishing;

        // *****************************
        // OnActionStarted 
        // *****************************
        protected override void OnActionStarted()
        {
            base.OnActionStarted();

            Debug.Log($"Action started! id={Id}");
            updatingString          = $"Action update id={Id}";
            updatingStringFinishing = $"Action finishing update id={Id}";
        }

        // *****************************
        // OnUpdate 
        // *****************************
        protected override void OnUpdate(UpdateMode _mode, float _delta)
        {
            base.OnUpdate(_mode, _delta);

            if (_mode == UpdateMode.FinishingSequence)
            {
                Debug.Log(updatingStringFinishing);

                timeDelta += _delta;
                if (timeDelta > 1f)
                {
                    ReportFinished();
                }
            }
            else {
                Debug.Log(updatingString);
            }
        }

        // *****************************
        // OnFrozen 
        // *****************************
        protected override void OnFrozen(bool _val)
        {
            base.OnFrozen(_val);

            Debug.Log($"Action frozen! id={Id}");
        }


        // *****************************
        // OnTriggerFinishAction 
        // *****************************
        protected override void OnTriggerFinishAction()
        {
            base.OnTriggerFinishAction();

            Debug.Log($"Action finish sequence started! id={Id}");
        }

        // *****************************
        // OnActionInterrupted 
        // *****************************
        protected override void OnActionInterrupted()
        {
            base.OnActionInterrupted();

            Debug.Log($"Action interrupted! id={Id}");
        }


        // *****************************
        // OnReset 
        // *****************************
        protected override void OnReset()
        {
            base.OnReset();

            timeDelta = 0f;
            Debug.Log($"Action reset! id={Id}");
        }

        // *****************************
        // OnActionFinish 
        // *****************************
        protected override void OnActionFinished()
        {
            base.OnActionFinished();

            Debug.Log($"Action finished! id={Id}");
        }
    }

    public class Action_Example_Prewarm : Action_Example { }
}