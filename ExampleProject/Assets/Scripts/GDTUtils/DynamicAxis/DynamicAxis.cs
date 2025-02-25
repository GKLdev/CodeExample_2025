using UnityEngine;


namespace GDTUtils.DynamicAxis
{
    public class DynamicAxis : IDynamicAxis
    {
        State state = new();
        
        // *****************************
        // InitAxis 
        // *****************************
        public void InitAxis(float _min, float _max, float _upSpeed, float _downSpeed)
        {
            LibInitAxis.Init(state, _min, _max, _upSpeed, _downSpeed);
        }

        // *****************************
        // SetParam 
        // *****************************
        public void SetParam(IDynamicAxis.AxisParamType axisParam, float _value)
        {
            switch (axisParam)
            {
                case IDynamicAxis.AxisParamType.Min:
                    state.dynamicData.lowerLimit = _value;
                    break;
                case IDynamicAxis.AxisParamType.Max:
                    state.dynamicData.upperLimit = _value;
                    break;
                case IDynamicAxis.AxisParamType.DownSpeed:
                    state.dynamicData.downSpeed = _value;
                    break;
                case IDynamicAxis.AxisParamType.UpSpeed:
                    state.dynamicData.upSpeed = _value;
                    break;
                default:
                    break;
            }
            
            // clamp values
            state.dynamicData.axisProgress = Mathf.Clamp(state.dynamicData.axisProgress, state.dynamicData.lowerLimit, state.dynamicData.upperLimit);
            state.dynamicData.target       = Mathf.Clamp(state.dynamicData.target, state.dynamicData.lowerLimit, state.dynamicData.upperLimit);
        }

        // *****************************
        // ResetAxis 
        // *****************************
        public void ResetAxis()
        {
            LibInitAxis.Reset(state);
        }

        // *****************************
        // SetProgress 
        // *****************************
        public void SetProgress(float _tgt)
        {
            state.dynamicData.axisProgress = Mathf.Clamp(_tgt, state.dynamicData.lowerLimit, state.dynamicData.upperLimit);
        }

        // *****************************
        // SetProgressPercent 
        // *****************************
        public void SetProgressPercent(float _tgtPercent)
        {
            float _tgtClamped = Mathf.Clamp(_tgtPercent, -1f, 1f);
            SetProgress(_tgtClamped > 0 ? state.dynamicData.upperLimit * _tgtClamped : state.dynamicData.lowerLimit  * _tgtClamped);
        }

        // *****************************
        // SetTarget 
        // *****************************
        public void SetTarget(float _tgt)
        {
            LibSetTarget.SetTarget(state, _tgt);
        }

        // *****************************
        // SetTargetPercent 
        // *****************************
        public void SetTargetPercent(float _tgt)
        {
            float _tgtClamped = Mathf.Clamp(_tgt, -1f, 1f);
            LibSetTarget.SetTarget(state, GDTMath.LessOREqual(_tgtClamped, 0f) ? state.dynamicData.lowerLimit * Mathf.Abs(_tgtClamped) : state.dynamicData.upperLimit * Mathf.Abs(_tgtClamped));
        }

        // *****************************
        // GetTarget 
        // *****************************
        public float GetTarget()
        {
            return state.dynamicData.target;
        }

        // *****************************
        // GetTargetPercent 
        // *****************************
        public float GetTargetPercent()
        {
            return GetValueProgressPercent(state, state.dynamicData.target);
        }

        // *****************************
        // UpdateAxis 
        // *****************************
        public float UpdateAxis(float _deltaTime = 1f)
        {
            LibUpdateAxis.UpdateAxis(state, _deltaTime);
            
            return state.dynamicData.axisProgress;
        }

        // *****************************
        // GetProgress 
        // *****************************
        public float GetProgress()
        {
            return state.dynamicData.axisProgress;
        }
        
        // *****************************
        // GetProgressPercent 
        // *****************************
        public float GetProgressPercent()
        {
            return GetValueProgressPercent(state, state.dynamicData.axisProgress);
        }
        
        // *****************************
        // GetValueProgressPercent 
        // *****************************
        static float GetValueProgressPercent(State _state,  float _value)
        {
            bool limitsAreEqual = Mathf.Approximately(_state.dynamicData.lowerLimit, _state.dynamicData.upperLimit);
            if (limitsAreEqual)
            {
                return 0f;
            }
            
            bool isZero = Mathf.Approximately(_value, 0f);
            if (isZero)
            {
                return 0f;
            }

            float lowerLimit = !SignsAreEqual(_state.dynamicData.lowerLimit, _value) ? 0f : _state.dynamicData.lowerLimit;
            float upperLimit = !SignsAreEqual(_state.dynamicData.upperLimit,_value) ? 0f : _state.dynamicData.upperLimit;

            float deltaProgress = Mathf.Clamp(_state.dynamicData.axisProgress, lowerLimit, upperLimit) - lowerLimit;
            return deltaProgress / (upperLimit - lowerLimit);

            bool SignsAreEqual(float _val0, float _val1)
            {
                return Mathf.Approximately(Mathf.Sign(_val0) - Mathf.Sign(_val1), 0f);
            }
        }
    }
            
    // *****************************
    // State 
    // *****************************
    public class State
    {
        public DynamicData dynamicData = new DynamicData();
        
        public class DynamicData
        {
            public float axisProgress;
            public float lowerLimit;
            public float upperLimit;
            public float upSpeed;
            public float downSpeed;
            public float target;
        }
    }
}
