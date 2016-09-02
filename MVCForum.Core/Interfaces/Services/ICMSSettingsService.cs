using System;
using System.Collections.Generic;
using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.CMS;

namespace MVCForum.Domain.Interfaces.Services
{
    public interface ICMSSettingsService
    {
        CMSSettings GetOrCreate();
        void Edit(CMSSettings settings);


    }
}
