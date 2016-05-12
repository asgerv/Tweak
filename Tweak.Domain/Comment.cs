using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.Entities;

namespace Tweak.Domain
{
    class Comment
    {
        public int Id { get; set; }
        public MembershipUser User { get; set; }

    }
}
