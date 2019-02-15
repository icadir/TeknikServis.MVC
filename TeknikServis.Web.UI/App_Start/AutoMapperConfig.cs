using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using TeknikServis.Entity.Entitties;
using TeknikServis.Entity.ViewModels.ArizaViewModels;

namespace TeknikServis.Web.UI.App_Start
{
    public class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(cfg => { ArizaKayitMap(cfg); });
        }

        private static void ArizaKayitMap(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<ArızaKayıt, ArizaViewModel>().ReverseMap();
        }
    }
}