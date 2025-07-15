namespace NumberToWords.Domain;

public readonly struct MoneyAmount
{
    public decimal Value { get; }
    public int DollarPart { get; }
    public int CentPart { get; }
    
    public MoneyAmount(decimal value) 
    {
        Value = value;
        var absValue = Math.Abs(value);
        var dollars = (int)Math.Floor(absValue);
        var cents = (int)Math.Round((absValue - dollars) * 100);
        
        // Handle the case where rounding results in 100 cents
        if (cents >= 100)
        {
            dollars += cents / 100;
            cents = cents % 100;
        }
        
        DollarPart = dollars;
        CentPart = cents;
    }
}