using MVCForum.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCForum.Domain.DomainModel.CMS
{
    public partial class CMSSettings : Entity
    {
        public CMSSettings()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public Guid? FrontPageCategory1 { get; set; }
        public Guid? FrontPageCategory2 { get; set; }
        public Guid? FrontPageCategory3 { get; set; }
        public Guid? FrontPageCategory4 { get; set; }
        public Guid? ArticleSticky1 { get; set; }
        public Guid? ArticleSticky2 { get; set; }
        public Guid? ArticleSticky3 { get; set; }
        public Guid? ArticleSticky4 { get; set; }


    }
}
