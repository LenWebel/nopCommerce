﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nop.Core;
using Nop.Plugin.ExternalAuth.ExtendedAuth.Domain;
using Nop.Plugin.ExternalAuth.ExtendedAuthentication.Infrastructure;
using Nop.Plugin.ExternalAuth.ExtendedAuthentication.Models;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nop.Plugin.ExternalAuth.ExtendedAuthentication.Controllers
{
    public class AuthenticationController : BasePluginController
    {
        #region Fields        
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IOptionsMonitorCache<FacebookOptions> _optionsFacebookCache;
        private readonly IOptionsMonitorCache<TwitterOptions> _optionsTwitterCache;
        private readonly IOptionsMonitorCache<GoogleOptions> _optionsGoogleCache;
        private readonly IOptionsMonitorCache<MicrosoftAccountOptions> _optionsMicrosoftCache;
        private readonly IAuthenticationPluginManager _authenticationPluginManager;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;       

        #endregion

        #region Ctor

        public AuthenticationController(
            IExternalAuthenticationService externalAuthenticationService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
             IOptionsMonitorCache<FacebookOptions> optionsFacebookCache,
            IOptionsMonitorCache<TwitterOptions> optionsTwitterCache,
            IOptionsMonitorCache<GoogleOptions> optionsGoogleCache,
            IOptionsMonitorCache<MicrosoftAccountOptions> optionsMicrosoftCache,
            IAuthenticationPluginManager authenticationPluginManager,
            INotificationService notificationService,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _externalAuthenticationService = externalAuthenticationService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _optionsFacebookCache = optionsFacebookCache;
            _optionsTwitterCache = optionsTwitterCache;
            _optionsGoogleCache = optionsGoogleCache;
            _optionsMicrosoftCache = optionsMicrosoftCache;
            _authenticationPluginManager = authenticationPluginManager;
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Methods

        public IActionResult Index()
        {
            return View();
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope            
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var externalAuths = Enum.GetValues(typeof(ExternalAuthEnum)).Cast<ExternalAuthEnum>();
            var _externalAuthSettings = _settingService.LoadSetting<ExternalAuthSettings>(storeScope);

            ConfigurationModel model = new ConfigurationModel();

            // active or not
            model.ActiveStoreScopeConfiguration = storeScope;

            // facebook
            model.FacebookClientId = _externalAuthSettings.FacebookClientId;
            model.FacebookClientSecret = _externalAuthSettings.FacebookClientSecret;
            model.FacebookBtnIsDisplay = _externalAuthSettings.FacebookEnable;
            if (storeScope > 0)
            {
                model.FacebookBtnIsDisplay_OverrideForStore = _settingService.SettingExists(_externalAuthSettings, x => x.FacebookEnable, storeScope);
                model.FacebookClientId_OverrideForStore = _settingService.SettingExists(_externalAuthSettings, x => x.FacebookClientId, storeScope);
                model.FacebookClientSecret_OverrideForStore = _settingService.SettingExists(_externalAuthSettings, x => x.FacebookClientSecret, storeScope);
            }

            // Twitter
            model.TwitterClientId = _externalAuthSettings.TwitterClientId;
            model.TwitterClientSecret = _externalAuthSettings.TwitterClientSecret;
            model.TwitterBtnIsDisplay = _externalAuthSettings.TwitterEnable;
            if (storeScope > 0)
            {
                model.TwitterBtnIsDisplay_OverrideForStore = _settingService.SettingExists(_externalAuthSettings, x => x.TwitterEnable, storeScope);
                model.TwitterClientId_OverrideForStore = _settingService.SettingExists(_externalAuthSettings, x => x.TwitterClientId, storeScope);
                model.TwitterClientSecret_OverrideForStore = _settingService.SettingExists(_externalAuthSettings, x => x.TwitterClientSecret, storeScope);
            }

            // Google
            model.GoogleClientId = _externalAuthSettings.GoogleClientId;
            model.GoogleClientSecret = _externalAuthSettings.GoogleClientSecret;
            model.GmailBtnIsDisplay = _externalAuthSettings.GoogleEnable;
            if (storeScope > 0)
            {
                model.GmailBtnIsDisplay_OverrideForStore = _settingService.SettingExists(_externalAuthSettings, x => x.GoogleEnable, storeScope);
                model.GoogleClientId_OverrideForStore = _settingService.SettingExists(_externalAuthSettings, x => x.GoogleClientId, storeScope);
                model.GoogleClientSecret_OverrideForStore = _settingService.SettingExists(_externalAuthSettings, x => x.GoogleClientSecret, storeScope);
            }

            // Microsoft
            model.MicrosoftClientId = _externalAuthSettings.MicrosoftClientId;
            model.MicrosoftClientSecret = _externalAuthSettings.MicrosoftClientSecret;
            model.MicrosoftBtnIsDisplay = _externalAuthSettings.MicrosoftEnable;
            if (storeScope > 0)
            {
                model.MicrosoftBtnIsDisplay_OverrideForStore = _settingService.SettingExists(_externalAuthSettings, x => x.MicrosoftEnable, storeScope);
                model.MicrosoftClientId_OverrideForStore = _settingService.SettingExists(_externalAuthSettings, x => x.MicrosoftClientId, storeScope);
                model.MicrosoftClientSecret_OverrideForStore = _settingService.SettingExists(_externalAuthSettings, x => x.MicrosoftClientSecret, storeScope);
            }

            // LinkedIn
            model.LinkedInClientId = _externalAuthSettings.LinkedInClientId;
            model.LinkedInClientSecret = _externalAuthSettings.LinkedInClientSecret;
            model.LinkedInBtnIsDisplay = _externalAuthSettings.LinkedInEnable;
            if (storeScope > 0)
            {
                model.LinkedInBtnIsDisplay_OverrideForStore = _settingService.SettingExists(_externalAuthSettings, x => x.LinkedInEnable, storeScope);
                model.LinkedInClientId_OverrideForStore = _settingService.SettingExists(_externalAuthSettings, x => x.LinkedInClientId, storeScope);
                model.LinkedInClientSecret_OverrideForStore = _settingService.SettingExists(_externalAuthSettings, x => x.LinkedInClientSecret, storeScope);
            }

            return View("~/Plugins/ExternalAuth.ExtendedAuth/Views/Configure.cshtml", model);
        }

        [HttpPost]        
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var _externalAuthSettings = _settingService.LoadSetting<ExternalAuthSettings>(storeScope);

            #region Facebook

            //save settings facebook
            _externalAuthSettings.FacebookClientId = model.FacebookClientId;
            _externalAuthSettings.FacebookClientSecret = model.FacebookClientSecret;
            _externalAuthSettings.FacebookEnable = model.FacebookBtnIsDisplay;
            _settingService.SaveSetting(_externalAuthSettings);

            _settingService.SaveSettingOverridablePerStore(_externalAuthSettings, x => x.FacebookEnable, model.FacebookBtnIsDisplay_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(_externalAuthSettings, x => x.FacebookClientSecret, model.FacebookClientSecret_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(_externalAuthSettings, x => x.FacebookClientId, model.FacebookClientId_OverrideForStore, storeScope, false);

            #endregion Facebook

            #region twitter

            //save settings
            _externalAuthSettings.TwitterClientId = model.TwitterClientId;
            _externalAuthSettings.TwitterClientSecret = model.TwitterClientSecret;
            _externalAuthSettings.TwitterEnable = model.TwitterBtnIsDisplay;
            _settingService.SaveSetting(_externalAuthSettings);

            _settingService.SaveSettingOverridablePerStore(_externalAuthSettings, x => x.TwitterEnable, model.TwitterBtnIsDisplay_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(_externalAuthSettings, x => x.TwitterClientId, model.TwitterClientId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(_externalAuthSettings, x => x.TwitterClientSecret, model.TwitterClientSecret_OverrideForStore, storeScope, false);

            #endregion twitter  

            #region Google

            //save settings
            _externalAuthSettings.GoogleClientId = model.GoogleClientId;
            _externalAuthSettings.GoogleClientSecret = model.GoogleClientSecret;
            _externalAuthSettings.GoogleEnable = model.GmailBtnIsDisplay;
            _settingService.SaveSetting(_externalAuthSettings);

            _settingService.SaveSettingOverridablePerStore(_externalAuthSettings, x => x.GoogleEnable, model.GmailBtnIsDisplay_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(_externalAuthSettings, x => x.GoogleClientId, model.GoogleClientId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(_externalAuthSettings, x => x.GoogleClientSecret, model.GoogleClientSecret_OverrideForStore, storeScope, false);

            #endregion Google

            #region Miscrosoft

            //save settings
            _externalAuthSettings.MicrosoftClientId = model.MicrosoftClientId;
            _externalAuthSettings.MicrosoftClientSecret = model.MicrosoftClientSecret;
            _externalAuthSettings.MicrosoftEnable = model.MicrosoftBtnIsDisplay;
            _settingService.SaveSetting(_externalAuthSettings);

            _settingService.SaveSettingOverridablePerStore(_externalAuthSettings, x => x.MicrosoftEnable, model.MicrosoftBtnIsDisplay_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(_externalAuthSettings, x => x.MicrosoftClientId, model.MicrosoftClientId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(_externalAuthSettings, x => x.MicrosoftClientSecret, model.MicrosoftClientSecret_OverrideForStore, storeScope, false);

            #endregion Miscrosoft

            #region LinkedIn

            //save settings
            _externalAuthSettings.LinkedInClientId = model.LinkedInClientId;
            _externalAuthSettings.LinkedInClientSecret = model.LinkedInClientSecret;
            _externalAuthSettings.LinkedInEnable = model.LinkedInBtnIsDisplay;
            _settingService.SaveSetting(_externalAuthSettings);

            _settingService.SaveSettingOverridablePerStore(_externalAuthSettings, x => x.LinkedInEnable, model.LinkedInBtnIsDisplay_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(_externalAuthSettings, x => x.LinkedInClientId, model.LinkedInClientId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(_externalAuthSettings, x => x.LinkedInClientSecret, model.LinkedInClientSecret_OverrideForStore, storeScope, false);

            #endregion LinkedIn

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            //clear Facebook authentication options cache
            _optionsFacebookCache.TryRemove(FacebookDefaults.AuthenticationScheme);
            _optionsGoogleCache.TryRemove(GoogleDefaults.AuthenticationScheme);
            _optionsMicrosoftCache.TryRemove(MicrosoftAccountDefaults.AuthenticationScheme);
            _optionsTwitterCache.TryRemove(TwitterDefaults.AuthenticationScheme);

            return Configure();
        }

        public IActionResult Login(string returnUrl, string authentication)
        {
            if (!_authenticationPluginManager.IsPluginActive(AuthenticationDefaults.PluginSystemName))
                throw new NopException("Authentication module cannot be loaded");

            //configure login callback action
            var socialMediaList = new SocialMediaList();

            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = "/"+ socialMediaList.SocialMedias.Where(x => x.Name.ToLower() == authentication.ToLower()).Select(x => x.CallBackPath.ToLower()).FirstOrDefault() + "?returnUrl="+ returnUrl  //Url.Action("LoginCallback", "Authentication", new { returnUrl = returnUrl })
            };
            

            authenticationProperties.SetString(AuthenticationDefaults.ErrorCallback, Url.RouteUrl("Login", new { returnUrl }));
                       

            return Challenge(authenticationProperties, authentication);
        }

        public async Task<IActionResult> LoginCallback(string returnUrl)
        {
            string authenticationName = string.Empty;
            var urlPath = _httpContextAccessor.HttpContext.Request.Path;
            if (urlPath.HasValue)
            {
                var socialMediaList = new SocialMediaList();
                authenticationName = socialMediaList.SocialMedias.Where(x => "/" + x.CallBackPath.ToLower() == urlPath.Value).Select(x => x.Name).FirstOrDefault();
            }
            
            //authenticate social user
            var authenticateResult = await this.HttpContext.AuthenticateAsync(authenticationName);
            if (!authenticateResult.Succeeded || !authenticateResult.Principal.Claims.Any())
                return RedirectToRoute("Login");

            string email = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                email = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name)?.Value + "@" + authenticateResult.Principal.Identity.AuthenticationType + ".com";

            //create external authentication parameters
            var authenticationParameters = new ExternalAuthenticationParameters
            {
                ProviderSystemName = AuthenticationDefaults.PluginSystemName,
                AccessToken = await this.HttpContext.GetTokenAsync(authenticationName, "access_token"),
                Email = email,
                ExternalIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value,
                ExternalDisplayIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name)?.Value,
                Claims = authenticateResult.Principal.Claims.Select(claim => new ExternalAuthenticationClaim(claim.Type, claim.Value)).ToList()
            };

            //authenticate Nop user
            return _externalAuthenticationService.Authenticate(authenticationParameters, returnUrl);
        }

        #endregion
    }
}