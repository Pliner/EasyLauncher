using System.IO;
using System.Text;

namespace Tests
{
    public static class TestHelpers
    {
        public static Stream ToMemoryStream(this string text)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(text));
        } 
    }
}