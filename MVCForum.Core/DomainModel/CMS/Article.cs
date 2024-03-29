﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public DateTime PublishDate { get; set; }
        public string Header { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "ntext")]		
        [MaxLength]
        public string Body { get; set; }
        public string Image { get; set; }
        public string Slug { get; set; }
        public int Views { get; set; }
        public bool IsPublished { get; set; }
        public virtual MembershipUser User { get; set; }
        public virtual ArticleCategory ArticleCategory { get; set; }
        public virtual IList<ArticleTag> Tags { get; set; }
        public virtual IList<ArticleComment> Comments { get; set; }
    }
}