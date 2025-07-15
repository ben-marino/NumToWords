using System.ComponentModel.DataAnnotations;

namespace NumberToWords.Web.Models;

public class ConvertRequest
{
    [Required(ErrorMessage = "Input value is required")]
    public string Input { get; set; } = string.Empty;
    
    public ConversionOptionsDto? Options { get; set; }
}

public class ConversionOptionsDto
{
    public string? CurrencyName { get; set; }
    public string? SubCurrencyName { get; set; }
    public bool? UseAndConnector { get; set; }
    public string? Style { get; set; }
}