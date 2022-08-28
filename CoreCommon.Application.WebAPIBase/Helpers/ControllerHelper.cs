using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace CoreCommon.Application.WebAPIBase.Helpers
{
    public class ControllerHelper
    {
        public static string GetToken(HttpRequest request, string name)
        {
            request.Cookies.TryGetValue(name, out string token);
            if (string.IsNullOrEmpty(token))
            {
                StringValues values;
                if (request.Headers.TryGetValue(name, out values))
                {
                    token = values.ToArray().ToList().FirstOrDefault();
                }
            }

            return token;
        }
    }
}
