using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCForum.Domain.DomainModel.Enums;
using MVCForum.Utilities;

namespace MVCForum.Domain.DomainModel.CMS
{
    public class ArticleCategory : Entity
    {
        public ArticleCategory()
        {
            Id = GuidComb.GenerateComb();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public string Slug { get; set; }
        public ArticleSection ArticleSection { get; set; }
        public virtual IList<Article> Articles { get; set; }
    }
}
