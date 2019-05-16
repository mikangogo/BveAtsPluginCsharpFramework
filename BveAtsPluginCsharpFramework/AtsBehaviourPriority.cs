using System;

namespace AtsPlugin
{
    public class AtsBehaviourPriority : Attribute
    {
        public int Priority;


        public AtsBehaviourPriority(int priority)
        {
            Priority = priority;
        }
    }
}
