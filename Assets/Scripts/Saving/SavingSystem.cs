using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        public IEnumerator LoadLastScene(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);

            if (state.ContainsKey("LastSceneBuildInex"))
            {
                int sceneIndex = (int)state["LastSceneBuildInex"];

                if (sceneIndex != SceneManager.GetActiveScene().buildIndex)
                {
                    yield return SceneManager.LoadSceneAsync(sceneIndex);
                }
            }
            
            RestoreState(state);
        }

        public void Save(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile, state);
        }

        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);

            if (!File.Exists(path))
            {
                Debug.Log("No Save Found, Creating New Save: " + path);
                return new Dictionary<string, object>();
            }

            Debug.Log("Loading from: " + path);

           using (FileStream stream = File.Open(path, FileMode.Open))
           {
               BinaryFormatter formatter = new BinaryFormatter();

               return (Dictionary<string, object>)formatter.Deserialize(stream);
           }
        }

        private void SaveFile(string saveFile, object captureState)
        {
            string path = GetPathFromSaveFile(saveFile);
            Debug.Log("Saving to: " + path);

            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, captureState);
            }
        }

        private void CaptureState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }

            state["LastSceneBuildInex"] = SceneManager.GetActiveScene().buildIndex;
        }

        private void RestoreState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                string id = saveable.GetUniqueIdentifier();

                if (!state.ContainsKey(id)) continue;

                saveable.RestoreState(state[id]);
            }
        }

        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }
    }
}