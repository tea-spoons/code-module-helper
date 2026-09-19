
namespace TeaSpoons.CodeModuleHelper
{
    using System;

    /// <summary>
    /// Can be used to decorate a class as "unfinished", meaning that it is not intended to be left on the main branch or even production without further development.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum)]
    public class UnfinishedAttribute : Attribute
    {
        public CodeStatus Status { get; private set; }

        public UnfinishedAttribute(CodeStatus status)
        {
            Status = status;
        }
    }
}
