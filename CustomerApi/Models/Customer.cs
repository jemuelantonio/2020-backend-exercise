namespace CustomerApi.Models
{
    public class Customer 
    {
        public int CustomerId { get;set; }
        public string Name { get;set; }
        public int Employees { get;set; }
        public string Tags { get;set; }
    }
}