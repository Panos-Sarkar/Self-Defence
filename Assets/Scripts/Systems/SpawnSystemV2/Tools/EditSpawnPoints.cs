﻿using UnityEngine;

namespace SelfDef.Systems.SpawnSystemV2.Tools
{
    [RequireComponent(typeof(SpawnInitializer))]
    public class EditSpawnPoints : MonoBehaviour
    {
        [HideInInspector]
        public LevelSpawnData data;
        
        public bool autoLockInspector;
        
        public Mesh positionItemMesh;
        public Material positionItemMaterial;

        [Range(-2,2)]
        public float labelVerticalOffset = 1;
        public GUIStyle labelStyle = new GUIStyle();
        
        [HideInInspector]
        public bool createPositionalObjects;
        [HideInInspector]
        public bool updatePositionalObjects;
        [HideInInspector]
        public bool deletePositionalObjects;
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            data = GetComponent<SpawnInitializer>().GetLevelData();
        }

        public void UpdateData()
        {
            data = GetComponent<SpawnInitializer>().GetLevelData();
        }
#endif
    }
}
