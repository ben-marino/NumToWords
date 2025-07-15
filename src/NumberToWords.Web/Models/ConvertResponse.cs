namespace NumberToWords.Web.Models;

public class ConvertResponse
{
    public bool IsSuccess { get; set; }
    public string? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public decimal? OriginalValue { get; set; }
    public long ProcessingTimeMs { get; set; }
}