using System;
using System.Collections.Generic;
using MVCForum.Utilities;

namespace MVCForum.Domain.DomainModel.CMS
{
    public class ArticleTag : Entity
    {
        public ArticleTag()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public virtual IList<Article> Articles { get; set; }
    }
}