namespace MeowWoofSocial.Data.DTO.Custom
{
    public class CustomException : Exception
    {
        public CustomException(string message) : base(message) { }
    }
}