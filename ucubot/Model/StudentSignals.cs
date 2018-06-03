namespace ucubot.Model
{
    public class StudentSignals
    {

//        SignalType: string, allowed values - [Simple, Normal, Hard],
        
        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public StudentSignalType Type { get; set; }
       
        public int Count { get; set; }
        
    }
}
