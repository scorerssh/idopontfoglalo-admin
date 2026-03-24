namespace ApartManBackend.StaticMambers
{
    public static class StaticHelpers
    {
        public static bool PatchPreConditionCheck<T>(T srcMember)
        {
            if (srcMember == null)
                return false;

            if (srcMember is string str)
                return !string.IsNullOrWhiteSpace(str);

            return true;
        }
    }
}
