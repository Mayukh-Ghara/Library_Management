using Microsoft.EntityFrameworkCore;

namespace LibraryWebAPI.Models
{
    [Keyless]
    public class BookCount
    {
        public int Count { get; set; }
    }
}