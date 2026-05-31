namespace LibraryWebAPI.DTOs
{
    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string BookTitle { get; set; }
        public int Rating { get; set; }
        public required string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
