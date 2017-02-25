using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using Owin;
using Pollenalarm.Backend.AspNet.DataObjects;
using Pollenalarm.Backend.AspNet.Models;
using Pollenalarm.Core.Models;

namespace Pollenalarm.Backend.AspNet
{
    public partial class Startup
    {
        public static void ConfigureMobileApp(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            new MobileAppConfiguration()
                .UseDefaultConfiguration()
                .ApplyTo(config);

            // Use Entity Framework Code First to create database tables based on your DbContext
            Database.SetInitializer(new MobileServiceInitializerDropCreateDatabaseAlways());

            MobileAppSettingsDictionary settings = config.GetMobileAppSettingsProvider().GetMobileAppSettings();

            if (string.IsNullOrEmpty(settings.HostName))
            {
                app.UseAppServiceAuthentication(new AppServiceAuthenticationOptions
                {
                    // This middleware is intended to be used locally for debugging. By default, HostName will
                    // only have a value when running in an App Service application.
                    SigningKey = ConfigurationManager.AppSettings["SigningKey"],
                    ValidAudiences = new[] { ConfigurationManager.AppSettings["ValidAudience"] },
                    ValidIssuers = new[] { ConfigurationManager.AppSettings["ValidIssuer"] },
                    TokenHandler = config.GetAppServiceTokenHandler()
                });
            }

            app.UseWebApi(config);

            // Swagger
            ConfigureSwagger(config);
        }
    }

    public class MobileServiceInitializerCreateDatabaseIfNotExists : CreateDatabaseIfNotExists<MobileServiceContext>
    {
        protected override void Seed(MobileServiceContext context)
        {
            Seeder.AddDefaultPollenToDatabase(context);
            base.Seed(context);
        }
    }

    public class MobileServiceInitializerDropCreateDatabaseAlways : DropCreateDatabaseAlways<MobileServiceContext>
    {
        protected override void Seed(MobileServiceContext context)
        {
            Seeder.AddDefaultPollenToDatabase(context);
            base.Seed(context);
        }
    }

    public static class Seeder
    {
        public static void AddDefaultPollenToDatabase(MobileServiceContext context)
        {
            List<PollenDto> defaultPollen = new List<PollenDto>
            {
                new PollenDto { Id = null, Name = "Ambrosia", Description = "Lorem ipsum dolor", BloomStart = new DateTime(2017, 06, 20), BloomEnd = new DateTime(2017, 10, 31), ClinicalPollution = 4, ImageCredits = "Erika Hartmann / pixelio.de" },
                new PollenDto { Id = null, Name = "Ampfer", Description = "Lorem ipsum dolor", BloomStart = new DateTime(2017, 04, 20), BloomEnd = new DateTime(2017, 09, 15), ClinicalPollution = 2, ImageCredits = "Susanne Schmich / pixelio.de" },
                new PollenDto { Id = null, Name = "Beifuß", Description = "Lorem ipsum dolor", BloomStart = new DateTime(2017, 05, 15), BloomEnd = new DateTime(2017, 10, 31), ClinicalPollution = 4, ImageCredits = "knipseline / pixelio.de" },
                new PollenDto { Id = null, Name = "Birke", Description = "Lorem ipsum dolor", BloomStart = new DateTime(2017, 02, 01), BloomEnd = new DateTime(2017, 07, 15), ClinicalPollution = 4, ImageCredits = "Regina Kaute / pixelio.de" },
                new PollenDto { Id = null, Name = "Buche", Description = "Lorem ipsum dolor", BloomStart = new DateTime(2017, 03, 01), BloomEnd = new DateTime(2017, 06, 30), ClinicalPollution = 1, ImageCredits = "Ilka Funke-Wellstein / pixelio.de" },
                new PollenDto { Id = null, Name = "Eiche", Description = "Lorem ipsum dolor", BloomStart = new DateTime(2017, 04, 15), BloomEnd = new DateTime(2017, 05, 31), ClinicalPollution = 2, ImageCredits = "Helene13 / pixelio.de" },
                new PollenDto { Id = null, Name = "Erle", Description = "Lorem ipsum dolor", BloomStart = new DateTime(2017, 02, 20), BloomEnd = new DateTime(2017, 03, 31), ClinicalPollution = 3, ImageCredits = "Andreas Hermsdorf / pixelio.de" },
                new PollenDto { Id = null, Name = "Gräser", Description = "Lorem ipsum dolor", BloomStart = new DateTime(2017, 05, 15), BloomEnd = new DateTime(2017, 08, 10), ClinicalPollution = 4, ImageCredits = "Rosel Eckstein / pixelio.de" },
                new PollenDto { Id = null, Name = "Hasel", Description = "Lorem ipsum dolor", BloomStart = new DateTime(2017, 02, 15), BloomEnd = new DateTime(2017, 03, 31), ClinicalPollution = 3, ImageCredits = "Maja Dumat / pixelio.de" },
                new PollenDto { Id = null, Name = "Pappel", Description = "Lorem ipsum dolor", BloomStart = new DateTime(2017, 03, 15), BloomEnd = new DateTime(2017, 04, 15), ClinicalPollution = 1, ImageCredits = "Maja Dumat / pixelio.de" },
                new PollenDto { Id = null, Name = "Roggen", Description = "Lorem ipsum dolor", BloomStart = new DateTime(2017, 04, 15), BloomEnd = new DateTime(2017, 07, 31), ClinicalPollution = 4, ImageCredits = "s.media / pixelio.de" },
                new PollenDto { Id = null, Name = "Ulme", Description = "Lorem ipsum dolor", BloomStart = new DateTime(2017, 03, 01), BloomEnd = new DateTime(2017, 04, 30), ClinicalPollution = 1, ImageCredits = "Elke Sawistowski / pixelio.de" },
                new PollenDto { Id = null, Name = "Wegerich", Description = "Lorem ipsum dolor", BloomStart = new DateTime(2017, 05, 31), BloomEnd = new DateTime(2017, 08, 30), ClinicalPollution = 2, ImageCredits = "Luise / pixelio.de" },
                new PollenDto { Id = null, Name = "Weide", Description = "Lorem ipsum dolor", BloomStart = new DateTime(2017, 03, 15), BloomEnd = new DateTime(2017, 05, 10), ClinicalPollution = 1, ImageCredits = "Susanne Schmich / pixelio.de" }
            };

            context.Set<PollenDto>().AddRange(defaultPollen);
        }
    }
}

