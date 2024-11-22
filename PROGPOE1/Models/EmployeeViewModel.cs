using System.ComponentModel;

namespace PROGPOE1.Models
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [DisplayName("Date Of Birth")]
        public DateTime DateOfBirth { get; set; }

        [DisplayName("E-mail")]
        public string Email { get; set; }

        [DisplayName("Hours Worked")]
        public double HoursWorked { get; set; }

        [DisplayName("Hourly Rate")]
        public double HourlyRate { get; set; }

        [DisplayName("Salary")]
        public double Salary { get; set; }

        public string Status { get; set; } // Add Status here

        [DisplayName("Name")]
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
    }
}
