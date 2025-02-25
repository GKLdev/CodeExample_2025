using System;
using System.Collections;
using System.Collections.Generic;
using GDTUtils;
using Modules.TimeManager_Public;
using UnityEngine;

namespace Test.TimeManager
{
    public class TEST_TimeManager : MonoBehaviour
    {
        [Header("Layers(T)")]
        public float worldScale = 1f;
        public float playerScale = 1f;
        public float playerWpnScale = 1f;
        public float projectileScale = 1f;

        [Header("Cooldowns (Y, 1 - p , 2 - unp, 3 - rst)")]
        public float cooldown;

        [SerializeField]
        private SerializedInterface<ITimeManager> target;

        private List<int> cooldownIds = new();

        private bool atSlomo = false;
        // *****************************
        // Update
        // *****************************
        private void Update()
        {
            if (Input.GetKeyDown("t"))
            {
                if (atSlomo)
                {
                    atSlomo = false;
                    target.Value.SetTimeScale(TimeLayerType.World, 1f);
                    target.Value.SetTimeScale(TimeLayerType.Player, 1f);
                    target.Value.SetTimeScale(TimeLayerType.PlayerWeapon, 1f);
                    target.Value.SetTimeScale(TimeLayerType.Projectiles, 1f);
                }
                else
                {
                    atSlomo = true;
                    target.Value.SetTimeScale(TimeLayerType.World, worldScale);
                    target.Value.SetTimeScale(TimeLayerType.Player, playerScale);
                    target.Value.SetTimeScale(TimeLayerType.PlayerWeapon, playerWpnScale);
                    target.Value.SetTimeScale(TimeLayerType.Projectiles, projectileScale);
                }
            }

            if (Input.GetKeyDown("y"))
            {
                TestStartCooldown();
            }
            
            if (Input.GetKeyDown("1"))
            {
                TestPauseCooldown();
            }
            
            if (Input.GetKeyDown("2"))
            {
                TestResumeCooldown();
            }
            
            if (Input.GetKeyDown("3"))
            {
                TestResetCooldown();
            }

            LogLayers();
            LogCooldowns();
            
            //Debug.Log("// ------------------------------------ //");
        }

        // *****************************
        // LogLayers
        // *****************************
        void LogLayers()
        {
            LogLayer(TimeLayerType.World);
            LogLayer(TimeLayerType.Player);
            LogLayer(TimeLayerType.PlayerWeapon);
            
            void LogLayer(TimeLayerType _type)
            {
                float dtime = target.Value.GetDeltaTime(_type);
                float scale = target.Value.GetTimeScale(_type);
                float total = target.Value.GetTimeSinceStartup(_type);

                //Debug.Log($"Layer: {_type}, Delta={dtime}, Total={total}, Scale={scale}");
            }
        }

        private bool started = false;
        // *****************************
        // TestStartCooldown
        // *****************************
        void TestStartCooldown()
        {
            if (started)
            {
                return;
            }

            int cdId = target.Value.AddCooldown(cooldown, TimeLayerType.World);
            target.Value.ResetCooldown(cdId);
            cooldownIds.Add(cdId);
            
            cdId = target.Value.AddCooldown(cooldown, TimeLayerType.Player);
            target.Value.ResetCooldown(cdId);
            cooldownIds.Add(cdId);
            started = true;
        }

        // *****************************
        // TestPauseCooldown
        // *****************************
        void TestPauseCooldown()
        {
            target.Value.PauseCooldown(0);
        }
        
        // *****************************
        // TestResumeCooldown
        // *****************************
        void TestResumeCooldown()
        {
            target.Value.ResumeCooldown(0);
        }
                
        // *****************************
        // TestResetCooldown
        // *****************************
        void TestResetCooldown()
        {
            target.Value.ResetCooldown(0);
        }

        // *****************************
        // LogCooldowns
        // *****************************
        void LogCooldowns()
        {
            for (int i = 0; i < cooldownIds.Count; i++)
            {
                float duration = target.Value.GetCooldownDuration(i);
                float progress = target.Value.GetCooldownProgressNormalised(i);
                bool passed    = target.Value.CheckCooldownPassed(i);
                
                //Debug.Log($"Cooldown[{i}]: {progress}, Duration={duration}, Passed = {passed}");
            }
        }
    }
}