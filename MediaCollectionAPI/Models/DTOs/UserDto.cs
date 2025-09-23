public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
}

public class CreateUserDto
{
    public string Username { get; set; }
}