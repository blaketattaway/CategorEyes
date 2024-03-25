using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
