using Domain.Entities;

namespace Application.Handlers
{
    internal static class CommonHandlerHelper
    {
        public static bool CheckTrialEndDateValue(DateOnly startDate, DateOnly? endDate, TrialStatus trialStatus, out string message)
        {
            message = "";

            if (!endDate.HasValue && trialStatus == TrialStatus.Completed)
            {
                message = "End date can't be null";
                return false;
            }
            if (endDate.HasValue && endDate < startDate)
            {
                message = "End date must be greated or equal as start date";
                return false;
            }

            return true;
        }
    }
}
