using System;
using System.Collections.Generic;
using System.Reflection;

namespace YG
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InitBaisYGAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    public class InitYGAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    public class StartYGAttribute : Attribute { }

    public partial class YandexGame
    {
        private static List<Action> methodsInitBaisToCall = new List<Action>();
        private static List<Action> methodsInitToCall = new List<Action>();
        private static List<Action> methodsStartToCall = new List<Action>();

        public static void CallInitBaisYG()
        {
            Type type = typeof(YandexGame);
            MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            foreach (MethodInfo method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(InitBaisYGAttribute), true);
                if (attributes.Length > 0)
                {
                    methodsInitBaisToCall.Add(() => method.Invoke(type, null));
                }
            }
            foreach (var action in methodsInitBaisToCall)
            {
                action.Invoke();
            }
        }

        public static void CallInitYG()
        {
            Type type = typeof(YandexGame);
            MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            foreach (MethodInfo method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(InitYGAttribute), true);
                if (attributes.Length > 0)
                {
                    methodsInitToCall.Add(() => method.Invoke(type, null));
                }
            }
            foreach (var action in methodsInitToCall)
            {
                action.Invoke();
            }
        }

        public static void CallStartYG()
        {
            Type type = typeof(YandexGame);
            MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            foreach (MethodInfo method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(StartYGAttribute), true);
                if (attributes.Length > 0)
                {
                    methodsStartToCall.Add(() => method.Invoke(type, null));
                }
            }
            foreach (var action in methodsStartToCall)
            {
                action.Invoke();
            }
        }
    }
}