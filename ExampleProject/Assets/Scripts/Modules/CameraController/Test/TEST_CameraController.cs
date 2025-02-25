using GDTUtils;
using Modules.CameraController_Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.Test
{
    public class TEST_CameraController : MonoBehaviour
    {
        public SerializedInterface<ICameraController> target;
        public Transform followTarget;

        public bool testFrustrumUtils = false;

        // *****************************
        // Start
        // *****************************
        private void Start()
        {
            Debug.Assert(target.Value != null, "Camera not defined!");
            target.Value.InitModule();

            if (followTarget != null)
            {
                target.Value.SetFollowTarget(followTarget, true);
            }

            target.Value.ToggleCamera(true);
        }

        // *****************************
        // LateUpdate
        // *****************************
        private void LateUpdate()
        {
            if (target.Value != null)
            {
                if (followTarget != null)
                {
                    target.Value.SetFollowTarget(followTarget);
                }

                target.Value.OnLateUpdate();
            }

            if (testFrustrumUtils)
            {
                TestFrustrumUtils();
            }
        }

        // *****************************
        // TestFrustrumUtils
        // *****************************
        void TestFrustrumUtils()
        {
            var frustrumData = target.Value.GetFrustrumSurfaceProjection();

            if (!frustrumData.isValid)
            {
                return;
            }

            for (int i = 0; i < frustrumData.points.Length; i++)
            {
                Debug.DrawRay(frustrumData.points[i], Vector3.up, Color.yellow, Time.deltaTime);
            }

            Debug.DrawRay(frustrumData.center, frustrumData.normal, Color.green, Time.deltaTime);
        }

    }
}