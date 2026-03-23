namespace tracking_service.Tracking.Application.Interface
{
    public interface IServiceAI
    {
        Task<string> Prompt(string prompt);
    }
}
