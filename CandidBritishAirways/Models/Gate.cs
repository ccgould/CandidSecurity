using CandidBritishAirways.Enumerator;

namespace CandidBritishAirways.Models
{
    public class Gate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Terminal Terminal { get; set; }
    }
}
