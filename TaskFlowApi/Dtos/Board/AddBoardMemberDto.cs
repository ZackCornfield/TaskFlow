using System.ComponentModel.DataAnnotations;

namespace TaskFlowApi.Dtos.Board;

public record class AddBoardMemberDto : IValidatableObject
{
    public Guid? UserId { get; set; } // Optional UserId
    public string? Email { get; set; } // Optional Email
    public int BoardId { get; set; }
    public string Role { get; set; } = null!;

    // Validation logic to ensure at least one of UserId or Email is provided
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (UserId == null && string.IsNullOrWhiteSpace(Email))
        {
            yield return new ValidationResult(
                "Either UserId or Email must be provided.",
                new[] { nameof(UserId), nameof(Email) }
            );
        }
    }
}
