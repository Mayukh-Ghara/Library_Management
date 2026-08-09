namespace LibraryWebAPI.DTOs
{
    public class BorrowingResponseDto
    {
        public int Id { get; set; }
        public required string BookTitle { get; set; }
        public required string Username { get; set; }
        public DateTime BorrowedAt { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public required string Status { get; set; }
    }
}
