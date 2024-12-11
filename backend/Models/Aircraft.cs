namespace backend.Models
{
    public class Aircraft
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public string Manufacturer { get; set; } = string.Empty;
        
        public string Model { get; set; } = string.Empty;
        
        public int CruiseSpeed { get; set; }
        
        public int Range { get; set; }
        
        public int MaxCrosswind { get; set; }
    }
}