using System;

namespace DependencyInjectionContainerLibrary
{
    public class Implementation
    {
        public Type Type { get; set; }
        public bool IsSingleton { get; set; }
        public object Value { get; set; }

        public Implementation(Type type, bool isSingleton)
        {
            Type = type;
            IsSingleton = isSingleton;
        }
    }
}