﻿namespace VkCelebrationApp.BLL.Extensions
{
    public static class ObjectExtensions
    {
        public static object GetPropValue(this object obj, string propName)
        {
            var nameParts = propName.Split('.');
            if (nameParts.Length == 1)
            {
                return obj.GetType().GetProperty(propName).GetValue(obj, null);
            }

            foreach (var part in nameParts)
            {
                if (obj == null) { return null; }

                var type = obj.GetType();
                var info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }
    }
}
