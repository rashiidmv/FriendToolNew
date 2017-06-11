using Microsoft.Practices.Unity;

namespace Friend.Infra
{
    public static class UnityExtention
    {
        public static void RegisterForNavigation<T>(this IUnityContainer uc)
        {
            uc.RegisterType(typeof(object), typeof(T), typeof(T).FullName);
        }
    }
}
