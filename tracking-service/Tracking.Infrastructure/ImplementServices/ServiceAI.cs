using Google.GenAI;
using Google.GenAI.Types;
using tracking_service.Tracking.Application.Interface;

namespace tracking_service.Tracking.Infrastructure.ImplementServices
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
                throw new Exception("L?i: Chua c?u hình bi?n môi tru?ng GEMINI_API_KEY!");
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

                return response.Text ?? "Không có ph?n h?i t? AI.";
            }
            catch (Exception ex)
            {
                return $"L?i ServiceAI: {ex.Message}";
            }
        }
    }
}