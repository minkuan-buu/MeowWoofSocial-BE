using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Repositories.GenericRepositories;
using MeowWoofSocial.Data.Repositories.PostAttachmentRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Data.Repositories.ReportRepositories
{
    public class ReportRepositories : GenericRepositories<Report>, IReportRepositories
    {
        public ReportRepositories(MeowWoofSocialContext context)
        : base(context)
        {
        }
    }
}
