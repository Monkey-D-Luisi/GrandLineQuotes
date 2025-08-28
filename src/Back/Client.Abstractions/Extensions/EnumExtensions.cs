using System.ComponentModel.DataAnnotations;
using System.Reflection;

public static class EnumExtensions
{


    public static string GetDisplayName(this Enum value)
    {
        var memberInfo = value.GetType()
                              .GetMember(value.ToString())
                              .FirstOrDefault();

        if (memberInfo is not null)
        {
            var displayAttribute = memberInfo.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute is not null)
                return displayAttribute.GetName() ?? value.ToString();
        }

        return value.ToString();
    }
}
