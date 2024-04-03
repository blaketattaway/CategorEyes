namespace OneCore.CategorEyes.Commons.Consts
{
    public enum UserActions
    {
        EnterAnalysisPage = 1,
        EnterHistoricalPage = 2,
        FilterHistorical = 3,
        ExportHistorical = 4,
    }

    public static class UserActionsExtensions
    {
        public static string GetDescription(this UserActions userAction)
        {
            return userAction switch
            {
                UserActions.EnterAnalysisPage => "Entered the analysis page",
                UserActions.EnterHistoricalPage => "Entered the historical page",
                UserActions.FilterHistorical => "Filtered the historical",
                UserActions.ExportHistorical => "Exported the historical",
                _ => "Unknown action",
            };
        }
    }
}
