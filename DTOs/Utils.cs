namespace ProductManagement.DTOs
{
    public class CustomResponseDto
    {
        public int StatusCode { get; set; }
        public bool IsError { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
    }
    public class JwtSettings
    {
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? Key { get; set; }
    }
}
