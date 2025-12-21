# FitnessCenter Management System
ASP.NET Core MVC mimarisi üzerine inşa edilmiş, spor salonları için uçtan uca randevu ve yönetim çözümüdür. Proje, bir işletmenin temel operasyonel ihtiyaçlarını dijitalleştirmeyi amaçlar.

## Teknik Yetkinlikler (Technical Stack)
Bu proje aşağıdaki teknolojiler ve prensipler kullanılarak geliştirilmiştir:

**Framework**: ASP.NET Core 8.0 MVC

**Veritabanı**: Entity Framework Core & MS SQL Server

**Kimlik Doğrulama**: Microsoft Identity (Rol tabanlı yetkilendirme: Admin & Üye)

**API Entegrasyonu**: Groq API (Llama-3 & Vision modelleri ile yapay zeka desteği)

**Frontend**: Bootstrap 5, jQuery ve AJAX tabanlı dinamik arayüzler

**Güvenlik**: Hassas veriler için User Secrets yönetimi ve form bazlı veri doğrulama (Data Annotations)

## Temel Özellikler
**1. Rol Bazlı Yönetim Paneli**
Admin: Eğitmen ekleme/silme, hizmet tanımlama ve tüm randevuları yönetme yetkisine sahiptir.

Üye: Kendine uygun eğitmeni ve hizmeti seçerek online randevu oluşturabilir.

**2. AI Destekli Danışmanlığı**
Groq API entegrasyonu sayesinde üyelerin fiziksel verilerine göre kişiselleştirilmiş beslenme ve antrenman önerileri sunar.

Görüntü işleme modelleri ile fotoğraf üzerinden analiz yapabilme altyapısına sahiptir.

**3. Dashboard ve Analiz**
Salonun toplam üye sayısı, günlük randevu yoğunluğu ve tahmini kazanç gibi kritik veriler admin panelinde görselleştirilir.

**4. RESTful API Servisleri**
Raporlama işlemleri için LINQ sorguları ile optimize edilmiş API endpoint'leri barındırır.
