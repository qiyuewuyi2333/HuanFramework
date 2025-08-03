namespace Huan.Framework.Runtime.Base
{
    public abstract class Singleton<T> : object where T : new()
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new();
                }
                return _instance;
            }
        }
    }
}