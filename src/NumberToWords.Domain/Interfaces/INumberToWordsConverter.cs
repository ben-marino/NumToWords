namespace NumberToWords.Domain.Interfaces;

public interface INumberToWordsConverter
{
    string Convert(decimal number, ConversionOptions? options = null);
}