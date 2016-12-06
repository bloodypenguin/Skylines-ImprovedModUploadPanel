using System;
using System.Linq;
using System.Reflection;

namespace ImprovedModUploadPanel
{

    public static class Util
    {
        public static FieldInfo FindField<T>(T o, string fieldName)
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            return fields.FirstOrDefault(f => f.Name == fieldName);
        }

        public static T GetFieldValue<T>(FieldInfo field, object o)
        {
            return (T)field.GetValue(o);
        }
    }

}
