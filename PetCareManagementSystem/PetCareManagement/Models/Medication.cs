namespace PetCareManagementSystem.Models
{
    /// <summary>
    /// Represents a medication or treatment administered to a pet.
    /// Tracks the medication name, dosage, frequency, start date,
    /// end date, and any additional notes from the vet.
    /// </summary>
    public class Medication
    {
        public string Id { get; set; }

        public string PetId { get; set; }

        public string Name { get; set; }

        public string Dosage { get; set; }

        public string Frequency { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Notes { get; set; }

        /// <summary>
        /// Returns true if the medication course is still active today.
        /// A medication is considered active if it has no end date or the end date has not passed.
        /// </summary>
        public bool IsActive
        {
            get
            {
                if (EndDate == null)
                    return true;

                return EndDate >= DateTime.Today;
            }
        }
    }
}
