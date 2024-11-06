using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

public class MaxBombCountAttribute : ValidationAttribute
{
    private readonly string _sizePropertyName;

    public MaxBombCountAttribute(string sizePropertyName)
    {
        _sizePropertyName = sizePropertyName;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // Get the Size property from the validation context
        PropertyInfo sizeProperty = validationContext.ObjectType.GetProperty(_sizePropertyName);
        if (sizeProperty == null)
        {
            return new ValidationResult($"Unknown property: {_sizePropertyName}");
        }

        // Retrieve the value of Size
        int sizeValue = (int)sizeProperty.GetValue(validationContext.ObjectInstance);

        // Calculate the maximum allowable bombs
        int maxBombs = sizeValue * sizeValue - 1;

        // Ensure that the BombCount is not null and within range
        if (value is int bombCount)
        {
            if (bombCount > maxBombs)
            {
                return new ValidationResult($"Bomb count cannot exceed {maxBombs} for a board of size {sizeValue}x{sizeValue}.");
            }
        }
        else
        {
            return new ValidationResult("Invalid BombCount value.");
        }

        return ValidationResult.Success;
    }
}
