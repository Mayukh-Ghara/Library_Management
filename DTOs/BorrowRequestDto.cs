namespace LibraryWebAPI.DTOs
{
    public class BorrowRequestDto
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
        public int BorrowDays { get; set; } = 14;
    }
}
