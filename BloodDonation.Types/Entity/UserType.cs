namespace BloodDonation.Types.Entity
{
    public enum UserType : byte
    {
        Admin = 1, Hospital = 2, Donor = 3
    }

    public static class Extensions
    {
        public static string GetName(this UserType userType)
        {
            switch (userType)
            {
                case UserType.Admin:
                    return "Yönetici";
                case UserType.Hospital:
                    return "Hastane Kullanıcısı";
                case UserType.Donor:
                    return "Donör";
                default:
                    return "Geçersiz Kullanıcı";
            }
        }
    }
}
