using GDTUtils.Patterns.Factory;
using Unity.VisualScripting;

namespace GDTUtils
{
    public interface IDynamicAxis : IFactoryProduct
    {
        void InitAxis(float _min, float _max, float _upSpeed, float _downSpeed);
        void SetParam(AxisParamType axisParam, float _value);
        void ResetAxis();
        void SetTarget(float _tgt);
        void SetProgress(float _tgt);
        float GetTarget();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        float GetTargetPercent();
        
        /// <summary>
        /// /// <param name="_tgtPercent">0f..1.0f</param>
        /// </summary>
        void SetProgressPercent(float _tgtPercent);
        
        /// <summary>
        /// /// <param name="_tgtPercent">0f..1.0f</param>
        /// </summary>
        void SetTargetPercent(float _tgtPercent);
        float UpdateAxis(float _deltaTime = 1f);
        float GetProgress();
        
        /// <summary>
        /// </summary>
        /// <returns>0f..1.0f</returns>
        float GetProgressPercent();
        
        public enum AxisParamType
        {
            Min,
            Max,
            UpSpeed,
            DownSpeed
        }
    }

}
