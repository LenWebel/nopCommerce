using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Plugin.Widgets.Image.Models;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.Image.Controllers
{
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class WidgetsImageController : BasePluginController
    {
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerService _customerService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        public WidgetsImageController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            ISettingService settingService,
            IStoreContext storeContext, ICustomerService customerService)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _settingService = settingService;
            _storeContext = storeContext;
            _customerService = customerService;
        }

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var imageSettings = _settingService.LoadSetting<ImageSettings>(storeScope);
            var model = new ConfigurationModel
            {
                Picture1Id = imageSettings.Picture1Id,
                Text1 = imageSettings.Text1,
                Link1 = imageSettings.Link1,
                AltText1 = imageSettings.AltText1,
                Picture2Id = imageSettings.Picture2Id,
                Text2 = imageSettings.Text2,
                Link2 = imageSettings.Link2,
                AltText2 = imageSettings.AltText2,
                Picture3Id = imageSettings.Picture3Id,
                Text3 = imageSettings.Text3,
                Link3 = imageSettings.Link3,
                AltText3 = imageSettings.AltText3,
                Picture4Id = imageSettings.Picture4Id,
                Text4 = imageSettings.Text4,
                Link4 = imageSettings.Link4,
                AltText4 = imageSettings.AltText4,
                Picture5Id = imageSettings.Picture5Id,
                Text5 = imageSettings.Text5,
                Link5 = imageSettings.Link5,
                AltText5 = imageSettings.AltText5,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.Picture1Id_OverrideForStore = _settingService.SettingExists(imageSettings, x => x.Picture1Id, storeScope);
                model.Text1_OverrideForStore = _settingService.SettingExists(imageSettings, x => x.Text1, storeScope);
                model.Link1_OverrideForStore = _settingService.SettingExists(imageSettings, x => x.Link1, storeScope);
                model.AltText1_OverrideForStore = _settingService.SettingExists(imageSettings, x => x.AltText1, storeScope);
                model.Picture2Id_OverrideForStore = _settingService.SettingExists(imageSettings, x => x.Picture2Id, storeScope);
                model.Text2_OverrideForStore = _settingService.SettingExists(imageSettings, x => x.Text2, storeScope);
                model.Link2_OverrideForStore = _settingService.SettingExists(imageSettings, x => x.Link2, storeScope);
                model.AltText2_OverrideForStore = _settingService.SettingExists(imageSettings, x => x.AltText2, storeScope);
                model.Picture3Id_OverrideForStore = _settingService.SettingExists(imageSettings, x => x.Picture3Id, storeScope);
                model.Text3_OverrideForStore = _settingService.SettingExists(imageSettings, x => x.Text3, storeScope);
                model.Link3_OverrideForStore = _settingService.SettingExists(imageSettings, x => x.Link3, storeScope);
                model.AltText3_OverrideForStore = _settingService.SettingExists(imageSettings, x => x.AltText3, storeScope);
                model.Picture4Id_OverrideForStore = _settingService.SettingExists(imageSettings, x => x.Picture4Id, storeScope);
                model.Text4_OverrideForStore = _settingService.SettingExists(imageSettings, x => x.Text4, storeScope);
                model.Link4_OverrideForStore = _settingService.SettingExists(imageSettings, x => x.Link4, storeScope);
                model.AltText4_OverrideForStore = _settingService.SettingExists(imageSettings, x => x.AltText4, storeScope);
                model.Picture5Id_OverrideForStore = _settingService.SettingExists(imageSettings, x => x.Picture5Id, storeScope);
                model.Text5_OverrideForStore = _settingService.SettingExists(imageSettings, x => x.Text5, storeScope);
                model.Link5_OverrideForStore = _settingService.SettingExists(imageSettings, x => x.Link5, storeScope);
                model.AltText5_OverrideForStore = _settingService.SettingExists(imageSettings, x => x.AltText5, storeScope);
            }

            return View("~/Plugins/Widgets.Image/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var imageSettings = _settingService.LoadSetting<ImageSettings>(storeScope);

            //get previous picture identifiers
            var previousPictureIds = new[] {imageSettings.Picture1Id, imageSettings.Picture2Id, imageSettings.Picture3Id, imageSettings.Picture4Id, imageSettings.Picture5Id};

            imageSettings.Picture1Id = model.Picture1Id;
            imageSettings.Text1 = model.Text1;
            imageSettings.Link1 = model.Link1;
            imageSettings.AltText1 = model.AltText1;
            imageSettings.Picture2Id = model.Picture2Id;
            imageSettings.Text2 = model.Text2;
            imageSettings.Link2 = model.Link2;
            imageSettings.AltText2 = model.AltText2;
            imageSettings.Picture3Id = model.Picture3Id;
            imageSettings.Text3 = model.Text3;
            imageSettings.Link3 = model.Link3;
            imageSettings.AltText3 = model.AltText3;
            imageSettings.Picture4Id = model.Picture4Id;
            imageSettings.Text4 = model.Text4;
            imageSettings.Link4 = model.Link4;
            imageSettings.AltText4 = model.AltText4;
            imageSettings.Picture5Id = model.Picture5Id;
            imageSettings.Text5 = model.Text5;
            imageSettings.Link5 = model.Link5;
            imageSettings.AltText5 = model.AltText5;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(imageSettings, x => x.Picture1Id, model.Picture1Id_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(imageSettings, x => x.Text1, model.Text1_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(imageSettings, x => x.Link1, model.Link1_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(imageSettings, x => x.AltText1, model.AltText1_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(imageSettings, x => x.Picture2Id, model.Picture2Id_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(imageSettings, x => x.Text2, model.Text2_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(imageSettings, x => x.Link2, model.Link2_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(imageSettings, x => x.AltText2, model.AltText2_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(imageSettings, x => x.Picture3Id, model.Picture3Id_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(imageSettings, x => x.Text3, model.Text3_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(imageSettings, x => x.Link3, model.Link3_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(imageSettings, x => x.AltText3, model.AltText3_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(imageSettings, x => x.Picture4Id, model.Picture4Id_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(imageSettings, x => x.Text4, model.Text4_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(imageSettings, x => x.Link4, model.Link4_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(imageSettings, x => x.AltText4, model.AltText4_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(imageSettings, x => x.Picture5Id, model.Picture5Id_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(imageSettings, x => x.Text5, model.Text5_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(imageSettings, x => x.Link5, model.Link5_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(imageSettings, x => x.AltText5, model.AltText5_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            //get current picture identifiers
            var currentPictureIds = new[] {imageSettings.Picture1Id, imageSettings.Picture2Id, imageSettings.Picture3Id, imageSettings.Picture4Id, imageSettings.Picture5Id};

            //delete an old picture (if deleted or updated)
            foreach (var pictureId in previousPictureIds.Except(currentPictureIds))
            {
                var previousPicture = _pictureService.GetPictureById(pictureId);
                if (previousPicture != null)
                    _pictureService.DeletePicture(previousPicture);
            }

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }
    }
}