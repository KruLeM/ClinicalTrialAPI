using Domain.Entities;
using System.Reflection.Metadata.Ecma335;

namespace Application.Handlers
{
    internal static class CommonHandlerHelper
    {
        public static bool CheckTrialEndDateValue(DateOnly startDate, DateOnly? endDate, TrialStatus trialStatus, out string message)
        {
            message = string.Empty;

            if (!endDate.HasValue)
            {
                if (trialStatus == TrialStatus.Completed)
                {
                    message = "End date is required for completed trials.";
                    return false;
                }
                return true;
            }

            if (endDate.Value < startDate)
            {
                message = "End date must be greater than or equal to start date.";
                return false;
            }

            return true;
        }

        public static bool CheckTrialIdLength(string trialId, out string message)
        {
            message = string.Empty;

            if (string.IsNullOrWhiteSpace(trialId))
            {
                message = "TrialId cannot be empty.";
                return false;
            }

            if (trialId.Length > 450)
            {
                message = $"TrialId exceeds the maximum length of 450 characters (current: {trialId.Length}).";
                return false;
            }

            return true;
        }
    }
}
