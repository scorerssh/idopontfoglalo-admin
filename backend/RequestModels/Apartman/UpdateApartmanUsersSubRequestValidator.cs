using ApartManBackend.Services;
using FluentValidation;

namespace ApartManBackend.RequestModels.Apartman
{
    public class UpdateApartmanUsersSubRequestValidator : AbstractValidator<UpdateApartmanUsersSubRequest>
    {
        public UpdateApartmanUsersSubRequestValidator(UserService userService)
        {
            RuleFor(x => x)
                .Must(x => x.UserIdsToAdd != null || x.UserIdsToRemove != null)
                .WithMessage("Legalabb egy felhasznalo lista megadasa kotelezo.");

            RuleFor(x => x)
                .Must(x => !HasDuplicates(x.UserIdsToAdd))
                .WithMessage("A hozzaadando felhasznalok listaja nem tartalmazhat duplikalt azonosito(ka)t.");

            RuleFor(x => x)
                .Must(x => !HasDuplicates(x.UserIdsToRemove))
                .WithMessage("Az eltavolitando felhasznalok listaja nem tartalmazhat duplikalt azonosito(ka)t.");

            RuleFor(x => x)
                .Must(x => !HasOverlappingUserIds(x))
                .WithMessage("Ugyanaz a felhasznalo nem szerepelhet egyszerre a hozzaadando es az eltavolitando listaban.");

            When(x => x.UserIdsToAdd != null, () =>
            {
                RuleFor(x => x.UserIdsToAdd)
                    .Must(ids => ids!.Count > 0)
                    .WithMessage("A hozzaadando felhasznalok listaja nem lehet ures.")
                    .Must(ids => ids!.All(id => id > 0))
                    .WithMessage("A hozzaadando felhasznalo azonosito(k)nak nagyobbnak kell lenniuk 0-nal.")
                    .MustAsync(async (ids, ct) => await userService.CheckUsersExistAsync(ids!, ct))
                    .WithMessage("A hozzaadando felhasznalok kozott nem letezo felhasznalo is szerepel.");
            });

            When(x => x.UserIdsToRemove != null, () =>
            {
                RuleFor(x => x.UserIdsToRemove)
                    .Must(ids => ids!.Count > 0)
                    .WithMessage("Az eltavolitando felhasznalok listaja nem lehet ures.")
                    .Must(ids => ids!.All(id => id > 0))
                    .WithMessage("Az eltavolitando felhasznalo azonosito(k)nak nagyobbnak kell lenniuk 0-nal.")
                    .MustAsync(async (ids, ct) => await userService.CheckUsersExistAsync(ids!, ct))
                    .WithMessage("Az eltavolitando felhasznalok kozott nem letezo felhasznalo is szerepel.");
            });
        }

        private static bool HasDuplicates(IEnumerable<int>? ids)
        {
            if (ids == null)
            {
                return false;
            }

            var idsList = ids.ToList();
            return idsList.Count != idsList.Distinct().Count();
        }

        private static bool HasOverlappingUserIds(UpdateApartmanUsersSubRequest request)
        {
            if (request.UserIdsToAdd == null || request.UserIdsToRemove == null)
            {
                return false;
            }

            var idsToRemove = request.UserIdsToRemove.ToHashSet();
            return request.UserIdsToAdd.Any(idsToRemove.Contains);
        }
    }
}
