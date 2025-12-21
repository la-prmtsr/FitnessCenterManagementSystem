# Fitness Center Management System
ASP.NET Core MVC mimarisi üzerine inşa edilmiş, spor salonları için uçtan uca randevu ve yönetim çözümüdür. Proje, bir işletmenin temel operasyonel ihtiyaçlarını dijitalleştirmeyi amaçlar.

## Technical Stack
Bu proje aşağıdaki teknolojiler ve prensipler kullanılarak geliştirilmiştir:
### **Backend**

**Framework**: ASP.NET Core 8.0 MVC

**ORM**: Entity Framework Core (Code First)

**Database**: MS SQL Server

**AI Models**: Groq Llama-3 (Text), Pollinations/Flux (Vision & Image Generation)

### **Frontend**

**UI Style**: CORE 36 Modern Soft UI (Siyah-Lime Teması)

**Frameworks**: Bootstrap 5.3, jQuery, AJAX

**Icons**: Bootstrap Icons

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
