using System.Collections;
using System.Collections.Generic;
using Editor.CodeGen.ReferenceDb;
using GDTUtils.Extensions;
using UnityCodeGen;
using UnityEditor;
using UnityEngine;

namespace Editor.ReferenceDb
{
    [CustomEditor(typeof(Modules.ReferenceDb.ReferenceDb))]
    public class ReferenceDbEditor : UnityEditor.Editor
    {
        private Modules.ReferenceDb.ReferenceDb reference;
        
        
        // *****************************
        // OnInspectorGUI 
        // *****************************
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            reference = target as Modules.ReferenceDb.ReferenceDb;
            
            bool setupDatabaseIsPressed = GUILayout.Button("Setup");
            if (setupDatabaseIsPressed)
            {
                OnSetupDbStarted();
            }
        }

        void OnSetupDbStarted()
        {
            SetupAssets();
            GenerateAliasesConfig();
        }

        void SetupAssets()
        {
            int lastId = -1;
            reference.state.entriesList.categories.ForEach(category =>
            {
                category.entries.ForEach(entryContainer =>
                {
                    entryContainer.entry.SetId(++lastId);
                    entryContainer.entry.SetAlias(entryContainer.alias);
                    
                    EditorUtility.SetDirty(entryContainer.entry);
                });
            });
            
            // save all //
            AssetDatabase.SaveAssets();
        }
        
        void GenerateAliasesConfig()
        {
            // TODO: make separate utility if there will be other generators
            // TODO: make path and file name as settings
            AliasesConfigGenerator.entriesPrefab = reference.state.entriesList;
            AliasesConfigGenerator.fileName      = "ReferenceDbAliasesConfig";
            AliasesConfigGenerator.outputPath    = "Assets/Scripts/Modules/ReferenceDb/Config";

            UnityCodeGenUtility.Generate();
        }
    }
}