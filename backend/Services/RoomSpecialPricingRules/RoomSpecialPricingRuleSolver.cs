using ApartManBackend.Models.DbModels.Models;

namespace ApartManBackend.Services.RoomSpecialPricingRules
{
    public class RoomSpecialPricingRuleSolver
    {
        private readonly IReadOnlyDictionary<StaticMambers.Enums.SpecialPricingRuleType, IRoomSpecialPricingRuleCalculator> _calculators;

        public RoomSpecialPricingRuleSolver(IEnumerable<IRoomSpecialPricingRuleCalculator> calculators)
        {
            _calculators = calculators.ToDictionary(x => x.RuleType);
        }

        public decimal ApplyRules(Room room, IReadOnlyList<ReservationPerson> persons, DateOnly startDate, DateOnly endDate)
        {
            var nights = (endDate.DayNumber - startDate.DayNumber)-1;
            if (nights <= 0)
            {
                return 0;
            }

            var baseTotalPrice = persons.Sum(x => x.PricePerNight) * nights;
            var context = new ReservationPricingContext
            {
                Room = room,
                StartDate = startDate,
                EndDate = endDate,
                Persons = persons,
                BaseTotalPrice = baseTotalPrice
            };

            var rules = room.RoomSpecialPricingRules?
                .Where(x => x.Active)
                .OrderBy(x => x.Priority)
                .ThenBy(x => x.Id)
                .ToList() ?? [];

            var totalAdjustment = rules.Sum(rule =>
                _calculators.TryGetValue(rule.RuleType, out var calculator)
                    ? calculator.CalculateAdjustment(context, rule)
                    : 0);

            return Math.Max(0, baseTotalPrice + totalAdjustment);
        }
    }
}
