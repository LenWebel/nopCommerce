using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.Image;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.Image
{
    /// <summary>
    /// PLugin
    /// </summary>
    public class ImagePlugin : BasePlugin, IWidgetPlugin
    {
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly INopFileProvider _fileProvider;

        public ImagePlugin(ILocalizationService localizationService,
            IPictureService pictureService,
            ISettingService settingService,
            IWebHelper webHelper,
            INopFileProvider fileProvider)
        {
            _localizationService = localizationService;
            _pictureService = pictureService;
            _settingService = settingService;
            _webHelper = webHelper;
            _fileProvider = fileProvider;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string> { "home_main_image" };
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/WidgetsImage/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsImage";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //pictures
            var sampleImagesPath = _fileProvider.MapPath("~/Plugins/Widgets.Image/Content/nivoslider/sample-images/");

            //settings
            var settings = new ImageSettings
            {
                Picture1Id = _pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "banner1.jpg")), MimeTypes.ImagePJpeg, "banner_1").Id,
                Text1 = "",
                Link1 = _webHelper.GetStoreLocation(false),
                Picture2Id = _pictureService.InsertPicture(_fileProvider.ReadAllBytes(_fileProvider.Combine(sampleImagesPath, "banner2.jpg")), MimeTypes.ImagePJpeg, "banner_2").Id,
                Text2 = "",
                Link2 = _webHelper.GetStoreLocation(false)
                //Picture3Id = _pictureService.InsertPicture(File.ReadAllBytes(_fileProvider.Combine(sampleImagesPath,"banner3.jpg")), MimeTypes.ImagePJpeg, "banner_3").Id,
                //Text3 = "",
                //Link3 = _webHelper.GetStoreLocation(false),
            };
            _settingService.SaveSetting(settings);

            _localizationService.AddPluginLocaleResource(new Dictionary<string, string>
            {
                ["Plugins.Widgets.Image.Picture1"] = "Picture 1",
                ["Plugins.Widgets.Image.Picture2"] = "Picture 2",
                ["Plugins.Widgets.Image.Picture3"] = "Picture 3",
                ["Plugins.Widgets.Image.Picture4"] = "Picture 4",
                ["Plugins.Widgets.Image.Picture5"] = "Picture 5",
                ["Plugins.Widgets.Image.Picture"] = "Picture",
                ["Plugins.Widgets.Image.Picture.Hint"] = "Upload picture.",
                ["Plugins.Widgets.Image.Text"] = "Comment",
                ["Plugins.Widgets.Image.Text.Hint"] = "Enter comment for picture. Leave empty if you don't want to display any text.",
                ["Plugins.Widgets.Image.Link"] = "URL",
                ["Plugins.Widgets.Image.Link.Hint"] = "Enter URL. Leave empty if you don't want this picture to be clickable.",
                ["Plugins.Widgets.Image.AltText"] = "Image alternate text",
                ["Plugins.Widgets.Image.AltText.Hint"] = "Enter alternate text that will be added to image."
            });

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<ImageSettings>();

            //locales
            _localizationService.DeletePluginLocaleResources("Plugins.Widgets.Image");

            base.Uninstall();
        }

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => false;
    }
}