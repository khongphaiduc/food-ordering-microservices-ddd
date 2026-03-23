using Google.GenAI;
using Google.GenAI.Types;
using tracking_service.Tracking.Application.Interface;

namespace tracking_service.Tracking.Infastructrure.ImplementServices
{
    public class ServiceAI : IServiceAI
    {
        private readonly Client _geminiClient;
        private const string _modelName = "gemini-2.5-flash";

        public ServiceAI(IConfiguration configuration)
        {

            var apiKey = configuration["GeminiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("Lỗi: Chưa cấu hình biến môi trường GEMINI_API_KEY!");
            }

            _geminiClient = new Client(apiKey: apiKey);
        }

        public async Task<string> Prompt(string prompt)
        {
            try
            {
                var contents = new List<Content>
                {
                    new Content { Role = "user", Parts = new List<Part> { new Part { Text = prompt } } }
                };


                var response = await _geminiClient.Models.GenerateContentAsync(_modelName, contents);

                return response.Text ?? "Không có phản hồi từ AI.";
            }
            catch (Exception ex)
            {
                return $"Lỗi ServiceAI: {ex.Message}";
            }
        }
    }
}