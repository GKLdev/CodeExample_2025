using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDTUtils.Animation
{
    public class AnimBhv_EventShooter : AnimBhvBase
    {
        public AnimEventContainer[] events;

        private int cycle = 0;
        
        //*****************************
        // OnStateEnter
        //*****************************
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            cycle = 0;
        }

        //*****************************
        // OnStateUpdate
        //*****************************
        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            float animationProgress = 0f;
            UpdateAnimProgress(ref animationProgress, stateInfo.normalizedTime, cycle);

            bool justStarted = Mathf.Approximately(animationProgress, 0f) || animationProgress > 1f; // if 0.99 and jumped over 1  event will never hit
            if (justStarted)
            {
                CheckAndDispatchEvents(1f);  // if 0.99 and jumped over 1  event will never hit so checkfor for prev events

                cycle++;
                UpdateAnimProgress(ref animationProgress, stateInfo.normalizedTime, cycle);
                ClearEventData(); // clear at new cycle if not HitOnce event
            }

            CheckAndDispatchEvents(animationProgress);
        }

        //*****************************
        // UpdateAnimProgress
        //*****************************
        void UpdateAnimProgress(ref float _progress, float _totalTime, int _cycle )
        {
            _progress = _totalTime - (float)cycle;
        }

        //*****************************
        // OnStateExit
        //*****************************
	    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            cycle = 0;
            CheckAndDispatchEvents(1f);
            ClearEventData(true);
        }

        //*****************************
        // ClearEventData
        //*****************************
        void ClearEventData(bool _force = false)
        {
            // clear history
            for (int i = 0; i < events.Length; i++)
            {
                bool clear = _force || !events[i].hitOnce; // hit once prevents "eventHappened" of being cleared unless "_force" is set to true
                if (clear)
                {
                    events[i].eventHappened = false;
                }
            }
        }

        //*****************************
        // CheckAndDispatchEvents
        //*****************************
        void CheckAndDispatchEvents(float _animProgress)
        {
            for (int i = 0; i < events.Length; i++)
            {
                if (events[i].ignoreEvent)
                {
                    continue;
                }

                bool timeisvalid    = _animProgress > events[i].eventTimeNormalized || Mathf.Approximately(_animProgress, events[i].eventTimeNormalized);
                bool eventHit       = !events[i].eventHappened && timeisvalid;
                if (eventHit)
                {
                    events[i].eventHappened = true;

                    // try make callback
                    int eventId = events[i].eventId;
                    if (eventId != -1)
                    {
                        if (initContainer == null)
                        {
                            Debug.LogError($"initContainer is NULL at AnimBhv_EventShooter={this}");
                            return;
                        }

                        initContainer.onAnimationEvent?.Invoke(eventId, events[i].eventParam);
                    }
                }
            }
        }

        //*****************************
        // AnimEventContainer
        //*****************************
        [System.Serializable]
        public class AnimEventContainer {
            public bool ignoreEvent = false;
            public bool hitOnce = false;
            
            [HideInInspector]
            public bool                     eventHappened = false;
            public int                      eventId;
            public int                      eventParam;
            public float                    eventTimeNormalized;
            //public int                      feedbackValue;
            //public Bhv_Base                 feedbackToBhv;
        }
    }
}