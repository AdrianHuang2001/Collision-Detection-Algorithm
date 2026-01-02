using System;

namespace CDA.Framework
{
    public class CDASingletonAttribute : Attribute
    {
        public Type[] Dependencies { get; }
        public int Priority { get; }
        public CDASingletonAttribute(int priority, params Type[] dependencies)
        {
            Priority = priority;
            Dependencies = dependencies;
        }
    }
}