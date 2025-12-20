using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace FitnessCenterManagementSystem.Controllers
{
    public class AiController : Controller
    {
        
        private readonly string _apiKey;
        private readonly string _apiUrl = "https://api.groq.com/openai/v1/chat/completions";
        private readonly IConfiguration _configuration;

        
        public AiController(IConfiguration configuration)
        {
            _configuration = configuration;
            // User Secrets veya appsettings.json dosyasındaki "GroqApiKey" değerini çeker
            _apiKey = _configuration["GroqApiKey"];
        }
        public IActionResult Index()
        {
            return View();
        }

        // ---------------------------------------------------------
        // 📝 1. METİN TABANLI TAVSİYE (Groq Llama-3)
        // ---------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> GetAdvice(int age, int weight, int height, string goal, string gender)
        {
            string prompt = $"Ben {age} yaşında, {height} cm boyunda, {weight} kg ağırlığında bir {gender} bireyim. " +
                            $"Hedefim: {goal}. " +
                            $"Bana 1 günlük örnek beslenme planı ve antrenman programı hazırla. " +
                            $"Cevabı HTML formatında (<b>, <ul>, <li>, <h4> etiketleri kullanarak) ver. Sadece içeriği ver, markdown kullanma.";

            // Groq için İstek Gövdesi (OpenAI Formatı)
            var requestBody = new
            {
                model = "llama-3.3-70b-versatile", // Metin için çok güçlü bir model
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            return await SendToGroq(requestBody);
        }

        // ---------------------------------------------------------
        // 📸 2. FOTOĞRAF ANALİZİ (Groq Llama-3.2 Vision)
        // ---------------------------------------------------------

        // ---------------------------------------------------------
        // 📸 2. FOTOĞRAF ANALİZİ VE DÖNÜŞÜM (GARANTİLİ VERSİYON)
        // ---------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> AnalyzeAndTransform(IFormFile photo, string targetGoal)
        {
            // Fotoğraf kontrolü
            if (photo == null || photo.Length == 0)
                return Json(new { success = false, message = "Lütfen fotoğraf yükleyin." });

            string finalPrompt = "";

            try
            {
                // 1. Önce Resmi Base64'e çevirip Groq'a göndermeyi DENE
                string base64Image = "";
                using (var ms = new MemoryStream())
                {
                    await photo.CopyToAsync(ms);
                    base64Image = Convert.ToBase64String(ms.ToArray());
                }
                string dataUrl = $"data:image/jpeg;base64,{base64Image}";

                string instruction = targetGoal switch
                {
                    "bulk" => "extremely bulky, massive muscles, bodybuilder physique",
                    "cut" => "shredded, lean, defined six pack abs, fitness model",
                    _ => "athletic, fit and healthy body"
                };

                var requestBody = new
                {
                    model = "llama-3.2-11b-vision-preview", // Vision modeli
                    messages = new[]
                    {
                new
                {
                    role = "user",
                    content = new object[]
                    {
                        new { type = "text", text = $"Describe this person based on gender and looks. Then imagine them having a {instruction}. Write a short English image prompt for this new look." },
                        new { type = "image_url", image_url = new { url = dataUrl } }
                    }
                }
            },
                    max_tokens = 300 // Cevabı kısa tutalım ki hızlı olsun
                };

                // Groq'a gönderiyoruz...
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
                    var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(_apiUrl, jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        // HEYOO! Groq fotoğrafı kabul etti
                        var responseString = await response.Content.ReadAsStringAsync();
                        dynamic result = JsonConvert.DeserializeObject(responseString);
                        finalPrompt = result.choices[0].message.content;
                    }
                    else
                    {
                        // HATA OLDU (Muhtemelen resim çok büyük): Manuel moda geçiyoruz
                        throw new Exception("Groq API resim boyutunu kabul etmedi.");
                    }
                }
            }
            catch (Exception)
            {
                // 🚨 B PLANI (FALLBACK): API hata verdiyse burası çalışır
                // Kullanıcıyı mağdur etmemek için hedefe uygun kaliteli bir prompt biz üretiyoruz.

                string goalDesc = targetGoal switch
                {
                    "bulk" => "a bodybuilder with massive muscles, bulky chest and arms, gym atmosphere, cinematic lighting",
                    "cut" => "a fitness model with shredded abs, low body fat, lean face, beach atmosphere, cinematic lighting",
                    "muscle" => "an athletic person with defined muscles, strong posture, sport studio lighting",
                    _ => "a fit and healthy person doing workout, cinematic lighting"
                };

                // Pollinations'a gidecek garanti prompt
                finalPrompt = $"A high quality, realistic photo of {goalDesc}, 8k resolution, detailed texture, photography style.";
            }

            // Sonuç: İster Groq'tan gelsin, ister biz yazalım, sonuçta bir prompt dönüyoruz.
            return Json(new { success = true, prompt = finalPrompt });
        }


        // ---------------------------------------------------------
        // 🚀 GROQ GÖNDERİM METODU
        // ---------------------------------------------------------
        private async Task<JsonResult> SendToGroq(object requestBody, bool isImagePrompt = false)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

                var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync(_apiUrl, jsonContent);
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        dynamic result = JsonConvert.DeserializeObject(responseString);
                        string aiText = result.choices[0].message.content;

                        if (isImagePrompt)
                            return Json(new { success = true, prompt = aiText });
                        else
                            return Json(new { success = true, message = aiText });
                    }
                    else
                    {
                        return Json(new { success = false, message = $"GROQ HATASI ({response.StatusCode}): {responseString}" });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"SİSTEM HATASI: {ex.Message}" });
                }
            }
        }
    }
}
