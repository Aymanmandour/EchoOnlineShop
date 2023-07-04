using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace EchoOnlineShop.Utilities
{
    public static class SessionExtentions
    {
        public static void Set<T>(this ISession session,string Key, T value)
        {
            session.SetString(Key, JsonSerializer.Serialize(value));
        }
        public static T Get<T>(this ISession session, string Key)
        {
            var value = session.GetString(Key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
