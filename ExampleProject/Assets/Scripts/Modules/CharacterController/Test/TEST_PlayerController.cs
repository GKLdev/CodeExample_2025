using GDTUtils;
using Modules.CharacterVisualController_Public;
using Modules.ModuleManager_Public;
using Modules.CharacterController_Public;
using Modules.TimeManager_Public;
using UnityEngine;
using Zenject;
using Modules.CharacterController;
using UnityEngine.AI;
using Modules.CharacterFacade_Public;
using System.Collections.Generic;
using Modules.ActionsManger_Public;
using Modules.CharacterManager_Public;
using static UnityEngine.GraphicsBuffer;
using Modules.CameraController_Public;
using Modules.CameraController;
using Actions.Abilities_Public;

namespace Modules.Test
{
    public class TEST_PlayerController : MonoBehaviour
    {
        [Header("Spawnning options")]
        public bool         spawnFromCharacterManager = false;
        public string       characterAlias;
        public Transform    spawnPos;

        [Header("Camera")]
        public SerializedInterface<ICameraController> cameraController;

        [Header("Setup / init options")]
        public bool setupPlayerOnStart  = false;
        public bool forceDontInitPlayer = false;
        public SerializedInterface<ICharacterFacade>                target;
        public SerializedInterface<ICharacterVisualController>      visual;

        [Header("Navigation")]
        public Transform    navigationTarget;
        public bool         useStaticTarget = false;
        public Transform    testTarget;

        [Header("Mass test")]
        public List<LogicBase> massTestTargets;
        public List<ICharacterFacade> massTestTargetsCasted = new();

        [Header("Ability usage")]
        public string   runAbilityAlias;
        public bool     forceOrientation = false;

        [Header("Input")]
        public float mouseSenstivity = 10f;

        [Header("AI test")]
        public bool                 startAITest = false;
        public string               enemyCharacterAlias;
        public Transform            enemyCharactersSpawnPointsParent;
        private List<Transform>     enemyCharactersSpawnPoints = new();


        bool lastTimeToggledPhysics = false;

        MovementMode mode = MovementMode.DirectControl;
        AbilityConfigurationData abilityConfigData = new();


        [Inject]
        IModuleManager moduleMgr;
        ICharacterManager characterMgr;

        bool globalCameraMode = false;

        // *****************************
        // Start
        // *****************************
        void Start()
        {
            Debug.Assert(target.Value != null, "Player not defined!");
            Debug.Assert(visual.Value != null, "Visual controller not defined!");
            Debug.Assert(navigationTarget != null, "Navigation target not defined!");

            if (!forceDontInitPlayer)
            {
                if (!spawnFromCharacterManager)
                {
                    target.Value.InitModule(visual.Value);
                }
            }
            
            cameraController.Value?.InitModule();

            var resolvedcamera = moduleMgr.Container.TryResolve<ICameraController>();
            globalCameraMode = resolvedcamera != null;
            if (globalCameraMode) {
                cameraController.Value?.ToggleCamera(false);
                cameraController.Value = resolvedcamera;
            }

            if (spawnFromCharacterManager)
            {
                characterMgr = moduleMgr.Container.Resolve<ICharacterManager>();

                target.Value = characterMgr.CreateCharacter(characterAlias);
                target.Value.P_Controller.P_Position = spawnPos.position;

                cameraController.Value.SetFollowTarget(target.Value.P_GameObjectAccess.transform, true);
                cameraController.Value.ToggleCamera(true);
            }

            if (setupPlayerOnStart)
            {
                SetupPlayer();
            }

            if (enemyCharactersSpawnPointsParent != null)
            {
                enemyCharactersSpawnPoints.Clear();

                foreach (Transform child in enemyCharactersSpawnPointsParent)
                {
                    if (!child.gameObject.activeInHierarchy)
                    {
                        continue;
                    }

                    enemyCharactersSpawnPoints.Add(child);
                }
            }

            /*
            if (massTestTargets.Count > 0)
            {
                massTestTargets.ForEach(x => massTestTargetsCasted.Add(x as ICharacterFacade));
                massTestTargetsCasted.ForEach(x => {
                    x.InitModule();
                    x.P_Controller.ResetModule();
                    //x.P_Controller.TogglePhysics(true);
                });
            }
            */

            //moduleMgr.Container.Resolve<ITimeManager>().ToggleTimeEvaluation(true);
        }

