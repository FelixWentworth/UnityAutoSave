namespace SavingSystem
{
    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property,
                       AllowMultiple = true)  // multiuse attribute  
    ]
    public class SaveDataAttribute : System.Attribute
    {
        public SaveDataAttribute()
        {
        }
    }
}