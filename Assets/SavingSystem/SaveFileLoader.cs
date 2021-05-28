using System.Collections;

namespace SavingSystem
{
    public class SaveFileLoader : LoadingComponent
    {
        public override bool Loaded { get; set; }

        public override IEnumerator Load()
        {
            SaveFile.Init();
            while (!SaveFile.Loaded)
            {
                yield return null;
            }
            Loaded = true;
        }
    }
}