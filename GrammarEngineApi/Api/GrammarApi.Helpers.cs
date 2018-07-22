using System;
using System.Text;

namespace GrammarEngineApi.Api
{
    public sealed partial class GrammarApi
    {
        public static string GetCString(Action<StringBuilder> action)
        {
            var builder = new StringBuilder(32);
            action(builder);
            return builder.ToString();
        }
    }
}