        // *****************************
        // Update
        // *****************************
        void Update()
        {
            if (target.Value == null) 
            {
                return;
            }

            // waiting because of unity delta time bug
            if (Time.frameCount == 3) {
                moduleMgr.Container.Resolve<ITimeManager>().ToggleTimeEvaluation(true);
            }

            ProcessInput();

            if (mode == MovementMode.Path)
            {
                target.Value.P_Controller.SetNavigationTarget(navigationTarget.position);

                if (massTestTargets.Count > 0)
                {
                    massTestTargetsCasted.ForEach(x => x.P_Controller.SetNavigationTarget(navigationTarget.position));
                }
            }

            if (!spawnFromCharacterManager)
            {
                target.Value.OnUpdate();
            }

            if (massTestTargets.Count > 0)
            {
                foreach (var x in massTestTargetsCasted)
                {
                    x.OnUpdate();
                }
            }

            if (startAITest)
            {
                startAITest = false;
                RunAITest();
            }

            if (!globalCameraMode)
            {
                cameraController.Value?.OnLateUpdate();
            }
         }

        // *****************************
        // ProcessInput
        // *****************************
        void ProcessInput(){
            // reset
            if (Input.GetKeyDown("r"))
            {
                target.Value.P_Controller.ResetModule();
            }

            if (Input.GetKeyDown("p"))
            {
                lastTimeToggledPhysics = !lastTimeToggledPhysics;
                target.Value.P_Controller.TogglePhysics(lastTimeToggledPhysics);
            }

            if (Input.GetKeyDown("q"))
            {
                if (mode == MovementMode.DirectControl)
                {
                    mode = MovementMode.Path;
                } else if (mode == MovementMode.Path)
                {
                    mode = MovementMode.DirectControl;
                    target.Value.P_Controller.SetDirectMovementMode();

                    if (massTestTargets.Count > 0)
                    {
                        massTestTargetsCasted.ForEach(x => x.P_Controller.SetDirectMovementMode());
                    }
                }
            }

            // movement
            Vector3 movementVector      = Vector3.zero;
            float   mvVerticalAxis      = Input.GetAxis("Vertical");
            float   mvHorizontalAxis    = Input.GetAxis("Horizontal");

            if (!GDTMath.Equal(mvVerticalAxis, 0f)) {
                movementVector += Vector3.forward * mvVerticalAxis;
            }

            if (!GDTMath.Equal(mvHorizontalAxis, 0f))
            {
                movementVector += Vector3.right * mvHorizontalAxis;
            }

            movementVector.Normalize();
            target.Value.P_Controller.Move(movementVector);

            if (massTestTargets.Count > 0)
            {
                massTestTargetsCasted.ForEach(x => x.P_Controller.Move(movementVector));
            }

            // look target
            Vector3 lookTarget  = Vector3.zero;
            Vector3 mousePos    = Input.mousePosition;

            if (useStaticTarget)
            {
                if (testTarget == null)
                {
                    return;
                }

                lookTarget = testTarget.position;
                Debug.DrawRay(lookTarget, Vector3.up, Color.yellow);
                target.Value.P_Controller.LookAt(lookTarget);
                return;
            }

            // raycasting
            if (!cameraController.Value.RaycastPosition(out lookTarget))
            {
                return;
            }

            Debug.DrawRay(lookTarget, Vector3.up, Color.yellow);
            target.Value.P_Controller.LookAt(lookTarget);

            // usong abilities
            if (Input.GetButtonDown("Fire1"))
            {
                var     mgr             = target.Value.P_AbilitiesMgr;
                bool    canBeStarted    = mgr.CanRunAbility(runAbilityAlias);
                if (canBeStarted)
                {
                    abilityConfigData.applyStartingDirection = forceOrientation;

                    if (forceOrientation)
                    {
                        abilityConfigData.startDirection = (lookTarget - target.Value.P_Controller.P_Position).normalized;
                        mgr.StartAbility(runAbilityAlias, abilityConfigData);
                    }
                    else
                    {
                        mgr.StartAbility(runAbilityAlias);
                    }
                }
                else
                {
                    Debug.Log("Cant start Ability yet!");
                }
            }
        }

        // *****************************
        // SetupPlayer
        // *****************************
        void SetupPlayer()
        {
            target.Value.P_Controller.ResetModule();
            target.Value.P_Controller.TogglePhysics(true);
        }

        // *****************************
        // RunAITest
        // *****************************
        void RunAITest()
        {
            // despawn old characters
            spawnedAICharacters.ForEach(c => characterMgr.RemoveCharacter(c));
            spawnedAICharacters.Clear();

            // cspawn new ones
            foreach (var point in enemyCharactersSpawnPoints)
            {
                var ai = SpawnAsAI(point.position);
                spawnedAICharacters.Add(ai);
            }
        }


        private List<ICharacterFacade> spawnedAICharacters = new();
        
        // *****************************
        // SpawnAsAI
        // *****************************
        ICharacterFacade SpawnAsAI(Vector3 _pos)
        {
            // AI was removed from example project

            ICharacterFacade result = characterMgr.CreateCharacter(enemyCharacterAlias);

            result.P_Controller.P_Position = _pos;
            result.MakeAIControlled();
            result.P_Controller.TogglePhysics(true);

            return result;
        }
    }
}