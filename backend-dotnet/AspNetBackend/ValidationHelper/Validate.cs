using System.ComponentModel.DataAnnotations;

namespace AspNetBackend.ValidationHelper;

/// <summary>
/// Provides utility methods for validating Data Transfer Objects (DTOs) using data annotations.
/// </summary>
public class Validate
{
    /// <summary>
    /// Validates a DTO based on its data annotations.
    /// </summary>
    /// <typeparam name="T">The type of the DTO to validate.</typeparam>
    /// <param name="dto">The DTO instance to validate.</param>
    /// <param name="results">A collection of validation results containing error messages, if any.</param>
    /// <returns>
    /// True if the DTO is valid according to its data annotations; otherwise, false.
    /// </returns>
    public static bool IsValidDto<T>(T dto, out ICollection<ValidationResult> results)
    {

        results = new List<ValidationResult>();

        if (dto == null)
        {
            return false;
        }

        return Validator.TryValidateObject(dto, new ValidationContext(dto), results, true);

    }

}
