using System;
using Microsoft.AspNetCore.Server.Kestrel.Internal.System.Runtime;

namespace ucubot.Model
{
    public class CanNotParseSlackCommandStudent : Exception
    {
        public CanNotParseSlackCommandStudent(string command) : base($"Can not parse command {command}") { }
    }
}