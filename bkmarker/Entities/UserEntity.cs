
namespace bkmarker.Entities;

public class UserEntity
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
}
