using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RPG.Saving
{
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] string uniqueIdentifier = "";
        public static Dictionary<string, SaveableEntity> s_GUIDRegistry = new Dictionary<string, SaveableEntity>();

        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        private bool IsUnique(string guid)
        {
            if (!s_GUIDRegistry.ContainsKey(guid)) return true;
            if (s_GUIDRegistry[guid] == this) return true;

            if (s_GUIDRegistry[guid] == null)
            {
                s_GUIDRegistry.Remove(guid);
                return true;
            }

            if (s_GUIDRegistry[guid].GetUniqueIdentifier() != guid)
            {
                s_GUIDRegistry.Remove(guid);
                return true;
            }

            return false;
        }

        public object CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();

            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }

            return state;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> stateDict = (Dictionary<string, object>)state;

            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                string typeString = saveable.GetType().ToString();

                if (stateDict.ContainsKey(typeString)) 
                {
                    saveable.RestoreState(stateDict[typeString]);
                }
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.IsPlaying(gameObject)) return;
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");

            if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                property.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            s_GUIDRegistry[property.stringValue] = this;
        }
#endif
    }
}