using System.Collections;
using UnityEngine;

namespace SavingSystem
{
    public abstract class LoadingComponent : MonoBehaviour
    {
        public abstract bool Loaded { get; set; }
        public abstract IEnumerator Load();
    }
}