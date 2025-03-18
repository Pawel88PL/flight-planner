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
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }

    public class PagedAircrafts
    {
        public int TotalRecords { get; set; }
        public List<Aircraft> Data { get; set; } = new List<Aircraft>();
    }
}