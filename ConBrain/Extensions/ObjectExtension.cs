using System.Reflection;

namespace ConBrain.Extensions
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NonCopyAttribute : Attribute { }
    public static class ObjectExtension
    {
        public static void CopyTo<T>(this T source, T target)
        {
            var type = source.GetType();
            var properties = type.GetProperties();
            foreach ( var property in properties )
            {
                if (property.GetCustomAttribute<NonCopyAttribute>() == null && property.CanWrite)
                {
                    var value = property.GetValue(source);
                    if( value != null )
                        property.SetValue(target, value);
                }
                    
            }
        }
    }
}
