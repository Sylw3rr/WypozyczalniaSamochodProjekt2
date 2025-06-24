using System.Text.RegularExpressions;

namespace CarRentalSystem.Utils
{
    public static class DataValidator
    {
        public static bool IsValidEmail(string email) => !string.IsNullOrWhiteSpace(email) && Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        public static bool IsValidYear(int year) => year > 1990 && year <= DateTime.Now.Year + 1;
        public static bool IsValidDailyRate(decimal rate) => rate > 0;
    }
}