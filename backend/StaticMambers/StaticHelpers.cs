namespace ApartManBackend.StaticMambers
{
    public static class StaticHelpers
    {
        public static bool PatchPreConditionCheck<TSource>(TSource source, string destinationMemberName)
        {
            if (source == null)
                return false;

            var sourceProperty = source.GetType().GetProperty(destinationMemberName);
            if (sourceProperty == null)
                return true;

            var sourceValue = sourceProperty.GetValue(source);
            return PatchPreConditionCheck(sourceValue);
        }

        public static bool PatchPreConditionCheck<T>(T srcMember)
        {
            if (srcMember == null)
                return false;

            if (srcMember is string str)
                return !string.IsNullOrWhiteSpace(str);
            if (srcMember is DateOnly)
                return true;
            return true;
        }
    }
}
