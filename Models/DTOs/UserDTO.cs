// Models/UserDTO.cs

namespace Spotify.Models.DTOs;

public class UserDTO
{
    public string Username { get; set; }
    public string HashedPassword { get; set; }
    public string Salt { get; set; }
}
