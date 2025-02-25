using GDTUtils;
using Modules.CharacterFacade_Public;
using Modules.CharacterManager_Public;
using Modules.ReferenceDb_Public;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Modules.Test
{
    public class TestCharacterManager : LogicBase
    {
        public SerializedInterface<ICharacterManager> target;
        
        //public ICharacterFacade character;
        
        public CATEGORY_CHARACTERS characterType    = default;
        public CATEGORY_CHARACTERS characterType2   = default;

        [Header("Default tests")]
        public bool runSimpleTest;
        public bool interruptCurrentTest;
        public float characterUptime;

        public bool runComplexTest; // resetting charcters
        public float characterRespawnTime;

        [Header("Manual testing")]
        public bool manualTest;
        public bool manualSpawn;
        public bool manualRemove;
        public bool manualStartAbility;
        public bool manualStartPath;
        public bool manualMove;
        public string abilityAlias;

        CancellationTokenSource cts;

        public Transform    _charactersTargetPos;
        public Transform    _charactersSpawnPos;

        ICharacterFacade spawnedCharacter;

        // *****************************
        // Start 
        // *****************************
        private void Start()
        {
            target.Value.InitModule();
            cts = new();
        }

        // *****************************
        // Update 
        // *****************************
        private void Update()
        {
            if (runSimpleTest)
            {
                runSimpleTest = false;

                if (interruptCurrentTest && !cts.IsCancellationRequested)
                {
                    cts.Cancel();
                }

                cts = new();

                var wrap = TestWrap(SimpleTest(cts.Token), cts.Token);
            }

            if (runComplexTest)
            {
                runComplexTest = false;

                if (interruptCurrentTest && !cts.IsCancellationRequested)
                {
                    cts.Cancel();
                }

                cts = new();

                var wrap = TestWrap(ComplexTest(cts.Token), cts.Token);
            }

            if (manualTest)
            {
                if (manualSpawn)
                {
                    manualSpawn = false;
                    spawnedCharacter = SpawnCharacter();
                }

                if (manualRemove)
                {
                    manualRemove    = false;
                    manualMove      = false;
                    RemoveCharacter(ref spawnedCharacter);
                }

                if (manualStartAbility)
                {
                    manualStartAbility = false;
                    spawnedCharacter?.P_AbilitiesMgr.StartAbility(abilityAlias);
                }

                if (manualStartPath)
                {
                    manualStartPath = false;
                    spawnedCharacter?.P_Controller.SetNavigationTarget(_charactersTargetPos.position);
                }

                if (manualMove)
                {
                    spawnedCharacter?.P_Controller.SetDirectMovementMode();
                    spawnedCharacter?.P_Controller.Move(-Vector3.right);
                }

                spawnedCharacter?.OnUpdate();
            }
        }

        // *****************************
        // SimpleTest 
        // *****************************
        async Task SimpleTest(CancellationToken _token)
        {
            // create at pos
            // wait
            // move with physics for some time
            // move be patgh for some time
            // remove

            ICharacterFacade character = null;

            character = SpawnCharacter();
            character.OnUpdate();

            Debug.Log("Spawned character");

            try
            {
                await Task.Delay((int)(characterUptime * 1000f), _token);
            }
            catch (System.Exception)
            {
                RemoveCharacter(ref character);
                return;
            }

            Debug.Log("Direct movement...");

            character.P_Controller.SetDirectMovementMode();


            float t = characterUptime * 2;

            while (t > 0f) {
                t -= Time.deltaTime;

                if (t > characterUptime)
                {
                    character.P_Controller.Move(-Vector3.right);
                }
                else
                {
                    character.P_Controller.SetNavigationTarget(_charactersTargetPos.position);
                }

                character.OnUpdate();

                try
                {
                    await Task.Delay(1, _token);
                } 
                catch (System.Exception)
                {
                    RemoveCharacter(ref character);
                    return;
                }
            }

            RemoveCharacter(ref character);
        }

        // *****************************
        // SpawnCharacter 
        // *****************************
        ICharacterFacade SpawnCharacter()
        {
            ICharacterFacade character;

            character = target.Value.CreateCharacter(characterType);
            character.P_Controller.TogglePhysics(true);

            character.P_Controller.P_Orientation = _charactersSpawnPos.forward;
            character.P_Controller.P_Position = _charactersSpawnPos.forward;

            return character;
        }

        // *****************************
        // RemoveCharacter 
        // *****************************
        void RemoveCharacter(ref ICharacterFacade _character)
        {
            if (_character == null)
            {
                return;
            }

            Debug.Log("Removing character...");
            _character.P_Controller.TogglePhysics(false);
            target.Value.RemoveCharacter(_character);
            _character = null;
        }

        // *****************************
        // ComplexTest 
        // *****************************
        async Task ComplexTest(CancellationToken _token)
        {
            CancellationTokenSource localCts = new();
            var simple = SimpleTest(localCts.Token);

            await Task.Delay((int)(characterRespawnTime * 1000f), _token);
            localCts.Cancel();

            if (_token.IsCancellationRequested)
            {
                return;
            }

            localCts = new();
            await SimpleTest(localCts.Token);
        }

        // *****************************
        // TestWrap 
        // *****************************
        async Task TestWrap(Task _test, CancellationToken _token)
        {
            try
            {
                await _test;
                Debug.Log("Test finished!");
            }
            catch (System.OperationCanceledException) { }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}