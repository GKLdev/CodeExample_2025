using System;
using System.Collections;
using System.Collections.Generic;
using GDTUtils.Patterns.Factory;
using UnityEngine;

namespace GDTUtils.StateMachine
{
    // *****************************
    // IStateMachine 
    // *****************************
    public interface IStateMachine<TNodeType, TExternalReference> : IDisposable, IFactoryProduct
        where TNodeType : Enum
        where TExternalReference : class
    {
        void SpecifyReference(TExternalReference _reference);
        void AddNode(TNodeType _type, StateMachineNodeBase<TNodeType, TExternalReference> _node);
        void TransitionTo(TNodeType _type, bool _force = false);
            
        void Update();
        void Reset();
        
        TNodeType P_ActiveNodeType { get; }
        
        StateMachineNodeBase<TNodeType, TExternalReference> GetNode(TNodeType _nodeType);
    }
    
    // *****************************
    // StateMachineFactory 
    // *****************************
    public class StateMachineFactory<TNodeType, TExternalReference> : IConcreteFactory<GDTStateMachine.StateMachine<TNodeType, TExternalReference>>
        where TNodeType : Enum
        where TExternalReference : class
    {
        public GDTStateMachine.StateMachine<TNodeType, TExternalReference> Produce()
        {
            GDTStateMachine.StateMachine<TNodeType, TExternalReference> result = new();

            return result;
        }

        IFactoryProduct IFactory.Produce()
        {
            return Produce();
        }
    }
    
    // *****************************
    // SM_NullState 
    // *****************************
    /// <summary>
    /// Use this for default states.
    /// </summary>
    public class SM_NullState<TNodeType, TExternalReference> : StateMachineNodeBase<TNodeType, TExternalReference>
        where TNodeType : Enum
        where TExternalReference : class
    {
        
        // *****************************
        // OnTransitionStarted 
        // *****************************
        protected override void OnTransitionStarted()
        {
            base.OnTransitionStarted();
            ReportNodeFinished();
        }
    }
    
