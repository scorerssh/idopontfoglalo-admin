using static ApartManBackend.StaticMambers.Enums;

namespace ApartManBackend.StaticMambers
{
    public static class SpecialPricingRuleDescriptions
    {
        public static string GetDescription(SpecialPricingRuleType ruleType)
        {
            return ruleType switch
            {
                SpecialPricingRuleType.OneNightSurcharge =>
                    "Ha a vendég csak egy éjszakára foglal, akkor az alap foglalási ár 30%-kal emelkedik.",
                SpecialPricingRuleType.WeekendSurcharge =>
                    "A pénteki és szombati éjszakákra 20% hétvégi felár kerül felszámításra.",
                SpecialPricingRuleType.HolidaySurcharge =>
                    "A magyar munkaszüneti ünnepnapokra eső éjszakákra 25% ünnepnapi felár kerül felszámításra.",
                SpecialPricingRuleType.LongStayDiscount =>
                    "Ha a foglalás legalább 7 éjszakára szól, akkor az alap foglalási árból 10% kedvezmény jár.",
                _ => "Ismeretlen speciális árazási szabály."
            };
        }
    }
}
