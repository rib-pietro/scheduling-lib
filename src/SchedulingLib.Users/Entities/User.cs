namespace SchedulingLib.Users.Entities;

/// <summary>
/// A registered platform user who can be booked as a client in appointments and reservations.
/// At least one of <see cref="Email"/> or <see cref="Phone"/> must be provided.
/// </summary>
public class User
{
    /// <summary>Gets the unique identifier. Used as <c>client_id</c> in appointments.</summary>
    public Guid Id { get; }

    /// <summary>Gets the user's display name.</summary>
    public string Name { get; private set; }

    /// <summary>Gets the user's email address, or <c>null</c> if not provided.</summary>
    public string? Email { get; private set; }

    /// <summary>Gets the user's phone number, or <c>null</c> if not provided.</summary>
    public string? Phone { get; private set; }

    /// <summary>Gets when this user was registered.</summary>
    public DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Initializes a <see cref="User"/>.
    /// Throws <see cref="ArgumentException"/> when both <paramref name="email"/> and <paramref name="phone"/> are null or whitespace.
    /// </summary>
    public User(Guid id, string name, string? email, string? phone, DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("At least one of email or phone must be provided.");

        Id = id;
        Name = name;
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Creates a new <see cref="User"/> with a generated ID and the current UTC timestamp.
    /// </summary>
    public static User Register(string name, string? email, string? phone) =>
        new(Guid.NewGuid(), name, email, phone, DateTimeOffset.UtcNow);
}
