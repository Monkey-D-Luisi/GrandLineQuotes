namespace Application.Common.IoC
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class IocAttribute : Attribute
    {


        public Type ServiceType { get; }


        public IocAttribute(Type serviceType)
        {
            ServiceType = serviceType;
        }
    }

}
