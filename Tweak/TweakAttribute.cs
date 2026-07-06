using System;

namespace OutPathOptionsMod.Tweaks
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class TweakAttribute : Attribute
    {
        public string Name;

        public string Category;

        public int ID = 0;
    }
}
