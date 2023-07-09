namespace WebApi.ErrorResponse.ViaProblemDetailsFactory.ValidationAttributes;

using System.ComponentModel.DataAnnotations;

public class GreaterThanZeroValidationAttribute : ValidationAttribute
{
	protected override ValidationResult? IsValid(Object? value, ValidationContext validationContext)
	{
		if (value is Int32 id && id > 0)
			return ValidationResult.Success;

		return new ValidationResult("Must be greater than 0.");
	}
}
