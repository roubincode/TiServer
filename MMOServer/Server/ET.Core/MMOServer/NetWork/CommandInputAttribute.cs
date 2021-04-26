using System;
namespace ETModel
{
    public class CommandInputAttribute : BaseAttribute
    {
        public Type Type;
        public CommandInputAttribute(Type type)
        {
            this.Type = type;
        }
    }
}