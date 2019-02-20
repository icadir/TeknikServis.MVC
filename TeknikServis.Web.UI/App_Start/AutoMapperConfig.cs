using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using TeknikServis.Entity.Anket;
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
            cfg.CreateMap<ArızaKayıt, ArizaViewModel>()
                .ForMember(dest => dest.ArizaId, opt => opt.MapFrom(x => x.Id))
                .ReverseMap();
        }

        // TODO asagıdaki mapper örnekleri diger projelerde yaptıgımız örnekler.
        //private static void EmployeeMapping(IMapperConfigurationExpression cfg)
        //{
        //    cfg.CreateMap<Employee, EmployeeViewModel>()
        //        .ForMember(dest => dest.SubEmplyeeCount, opt => opt.MapFrom(x => x.Employees1.Count))
        //        .ForMember(dest => dest.ReportsName, opt => opt.MapFrom((s, d) => s.Employee1?.FirstName + " " + s.Employee1?.LastName))
        //        .ReverseMap();
        //}

        //private static void ProductMapping(IMapperConfigurationExpression cfg)
        //{
        //    cfg.CreateMap<Product, ProductViewModel>()
        //        .ForMember(dest => dest.CategoryName, opt => opt.MapFrom((s, d) => s.Category == null ? "Kategorisiz" : s.Category.CategoryName))
        //        .ReverseMap();
        //}

        //private static void CategoryMapping(IMapperConfigurationExpression cfg)
        //{
        //    cfg.CreateMap<Category, CategoryViewModel>()
        //        .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(x => x.Products.Count))
        //        .ReverseMap();
        //}
    }
}