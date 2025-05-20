using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace BookStore.Helper
{
    public static class TempDataHelper
    {
        public static string GetObjectString<T>(T model)
        {
            return JsonConvert.SerializeObject(model);
        }

        public static T GetObject<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
