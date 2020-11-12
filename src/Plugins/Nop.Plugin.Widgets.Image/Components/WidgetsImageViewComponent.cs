using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.Image.Infrastructure.Cache;
using Nop.Plugin.Widgets.Image.Models;
using Nop.Services.Caching;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.Image.Components
{
    [ViewComponent(Name = "WidgetsImage")]
    public class WidgetsImageViewComponent : NopViewComponent
    {
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IStoreContext _storeContext;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ISettingService _settingService;
        private readonly IPictureService _pictureService;
        private readonly IWebHelper _webHelper;

        public WidgetsImageViewComponent(ICacheKeyService cacheKeyService,
            IStoreContext storeContext, 
            IStaticCacheManager staticCacheManager, 
            ISettingService settingService, 
            IPictureService pictureService,
            IWebHelper webHelper)
        {
            _cacheKeyService = cacheKeyService;
            _storeContext = storeContext;
            _staticCacheManager = staticCacheManager;
            _settingService = settingService;
            _pictureService = pictureService;
            _webHelper = webHelper;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var imageSettings = _settingService.LoadSetting<ImageSettings>(_storeContext.CurrentStore.Id);

            var model = new PublicInfoModel
            {
                Picture1Url = GetPictureUrl(imageSettings.Picture1Id),
                Text1 = imageSettings.Text1,
                Link1 = imageSettings.Link1,
                AltText1 = imageSettings.AltText1,

                Picture2Url = GetPictureUrl(imageSettings.Picture2Id),
                Text2 = imageSettings.Text2,
                Link2 = imageSettings.Link2,
                AltText2 = imageSettings.AltText2,

                Picture3Url = GetPictureUrl(imageSettings.Picture3Id),
                Text3 = imageSettings.Text3,
                Link3 = imageSettings.Link3,
                AltText3 = imageSettings.AltText3,

                Picture4Url = GetPictureUrl(imageSettings.Picture4Id),
                Text4 = imageSettings.Text4,
                Link4 = imageSettings.Link4,
                AltText4 = imageSettings.AltText4,

                Picture5Url = GetPictureUrl(imageSettings.Picture5Id),
                Text5 = imageSettings.Text5,
                Link5 = imageSettings.Link5,
                AltText5 = imageSettings.AltText5
            };

            if (string.IsNullOrEmpty(model.Picture1Url) && string.IsNullOrEmpty(model.Picture2Url) &&
                string.IsNullOrEmpty(model.Picture3Url) && string.IsNullOrEmpty(model.Picture4Url) &&
                string.IsNullOrEmpty(model.Picture5Url))
                //no pictures uploaded
                return Content("");

            return View("~/Plugins/Widgets.Image/Views/PublicInfo.cshtml", model);
        }

        protected string GetPictureUrl(int pictureId)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(ModelCacheEventConsumer.PICTURE_URL_MODEL_KEY, 
                pictureId, _webHelper.IsCurrentConnectionSecured() ? Uri.UriSchemeHttps : Uri.UriSchemeHttp);

            return _staticCacheManager.Get(cacheKey, () =>
            {
                //little hack here. nulls aren't cacheable so set it to ""
                var url = _pictureService.GetPictureUrl(pictureId, showDefaultPicture: false) ?? "";
                return url;
            });
        }
    }
}
