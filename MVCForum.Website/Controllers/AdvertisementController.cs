using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Domain.Interfaces.UnitOfWork;
using MVCForum.Website.ViewModels;

namespace MVCForum.Website.Controllers
{
    public class AdvertisementController : BaseController
    {

        public PartialViewResult _RenderAd(int i)
        {
            var ad = new AdViewModel();
            ad.zone = i;
            switch (i)
            {
                case 5:
                case 6:
                case 7:
                    ad.width = 300;
                    ad.height = 250;
                    break;
                case 1:
                case 2:
                case 3:
                case 4:
                    ad.width = 930;
                    ad.height = 180;
                    break;
            }
            return PartialView(ad);
        }


        public AdvertisementController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService) : base(loggingService, unitOfWorkManager, membershipService, localizationService, roleService, settingsService)
        {
        }
    }
}