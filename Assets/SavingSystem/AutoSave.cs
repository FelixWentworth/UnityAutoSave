using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SavingSystem
{
    public class AutoSave : MonoBehaviour
    {
        public virtual void Awake()
        {
            SaveFile.AddBehaviourToSaveList(this);
        }

        public virtual void OnDestroy()
        {
            SaveFile.RemoveBehaviourFromSaveList(this);
        }

        public virtual IEnumerator ApplySaveData(Dictionary<string, string> data)
        {
            yield return null;
        }
    }
}