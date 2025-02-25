using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GDTUtils
{
    public static class GDTRandom
    {
        public static Random generalRng = new();
     
        // *****************************
        // Random 
        // *****************************
        public class Random
        {
            private System.Random rng;
            
            // *****************************
            // Random 
            // *****************************
            public Random()
            {
                rng = new System.Random(GetDateTimeBasedRngSeed());
            }
            
            public Random(int _seed)
            {
                rng = new System.Random(_seed);
            }

            // *****************************
            // Next 
            // *****************************
            public int Next()
            {
                return rng.Next();
            }

            public int Next(int _maxValue)
            {
                return rng.Next(_maxValue);
            }
            
            public int Next(int _minValue, int _maxValue)
            {
                if (_maxValue < _minValue)
                {
                    throw new System.ArgumentException(" '_maxValue' is less than '_minValue'!");
                }

                return rng.Next(_minValue, _maxValue);
            }

            // *****************************
            // NextDouble 
            // *****************************
            public double NextDouble(double _maxValue)
            {
                return rng.NextDouble() * _maxValue;
            }

            public double NextDouble(double _minValue, double _maxValue)
            {
                double result = (_maxValue - _minValue);

                if (result < 0f)
                {
                    throw new System.ArgumentException(" '_maxValue' is less than '_minValue'!");
                }

                return result * rng.NextDouble();
            }
        }
             
        // *****************************
        // GetDateTimeBasedRngSeed 
        // *****************************
        public static int GetDateTimeBasedRngSeed()
        {
            DateTime    dtime   = DateTime.UtcNow;
            int         seed    = dtime.Year + dtime.Month + dtime.Day + dtime.Hour + dtime.Minute + dtime.Second + (int)(Time.time);

            return seed;
        }

        // *****************************
        // RandomlySelectByWeight 
        // *****************************

        /// <summary>
        /// Randomly select at item baserd on weight.
        /// </summary>
        /// <param name="_weights">normalized weights table. Use NormalizeWeights. </param>
        /// <param name="_randomValue"></param>
        /// <returns></returns>
        public static int RandomlySelectByWeight(float[] _weights, GDTRandom.Random _rng)
        {
            double totalSum = 0;
            double rngValue = _rng.NextDouble(1);
                ;
            for (int i = 0; i < _weights.Length; i++)
            {
                if (WeightSelectionInternal(_weights[i], rngValue, ref totalSum))
                {
                    return i;
                }
            }
            
            return -1;
        }

        /// <summary>
        /// Randomly select at item baserd on weight.
        /// </summary>
        /// <param name="_weights">normalized weights table. Use NormalizeWeights. </param>
        /// <param name="_randomValue"></param>
        /// <returns></returns>
        public static int RandomlySelectByWeight(List<float> _weights, GDTRandom.Random _rng)
        {
            double totalSum = 0;
            double rngValue = _rng.NextDouble(1);

            for (int i = 0; i < _weights.Count; i++)
            {
                if (WeightSelectionInternal(_weights[i], rngValue, ref totalSum))
                {
                    return i;
                }
            }

            return -1;
        }


        // *****************************
        // WeightSelectionInternal 
        // *****************************
        static bool WeightSelectionInternal(float _weight, double _randomValue, ref double _totalSum)
        {
            float weight = _weight * 100f;

            _totalSum += weight;

            return (_randomValue < _totalSum);
        }

        // *****************************
        // NormalizeWeights 
        // *****************************
        /// <summary>
        /// Used for weight normalization for 'RandomlySelectByWeight'
        /// </summary>
        /// <param name="_weights"></param>
        public static void NormalizeWeights(float[] _weights)
        {
            float totalWeightsSum   = 0f;
            float scale             = 1f;

            for (int i = 0; i < _weights.Length; i++)
            {
                totalWeightsSum += _weights[i];
            }

            if (Mathf.Approximately(totalWeightsSum, 0f))
            {
                return;
            }

            scale = 1f / totalWeightsSum;

            for (int i = 0; i < _weights.Length; i++)
            {
                _weights[i] *= scale;
            }
        }

        // *****************************
        // NormalizeWeights 
        // *****************************
        /// <summary>
        /// Used for weight normalization for 'RandomlySelectByWeight'
        /// </summary>
        /// <param name="_weights"></param>
        public static void NormalizeWeights(List<float> _weights)
        {
            float totalWeightsSum = 0f;
            float scale = 1f;

            for (int i = 0; i < _weights.Count; i++)
            {
                totalWeightsSum += _weights[i];
            }

            if (Mathf.Approximately(totalWeightsSum, 0f))
            {
                return;
            }

            scale = 1f / totalWeightsSum;

            for (int i = 0; i < _weights.Count; i++)
            {
                _weights[i] *= scale;
            }
        }

    }


}