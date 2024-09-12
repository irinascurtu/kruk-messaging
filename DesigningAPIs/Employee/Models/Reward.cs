using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.Models
{
    public class Reward
    {
        public int Id { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public string RewardName { get; set; }
        public string Description { get; set; }
        public DateTime DateGiven { get; set; }

        public Employee Employee { get; set; }
    }
}
