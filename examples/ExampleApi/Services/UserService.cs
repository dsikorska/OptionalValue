using ExampleApi.Models;

namespace ExampleApi.Services;

public class UserService : IUserService
{
	private readonly List<User> _users = new()
	{
		new User
		{
			Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
			Name = "John Doe",
			Email = "john@example.com",
			Bio = "Software engineer",
			ExpiresOn = null,
			IsActive = true,
			CreatedAt = DateTime.UtcNow.AddDays(-30),
			UpdatedAt = DateTime.UtcNow.AddDays(-30)
		},
		new User
		{
			Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
			Name = "Jane Smith",
			Email = "jane@example.com",
			Bio = null,
			ExpiresOn = DateTime.UtcNow.AddDays(365),
			IsActive = true,
			CreatedAt = DateTime.UtcNow.AddDays(-15),
			UpdatedAt = DateTime.UtcNow.AddDays(-15)
		}
	};

	public IEnumerable<User> GetAll() => _users;

	public User? GetById(Guid id) => _users.FirstOrDefault(u => u.Id == id);

	public User Create(User user)
	{
		user.Id = Guid.NewGuid();
		user.CreatedAt = DateTime.UtcNow;
		user.UpdatedAt = DateTime.UtcNow;
		_users.Add(user);
		return user;
	}

	public User? Update(Guid id, PatchUserRequest request)
	{
		var user = GetById(id);
		if (user == null)
		{
			return null;
		}

		// This is where OptionalValue<T> shines!
		// Only update fields that were provided in the request

		if (request.Name.IsSpecified)
		{
			user.Name = request.Name.Value ?? string.Empty;
		}

		if (request.Email.IsSpecified)
		{
			user.Email = request.Email.Value ?? string.Empty;
		}

		if (request.Bio.IsSpecified)
		{
			user.Bio = request.Bio.Value; // Can be null
		}

		if (request.ExpiresOn.IsSpecified)
		{
			user.ExpiresOn = request.ExpiresOn.Value; // Can be null
		}

		if (request.IsActive.IsSpecified)
		{
			user.IsActive = request.IsActive.Value;
		}

		user.UpdatedAt = DateTime.UtcNow;

		return user;
	}

	public bool Delete(Guid id)
	{
		var user = GetById(id);
		if (user == null)
		{
			return false;
		}

		_users.Remove(user);
		return true;
	}
}
