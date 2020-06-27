using System;

namespace JamesOR.Edna.DataContainers
{
    [Serializable]
    public class ExposedProperty
    {
        public static ExposedProperty CreateInstance()
        {
            return new ExposedProperty();
        }

        public string PropertyName = "New String";
        public string PropertyValue = "New Value";
    }
}
