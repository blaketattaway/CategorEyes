namespace OneCore.CategorEyes.Commons.Consts
{
    public enum UserAction
    {
        EnterAnalysisPage = 1,
        EnterHistoricalPage = 2,
        FilterHistorical = 3,
        ExportHistorical = 4,
    }

    public static class UserActionsExtensions
    {
        public static string GetDescription(this UserAction userAction)
        {
            return userAction switch
            {
                UserAction.EnterAnalysisPage => "Entered the analysis page",
                UserAction.EnterHistoricalPage => "Entered the historical page",
                UserAction.FilterHistorical => "Filtered the historical",
                UserAction.ExportHistorical => "Exported the historical",
                _ => "Unknown action",
            };
        }
    }
}
