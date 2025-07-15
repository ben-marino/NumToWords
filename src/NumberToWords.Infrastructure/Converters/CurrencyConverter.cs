using NumberToWords.Domain;
using NumberToWords.Domain.Interfaces;
using System.Text;

namespace NumberToWords.Infrastructure.Converters;

public class CurrencyConverter : INumberToWordsConverter
{
    private static readonly string[] Ones = 
    { 
        "", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE" 
    };
    
    private static readonly string[] Teens = 
    { 
        "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", 
        "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN" 
    };
    
    private static readonly string[] Tens = 
    { 
        "", "", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY" 
    };
    
    public string Convert(decimal number, ConversionOptions? options = null)
    {
        options ??= new ConversionOptions();
        
        // TODO: Handle negative numbers
        if (number < 0)
        {
            throw new NotImplementedException("Negative numbers not yet supported");
        }
        
        // TODO: Handle numbers > 999,999
        if (number > 999999.99m)
        {
            throw new NotImplementedException("Numbers greater than 999,999.99 not yet supported");
        }
        
        var money = new MoneyAmount(number);
        var result = new StringBuilder();
        
        // Convert dollars part
        if (money.DollarPart > 0)
        {
            result.Append(ConvertWholeNumber(money.DollarPart));
            result.Append($" {options.CurrencyName}");
        }
        else
        {
            result.Append($"ZERO {options.CurrencyName}");
        }
        
        // Convert cents part
        if (options.UseAndConnector)
        {
            result.Append(" AND ");
        }
        else
        {
            result.Append(" ");
        }
        
        if (money.CentPart > 0)
        {
            result.Append(ConvertWholeNumber(money.CentPart));
            result.Append($" {options.SubCurrencyName}");
        }
        else
        {
            result.Append($"ZERO {options.SubCurrencyName}");
        }
        
        var finalResult = result.ToString();
        
        // Apply capitalization style
        return ApplyCapitalizationStyle(finalResult, options.Style);
    }
    
    private string ConvertWholeNumber(int number)
    {
        if (number == 0)
            return "ZERO";
            
        var parts = new List<string>();
        
        // Handle thousands
        if (number >= 1000)
        {
            int thousands = number / 1000;
            parts.Add(ConvertHundreds(thousands));
            parts.Add("THOUSAND");
            number %= 1000;
        }
        
        // Handle hundreds
        if (number > 0)
        {
            parts.Add(ConvertHundreds(number));
        }
        
        return string.Join(" ", parts.Where(p => !string.IsNullOrEmpty(p)));
    }
    
    private string ConvertHundreds(int number)
    {
        var parts = new List<string>();
        
        // Handle hundreds
        if (number >= 100)
        {
            int hundreds = number / 100;
            parts.Add(Ones[hundreds]);
            parts.Add("HUNDRED");
            number %= 100;
        }
        
        // Handle tens and ones
        if (number >= 20)
        {
            int tens = number / 10;
            parts.Add(Tens[tens]);
            number %= 10;
            
            if (number > 0)
            {
                parts.Add(Ones[number]);
            }
        }
        else if (number >= 10)
        {
            // Handle teens
            parts.Add(Teens[number - 10]);
        }
        else if (number > 0)
        {
            // Handle ones
            parts.Add(Ones[number]);
        }
        
        return string.Join(" ", parts.Where(p => !string.IsNullOrEmpty(p)));
    }
    
    private string ApplyCapitalizationStyle(string text, CapitalizationStyle style)
    {
        return style switch
        {
            CapitalizationStyle.AllUpper => text.ToUpper(),
            CapitalizationStyle.TitleCase => ToTitleCase(text),
            CapitalizationStyle.SentenceCase => ToSentenceCase(text),
            _ => text
        };
    }
    
    private string ToTitleCase(string text)
    {
        var words = text.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].Length > 0)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
            }
        }
        return string.Join(" ", words);
    }
    
    private string ToSentenceCase(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;
            
        return char.ToUpper(text[0]) + text.Substring(1).ToLower();
    }
}