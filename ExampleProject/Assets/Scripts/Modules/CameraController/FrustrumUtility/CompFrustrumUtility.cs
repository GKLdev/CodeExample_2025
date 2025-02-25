using Modules.CameraController_Public;
using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace Modules.CameraController
{
    public static class CompFrustrumUtility
    {
        // *****************************
        // GetFrustrumSurfaceProjection
        // *****************************
        public static FrustumProjectionContainer GetFrustrumSurfaceProjection(State _state)
        {
            FrustumProjectionContainer result = default;

            GetBasicVectors(_state, ref result);

            return result;
        }

        // *****************************
        // GetCenterPoint
        // *****************************
        // works assuming FOV is vertical
        static void GetBasicVectors(State _state, ref FrustumProjectionContainer container)
        {
            Vector3 camPos = _state.camera.transform.position;
            Vector3 origin = _state.camera.transform.position;

            bool camIsOrthographic = _state.camera.orthographic;
            if (camIsOrthographic)
            {
                float horRectSize = _state.camera.orthographicSize * _state.camera.aspect;
                float verRectSize = _state.camera.orthographicSize;

                _state.dynamic.pointOrigins[0] = camPos + -_state.camera.transform.right * horRectSize + _state.camera.transform.up * verRectSize;
                _state.dynamic.pointOrigins[1] = camPos + _state.camera.transform.right * horRectSize + _state.camera.transform.up * verRectSize;
                _state.dynamic.pointOrigins[2] = camPos + _state.camera.transform.right * horRectSize + -_state.camera.transform.up * verRectSize;
                _state.dynamic.pointOrigins[3] = camPos + -_state.camera.transform.right * horRectSize + -_state.camera.transform.up * verRectSize;

                for (int i = 0; i < _state.dynamic.pointOrigins.Length; i++)
                {
                    _state.dynamic.pointDirections[i] = _state.camera.transform.forward;
                }
            }
            else
            {
                float vertAngle = _state.camera.fieldOfView * 0.5f;
                float horAngle  = Camera.VerticalToHorizontalFieldOfView(_state.camera.fieldOfView, _state.camera.aspect) * 0.5f;

                for (int i = 0; i < _state.dynamic.pointOrigins.Length; i++)
                {
                    _state.dynamic.pointOrigins[i] = origin;
                }

                _state.dynamic.pointDirections[0] = GetCornerPoint(horAngle, vertAngle, _state.camera.transform) - origin;
                _state.dynamic.pointDirections[1] = GetCornerPoint(-horAngle, vertAngle, _state.camera.transform) - origin;
                _state.dynamic.pointDirections[2] = GetCornerPoint(-horAngle, -vertAngle, _state.camera.transform) - origin;
                _state.dynamic.pointDirections[3] = GetCornerPoint(horAngle, -vertAngle, _state.camera.transform) - origin;
            }

            container.points = null;

            // cast points
            container.isValid = true;

            for (int i = 0;i < _state.dynamic.pointOrigins.Length; i++)
            {
                container.isValid = container.isValid && CastPoint(_state, _state.dynamic.pointOrigins[i], _state.dynamic.pointDirections[i], out _state.dynamic.projectedFrustrumPoints[i]);

                if (!container.isValid)
                {
                    return;
                }
            }

            container.points = _state.dynamic.projectedFrustrumPoints;

            // TODO: get center
            container.up        = _state.root.forward;
            container.right     = _state.root.right;
            container.normal    = _state.root.up;

            // determine most upper poijnt
            bool    toUpper     = _state.frustrumProjectionMode == FrustrumPointsOutputMode.ToUpper;
            float   maxHeight   = toUpper ? float.MinValue : float.MaxValue;

            for (int i = 0; i < container.points.Length; i++)
            {
                float height = container.points[i].y;
                bool isMax = toUpper ? height > maxHeight : height < maxHeight;
                if (isMax)
                {
                    maxHeight = height;
                }
            }

            // adjust all points relative height
            for (int i = 0; i < container.points.Length; i++)
            {
                container.points[i].y = maxHeight;
            }

            // get center point
            Vector3 rectHeight = Vector3.Project(container.points[3] - container.points[0], -_state.root.forward);
            Vector3 rectWidth  = Vector3.Project(container.points[1] - container.points[0], _state.root.right);

            container.center = container.points[0] + rectHeight * 0.5f + rectWidth * 0.5f;
            

            Vector3 GetCornerPoint(float _angleHor, float _angleVert, Transform _transform)
            {
                Vector3 result = default;

                float absHor = Mathf.Abs(_angleHor);
                float absVert = Mathf.Abs(_angleVert);

                Vector3 vertDir = Quaternion.AngleAxis(-_angleVert, _transform.right) * _transform.forward;
                Vector3 horDir  = Quaternion.AngleAxis(-_angleHor, _transform.up) * _transform.forward;
                float   vertDist    = Mathf.Sin(absVert * Mathf.Deg2Rad); // hypotenuse is 1f
                float   fwdDist     = Mathf.Cos(absVert * Mathf.Deg2Rad);
                float   horDist     = Mathf.Sin(absHor * Mathf.Deg2Rad) * fwdDist / Mathf.Sin((90f - absHor) * Mathf.Deg2Rad); // using "Law of sines" to get horizontal distance

                vertDist    *= Mathf.Sign(_angleVert);
                horDist     *= Mathf.Sign(_angleHor);

                result = _transform.position + _transform.forward * fwdDist + _transform.up * vertDist + -_transform.right * horDist;

                return result;
            }

            bool CastPoint(State _state, Vector3 _origin, Vector3 _dir, out Vector3 point)
            {
                bool result = false;

                result = RaycastFrustrumPoint(_state, _origin, _dir.normalized, out point, out _);

                if (result)
                {
                    Debug.DrawLine(_origin, point, Color.magenta, Time.deltaTime);
                }

                return result;
            }
        }

        // *****************************
        // RaycastFrustrumPoint
        // *****************************
        static bool RaycastFrustrumPoint(State _state, Vector3 _origin, Vector3 _direction, out Vector3 point, out Vector3 normal)
        {
            bool result = false;

            RaycastHit hit;

            result = Physics.Raycast(_origin, _direction, out hit, _state.camera.farClipPlane, _state.config.RaycastingMask);

            point   = hit.point;
            normal  = hit.normal;

            return result;
        }
    }
}