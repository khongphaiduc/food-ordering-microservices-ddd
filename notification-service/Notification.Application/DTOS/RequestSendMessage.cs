namespace notification_service.Notifications.DTOS
{
    public class RequestSendMessage
    {
        public Guid IdMessage { get; set; }

        public string To { get; set; }

        public string Subject { get; set; }


        public string Body { get; set; }


        public DateTime CreatedAt { get; set; }

    }
}
