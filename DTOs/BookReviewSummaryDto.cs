namespace LibraryWebAPI.DTOs
{
    public class BookReviewSummaryDto
    {
        public required string BookTitle { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public List<ReviewResponseDto> Reviews { get; set; }
    }
}
