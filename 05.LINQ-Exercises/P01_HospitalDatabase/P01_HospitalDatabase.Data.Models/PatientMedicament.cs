namespace P01_HospitalDatabase.Data.Models
{
    public class PatientMedicament
    {
        public Patient Patient { get; set; }
        public int PatientId { get; set; }

        public Medicament Medicament { get; set; }
        public int MedicamentId { get; set; }
    }
}
