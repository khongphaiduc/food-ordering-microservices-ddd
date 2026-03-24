using tracking_service.Tracking.Domain.Enums;

namespace tracking_service.Tracking.Application.Interface
{
    public interface IGetBehaviourOfUser
    {
        Task<BehaviourUser> Execute(Guid IdUser);
    }


    public class BehaviourUser
    {
        public Guid IdUser { get; set; }

        public List<DataOfUsers> BehaviourUsers { get; set; } = new List<DataOfUsers>();

    }

    public class DataOfUsers
    {
        public string eventType { get; set; }
        public Guid IdProduct { get; set; }
        public int CountTimes { get; set; }
    }
}
