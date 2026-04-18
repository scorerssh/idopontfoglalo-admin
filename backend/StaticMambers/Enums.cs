namespace ApartManBackend.StaticMambers
{
    public static class Enums
    {
        public enum UserRole
        {
            Admin,
            User
        }

        public enum  ResourceObjectType
        {
            Apartman,
            Room,
            RoomPriceTier,
            AgePriceTier,
            Reservation,
            RoomSpecialPricingRule,
        }

        public enum ReservationSource
        {
            Website,
            BookingCom,
            SzallasHu
        }

        public enum SpecialPricingRuleType
        {
            OneNightSurcharge,
            WeekendSurcharge,
            HolidaySurcharge,
            LongStayDiscount,
        }

    }
}
