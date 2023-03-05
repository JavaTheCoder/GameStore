using Microsoft.AspNetCore.Identity;

namespace GameStoreData.Identity.Data;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ImageURL { get; set; }
}