    // *****************************
    // StateMachineNodeBase 
    // *****************************
    public class StateMachineNodeBase<TNodeType, TExternalReference> : IDisposable
        where TNodeType : Enum
        where TExternalReference : class
    {
        public bool P_AtTransition => atTransition;
        
        private   bool active       = false;
        private   bool atTransition = false;
        private   bool interrupted  = false;
        private   bool disposed     = false;
        
        protected TNodeType                                                   thisNodeType;
        protected TNodeType                                                   transitioningTo;
        protected GDTStateMachine.StateMachine<TNodeType, TExternalReference> stateMachine;
        
        // *****************************
        // Start 
        // *****************************
        /// <summary>
        /// called internally by state machine
        /// </summary>
        public void OnAdded(GDTStateMachine.StateMachine<TNodeType, TExternalReference> _stateMachine, TNodeType _thisNodeType)
        {
            stateMachine = _stateMachine;
            thisNodeType = _thisNodeType;
        }

        // *****************************
        // Start 
        // *****************************
        /// <summary>
        /// Make node active and running
        /// </summary>
        public void Start()
        {
            active       = true;
            interrupted  = false;
            atTransition = false;
            OnStart();
        }

        // *****************************
        // OnStart 
        // *****************************
        protected virtual void OnStart()
        {
        }

        // *****************************
        // Transition 
        // *****************************
        /// <summary>
        /// Translation to other node from this one.
        /// </summary>
        public void Transition(TNodeType _nodeType)
        {
            active          = false;
            atTransition    = true;
            transitioningTo = _nodeType;
            OnTransitionStarted();
        }

        // *****************************
        // OnTransitionStarted
        // *****************************
        protected virtual void OnTransitionStarted()
        {
            
        }

        // *****************************
        // InterruptNode
        // *****************************
        /// <summary>
        /// Interrupts node execution. Hard stop method basically. This is the only method called within Node on interruption..
        /// </summary>
        public void InterruptNode()
        {
            if (interrupted)
            {
                throw new System.Exception($"This node is already interrupted!");
            }

            interrupted     = true;
            
            OnInterruption();
            
            active          = false;
            atTransition    = false;
            interrupted     = false;
        }
        
        // *****************************
        // OnInterruption
        // *****************************
        /// <summary>
        /// Called before 'active' and 'atTransition' flags removed.
        /// </summary>
        protected virtual void OnInterruption()
        {
            
        }

        // *****************************
        // OnUpdate 
        // *****************************
        public void Update()
        {
            if (interrupted)
            {
                return;
            }
            
            if (active)
            {
                OnUpdate();
            }

            if (atTransition)
            {
                OnUpdateTransition(transitioningTo);                
            }
        }
        
        // *****************************
        // OnUpdate 
        // *****************************
        protected virtual void OnUpdate()
        {
            
        }

        // *****************************
        // OnUpdateTransition 
        // *****************************
        protected virtual void OnUpdateTransition(TNodeType _nodeType)
        {
            
        }

        // *****************************
        // ReportNodeFinished 
        // *****************************
        /// <summary>
        /// Call this from 'OnUpdateTransition' to tell state machine that node finished executing.
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected void ReportNodeFinished()
        {
            if (interrupted)
            {
                throw new System.Exception($"Calling 'ReportNodeFinished' is unacceptable when node is interrupted!");
            }

            if (!atTransition)
            {
                throw new System.Exception($"'ReportNodeFinished)' can only be called from transition state! Specifically from 'OnTransitionStarted' or 'OnUpdateTransition'.");
            }
            
            atTransition = false;
            stateMachine.OnNodeFinished();
        }

        // *****************************
        // Reset 
        // *****************************
        public void Reset()
        {
            OnReset();
            active          = false;
            atTransition    = false;
            interrupted     = false;
        }

        // *****************************
        // OnReset 
        // *****************************
        /// <summary>
        /// Called before flags cleared
        /// </summary>
        protected virtual void OnReset()
        {
            
        }

        // *****************************
        // Dispose 
        // *****************************
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed     = true;
            stateMachine = null;
        }
    }

    public class GDTStateMachine : MonoBehaviour
    {
        // *****************************
        // StateMachine 
        // *****************************
        public class StateMachine<TNodeType, TExternalReference> : IStateMachine<TNodeType, TExternalReference>
            where TNodeType : Enum
            where TExternalReference : class
        {
            public StateMachineNodeBase<TNodeType, TExternalReference> P_ActiveNode   => GetNode(activeNode);
            public TNodeType P_ActiveNodeType => activeNode;
            public TNodeType P_PreviousNodeType => previousNode;
            
            public TExternalReference                                  P_ExternalReference => reference;
            
            private bool               disposed     =  false;
            private TNodeType          activeNode   = default;
            private TNodeType          targetNode   = default;
            private TNodeType          previousNode = default;
            
            private TExternalReference reference;
            private Dictionary<TNodeType, StateMachineNodeBase<TNodeType, TExternalReference>> nodes = new();

            // *****************************
            // SpecifyReference 
            // *****************************
            public void SpecifyReference(TExternalReference _reference)
            {
                reference = _reference;
            }

            // *****************************
            // AddNode 
            // *****************************
            public void AddNode(TNodeType _type, StateMachineNodeBase<TNodeType, TExternalReference> _node)
            {
                nodes.Add(_type, _node);
                _node.OnAdded(this, _type);
            }
            
            // *****************************
            // TransitionTo 
            // *****************************
            public void TransitionTo(TNodeType _type, bool _force = false)
            {
                var  active          = P_ActiveNode;
                bool transitionError = !_force && (active is null ? false : active.P_AtTransition);
                if (transitionError)
                {
                    throw new System.Exception($"Trying to start transition to {_type} , while already at transition!");
                }

                targetNode = _type;
                
                if (_force)
                {
                    GetNode(activeNode).InterruptNode();
                    OnNodeFinished();
                    return;
                }
                
                active.Transition(_type);
            }
            
            // *****************************
            // OnNodeFinished 
            // *****************************
            public void OnNodeFinished()
            {
                previousNode = activeNode;
                activeNode   = targetNode;
                
                P_ActiveNode.Start();
            }

            // *****************************
            // Update 
            // *****************************
            public void Update()
            {
                foreach (var item in nodes.Values)
                {
                    item.Update();
                }
            }

            // *****************************
            // GetNode 
            // *****************************
            public StateMachineNodeBase<TNodeType, TExternalReference> GetNode(TNodeType _nodeType)
            {
                bool found = nodes.TryGetValue(_nodeType, out StateMachineNodeBase<TNodeType, TExternalReference> _node);
                if (!found)
                {
                    throw new System.Exception($"Item of type={_nodeType} does not exists.");
                }

                return _node;
            }

            // *****************************
            // Reset 
            // *****************************
            public void Reset()
            {
                foreach (var item in nodes.Values)
                {
                    item.Reset();
                }

                activeNode = default;
                targetNode = default;
            }

            // *****************************
            // Dispose 
            // *****************************
            public void Dispose()
            {
                if (disposed)
                {
                    return;
                }

                disposed  = true;
                reference = null;
                
                foreach (var item in nodes.Values)
                {
                    item.Update();
                }
                nodes.Clear();
            }
        }
    }
}