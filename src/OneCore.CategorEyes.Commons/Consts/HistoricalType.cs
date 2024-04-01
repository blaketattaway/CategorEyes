using System.ComponentModel.DataAnnotations;

namespace OneCore.CategorEyes.Commons.Consts
{
    public enum HistoricalType
    {
        [Display(Name = "Document Upload")]
        DocumentUpload = 1,
        [Display(Name = "IA")]
        IA = 2,
        [Display(Name = "User Interaction")]
        UserInteraction = 3,
    }
}
