namespace WebApi.ErrorResponse.ViaFilterAndMiddleware.Attributes;

using System;
using System.ComponentModel.DataAnnotations;

internal class GreaterThanZeroValidationAttribute : ValidationAttribute
{
	protected override ValidationResult? IsValid(Object? value, ValidationContext validationContext)
	{
		if (value is Int32 id && id > 0)
			return ValidationResult.Success;

		return new ValidationResult("Must be greater than 0.");
	}
}