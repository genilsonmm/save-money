using System.Collections.Generic;

namespace SaveMoney.Util
{
    public class ManagerVariables
    {
        private static ManagerVariables instance;
        private Dictionary<string, object> parameters = new Dictionary<string, object>();

        public static ManagerVariables Instance()
        {
            if (instance == null)
                instance = new ManagerVariables();
            return instance;
        }

        public void Add(string key, object param)
        {
            parameters.Add(key, param);
        }

        public object Get(string key)
        {
            object temp = parameters[key];
            parameters.Remove(key);
            return temp;
        }
    }
}
