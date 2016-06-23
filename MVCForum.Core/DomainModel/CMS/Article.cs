﻿using System;
using System.Collections.Generic;
using MVCForum.Utilities;

namespace MVCForum.Domain.DomainModel.CMS
{
    public class Article : Entity
    {
        public Article()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? DateModified { get; set; }
        public string Header { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }
        public string Image { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsPublished { get; set; }
        public virtual MembershipUser User { get; set; }
        public virtual IList<ArticleTag> Tags { get; set; }
        public virtual IList<ArticleComment> Comments { get; set; }
    }
}