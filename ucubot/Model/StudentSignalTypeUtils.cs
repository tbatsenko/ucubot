namespace ucubot.Model
{
    
    public enum StudentSignalType
    {
        Simple = -1,
        Normal = 0,
        Hard = 1
    }

    public static class StudentSignalTypeUtils
    {
        public static StudentSignalType ConvertSlackMessageToStudentSignalType(this string message)
        {
            switch (message)
            {
                case "Simple":
                    return StudentSignalType.Simple;
                case "Normal":
                    return StudentSignalType.Normal;
                case "Hard":
                    return StudentSignalType.Hard;
                default:
                    throw new CanNotParseSlackCommandStudent(message);
            }
        }
    }
}