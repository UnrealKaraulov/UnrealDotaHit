using System;

namespace Deerchao.War3Share.W3gParser
{
    public class W3gParserException : Exception
    {
        public W3gParserException(string message)
            : base(message)
        { }

        public W3gParserException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}