using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TeknikServis.Entity.Enums;
using TeknikServis.Entity.IdentityModels;
using TeknikServis.Entity.ViewModels.LogViewModel;
using TeknikServis.Web.UI.Abstracts;

namespace TeknikServis.Entity.Entitties
{
    [Table("ArizaKayitlari")]
    public class ArızaKayıt : BaseEntity<int>
    {

        [Required(ErrorMessage = "Lütfen Açıklama kısmını doldurdunuz.")]
        [DisplayName("Arıza Açıklaması")]
        [StringLength(1000)]
        public string ArızaAcıklaması { get; set; }

        [Required(ErrorMessage = "Adres Alanını doldurunuz.")]
        [DisplayName("Adres Giriniz")]
        [StringLength(500, ErrorMessage = "Adres Alanı max 500 karakter olabilir.")]
        public string Adres { get; set; }
        [DisplayName("Telefon Numarası")]
        public string Telno { get; set; }
        [DisplayName("İletişim Maili")]
        public string Email { get; set; }

        [DisplayName("Enlem Giriniz")]
        public string Enlem { get; set; }
        [DisplayName("Boylam Giriniz ")]
        public string Boylam { get; set; }


        //TODO resimler için gerekli alanlar kısmı
        [DisplayName("Ürün Resmi Ekleyiniz")]
        public List<string> ArızaPath { get; set; }
        //todo view modelyapcaksın bu alanı resim için.
        [DisplayName("Fatura Resmini Ekleyiniz")]
        public string FaturaPath { get; set; }

        //TODO TARİH KISMI
        [DisplayName("Arızanın Olusturuldugu Tarihi")]
        public DateTime ArizaOlusturmaTarihi { get; set; } = DateTime.Now;
        [DisplayName("Operator Kabul Tarihi")]
        public DateTime? OperatorKabulTarih { get; set; }
        [DisplayName("Operator Teknisyen Atadıgı Tarih")]
        public DateTime? TeknisyenAtandigiTarih { get; set; }
        [DisplayName("Arızanın Çözüldüğü Tarih")]
        public DateTime? ArizaCozulduguTarih { get; set; }
        [DisplayName("Arıza Son Kontrol Tarihi")]
        public DateTime? ArizaSonKontrolTarihi { get; set; }

        //TODO Enums
        [DisplayName("Çözüm Durumunu Seçiniz")]
        public TeknisyenArizaDurum? TeknisyenArizaDurum { get; set; }
        public ArizaDurum ArizaDurumu { get; set; } = ArizaDurum.Beklemede;
        [DisplayName("Ürün Tipini Seçiniz")]
        public BeyazEsyaTip BeyazEsya { get; set; }

        [DisplayName("Teknisyen Arıza Açıklaması")]
        public string TeknisyenArizaAciklama { get; set; }

        //todo otomatik olarak false sen operator sayfasında false olanları getirirsin. eger onaylarsa yeni bir sayfasına true olanları alırsın. ve orda yönlendirmeyi yaparsın. Yerse belki ariza durum a da onaylandi eklenebilir.
        public bool OperatorKabul { get; set; } = false;

        //TODO Operatör icin bool alan.
        public string AnketCode { get; set; }

        //TODO Bosta alan teknisyenleri için
        public bool TeknisyenIstemi { get; set; } = false;


        [Required]
        public string MusteriId { get; set; }
        public string OperatorId { get; set; }
        public string TeknisyenId { get; set; }

        [ForeignKey("MusteriId")]
        public User Musteri { get; set; }
        [ForeignKey("OperatorId")]
        public User Operator { get; set; }
        [ForeignKey("TeknisyenId")]
        public User Teknisyen { get; set; }


        //TODO ANKET KISMI

        [DisplayName("Teknisyenin Konu Hakkında Bilgisi Yeterli miydi ?")]
        public AnketEnum TeknisyenBilgiPuani { get; set; }
        [DisplayName("Teknisyenin Size Karşı Davranışı nasıldı ?")]
        public AnketEnum TeknisyenDavranisPuani { get; set; }
        [DisplayName("Çözüm Sürecinde Fitech Çalışanlarının iletişimi Nasıldı ?")]
        public AnketEnum FitechDavranisPuani { get; set; }
        [DisplayName("FiTechten Memnun Kaldınız mı ?")]
        public AnketEnum HizmetPuanı { get; set; }
        [DisplayName("FİTech Hakkındaki Görüşleriniz.")]
        public string FitechHakkindakiGorusler { get; set; }

        //TODO anket yapıldımı için alan.
        public bool AnketYapildimi { get; set; } = false;


        public virtual List<Fotograf> Fotograflar { get; set; } = new List<Fotograf>();

        public virtual ICollection<ArizaLOG> ArizaLogs { get; set; }=new HashSet<ArizaLOG>();

    }
}
