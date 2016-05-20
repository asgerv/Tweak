using System;
using System.Collections.Generic;

namespace Tweak.Domain
{
    public class Article
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string Author { get; set; }
        public string Header { get; set; }
        public string Description { get; set; }
        public string Body { get; set; }
        public ArticleType Type { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsPublished { get; set; }
        public virtual List<Tag> Tags { get; set; }
        public string[] Comments { get; set; }
    }
}