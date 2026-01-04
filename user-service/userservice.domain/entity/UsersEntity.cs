using user_service.userservice.domain.value_object;

namespace user_service.userservice.domain.entity
{
    public class UsersEntity
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;

        public Email? EmailAddress { get; set; }

        public PhoneNumber PhoneNumbers { get; set; } = null!;

        public UsersEntity(Guid id, string fullName, Email? emailAddress, PhoneNumber phoneNumbers)
        {
            Id = id;
            FullName = fullName;
            EmailAddress = emailAddress;
            PhoneNumbers = phoneNumbers;
        }
    }

}
