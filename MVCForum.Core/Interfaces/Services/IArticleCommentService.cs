﻿using System;
using System.Collections.Generic;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.CMS;

namespace MVCForum.Domain.Interfaces.Services
{
    public interface IArticleCommentService
    {
        ArticleComment Add(string commentBody, Guid? InReplyTo, Guid? ArticleId, MembershipUser user);
        void DeleteFromDb(ArticleComment articleComment);
        void Delete(Guid articleComment);
        void Delete(ArticleComment comment);
        void Update(ArticleComment articleComment);

        void UpdateBody(string newBody, Guid articleCommentId);
        IList<ArticleComment> GetByArticle(Guid articleId);

        ArticleComment GetComment(Guid? commentId);
        IEnumerable<ArticleComment> GetAll();
    }
}