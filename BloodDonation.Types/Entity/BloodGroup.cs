namespace BloodDonation.Types.Entity
{
    public enum BloodGroup : byte
    {
        ARhPositive = 1,
        ARhNegative = 2,
        BRhPositive = 3,
        BRhNegative = 4,
        ABRhPositive = 5,
        ABRhNegative = 6,
        ORhPositive = 7,
        ORhNegative = 8
    }

    public static class BloodGroupExtensions
    {
        public static string GetName(this BloodGroup bloodGroup)
        {
            return bloodGroup.ToString().Replace("Rh", " Rh").Replace("Positive", "+").Replace("Negative", "-");
        }
    }
}
