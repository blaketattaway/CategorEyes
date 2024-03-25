﻿using OneCore.CategorEyes.Commons.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCore.CategorEyes.Business.Report
{
    public class ReportBusiness : IReportBusiness
    {
        //public string CreateExcel()
        //{

        //}

        //private string GenerateExcel(string guid)
        //{
        //    string fileName;
        //    byte[] responseService;


        //}

        public ReportStatusResponse GetReportStatus(string guid)
        {
            ReportStatusResponse reportStatusResponse = new ReportStatusResponse();
            reportStatusResponse.Guid = guid;
            reportStatusResponse.Ready = false;
            reportStatusResponse.Progress = "0%";
            reportStatusResponse.HasError = false;
            reportStatusResponse.ErrorDescription = string.Empty;
            reportStatusResponse.Url = string.Empty;

            return reportStatusResponse;
        }
    }
}
