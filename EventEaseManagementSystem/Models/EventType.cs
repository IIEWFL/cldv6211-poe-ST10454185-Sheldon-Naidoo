using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventEaseManagementSystem.Models
{
    public class EventType
    {
        [Key]
        public int EventTypeId { get; set; }

        [Required] 
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        public ICollection<Event>? Events { get; set; }
    }
}
