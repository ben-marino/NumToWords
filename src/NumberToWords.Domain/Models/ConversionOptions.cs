namespace NumberToWords.Domain;

public class ConversionOptions
{
    public string CurrencyName { get; init; } = "DOLLARS";
    public string SubCurrencyName { get; init; } = "CENTS";
    public bool UseAndConnector { get; init; } = true;
    public CapitalizationStyle Style { get; init; } = CapitalizationStyle.AllUpper;
}

public enum CapitalizationStyle 
{ 
    AllUpper, 
    TitleCase, 
    SentenceCase 
}