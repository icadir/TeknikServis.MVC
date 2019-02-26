using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using TeknikServis.Entity.Entitties;
using TeknikServis.Entity.Enums;
using TeknikServis.Entity.ViewModels.LogViewModel;

namespace TeknikServis.Entity.ViewModels.ArizaViewModels
{
    public class ArizaViewModel
    {
        public int ArizaId { get; set; }

        [Required(ErrorMessage = "Lütfen Açıklama kısmını doldurdunuz.")]
        [DisplayName("Arıza Açıklaması")]
        [StringLength(1000)]
        public string ArızaAcıklaması { get; set; }

        [Required(ErrorMessage = "Adres Alanını doldurunuz.")]
        [DisplayName("Adres Giriniz :")]
        [StringLength(500, ErrorMessage = "Adres Alanı max 500 karakter olabilir.")]
        public string Adres { get; set; }

        [DisplayName("Telefon Numarası :")]
        public string Telno { get; set; }

        [DisplayName("İletişim Maili")]
        public string Email { get; set; }


        public double? Enlem { get; set; }

        public double? Boylam { get; set; }

        public string UserId { get; set; }

        //TODO resimler için gerekli alanlar kısmı
        [DisplayName("Ürün Resmi Ekleyiniz :")]
        public List<string> ArızaPath { get; set; }
        [DisplayName("Arızali Ürün Resmini Ekleyiniz :")]
        public List<HttpPostedFileBase> PostedFileAriza { get; set; }
        [DisplayName("Fatura Resmini Ekleyiniz")]
        public string FaturaPath { get; set; }
        [DisplayName("Ürünün Fatura Resmini Ekleyiniz.")]
        public HttpPostedFileBase PostedFileFatura { get; set; }


        //TODO TARİH KISMI
        [DisplayName("Arızanın Olusturuldugu Tarihi")]
        public DateTime ArizaOlusturmaTarihi { get; set; } = DateTime.Now;
        [DisplayName("Operator Kabul Tarihi")]
        public DateTime? OperatorKabulTarih { get; set; }
        [DisplayName("Operator Teknisyen Atadıgı Tarih")]
        public DateTime? TeknisyenAtandigiTarih { get; set; }
        [DisplayName("Arıza Son Kontrol Tarihi")]
        public DateTime? ArizaSonKontrolTarihi { get; set; }
        [DisplayName("Arızanın Çözüldüğü Tarih")]
        public DateTime? ArizaCozulduguTarih { get; set; }
        //default olarak çözülemedi atadık.


        //TODO ENUMS
        [DisplayName("Çözüm Durumunu Seçiniz")]
        public TeknisyenArizaDurum? TeknisyenArizaDurum { get; set; }
        public ArizaDurum ArizaDurumu { get; set; } = ArizaDurum.Beklemede;
        [DisplayName("Ürün Tipini Seçiniz")]
        public BeyazEsyaTip BeyazEsya { get; set; }
        public bool OperatorKabul { get; set; } = false;
        [DisplayName("Teknisyen Arıza Açıklaması")]
        public string TeknisyenArizaAciklama { get; set; }

        //TODO Bosta alan teknisyenleri için
        public TeknisyenDurumu TeknisyenDurumu { get; set; } = TeknisyenDurumu.Bosta;


        [Required]
        public string MusteriId { get; set; }
        public string OperatorId { get; set; }
        public string TeknisyenId { get; set; }

        //TODO arizaLogViewModel için
        public List<ArizaLOG> ArizaLogs { get; set; }

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

        public bool AnketYapildimi { get; set; } = false;

        public virtual List<Fotograf> Fotograflar { get; set; } = new List<Fotograf>();


    }
}
