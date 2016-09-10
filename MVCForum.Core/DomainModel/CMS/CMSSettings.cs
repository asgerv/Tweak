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

        public virtual Article StickyArticle1 { get; set; }
        public virtual Article StickyArticle2 { get; set; }
        public virtual Article StickyArticle3 { get; set; }
        public virtual Article StickyArticle4 { get; set; }

        public virtual IList<ArticleTag> StickyTags { get; set; }


    }
}
