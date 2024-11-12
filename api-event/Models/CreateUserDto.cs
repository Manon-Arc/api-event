namespace api_event.Models
{
    public class CreateUserDto
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Mail { get; set; } = null!;

        public int? Permission { get; set; } = null!;
    }
}
