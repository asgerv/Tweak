using System;
using System.Collections.Generic;
using MVCForum.Domain.DomainModel;

namespace Tweak.Domain
{
    public class Article
    {
        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime DateModified { get; set; }
        public string Header { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }
        public ArticleType Type { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsPublished { get; set; }
        public virtual MembershipUser User { get; set; }
        public virtual List<ArticleTag> Tags { get; set; }
        public virtual List<ArticleComment> Comments { get; set; } 
    }
}