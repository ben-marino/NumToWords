namespace NumberToWords.Domain;

public readonly struct MoneyAmount
{
    public decimal Value { get; }
    public int DollarPart { get; }
    public int CentPart { get; }
    
    public MoneyAmount(decimal value) 
    {
        Value = value;
        DollarPart = (int)Math.Floor(Math.Abs(value));
        CentPart = (int)Math.Round((Math.Abs(value) - DollarPart) * 100);
    }
}