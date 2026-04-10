namespace PetCareManagementSystem.Models
{
    /// <summary>
    /// Represents a consumable supply item (e.g., pet food, medication)
    /// </summary>
    public class Supply
    {
        public string Name { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int DurationDays { get; set; }
    }
}