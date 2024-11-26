namespace API.Validators
{
    using API.Dtos;
    using FluentValidation;

    public class BlobUploadRequestValidator : AbstractValidator<CreateBlobRequest>
    {
        public BlobUploadRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("'Id' is required.")
                .MaximumLength(255).WithMessage("'Id' cannot exceed 255 characters.");

            // Validate 'Data'
            RuleFor(x => x.Data)
                .NotEmpty().WithMessage("'Data' is required.")
                .Must(BeValidBase64).WithMessage("'Data' must be a valid Base64 string.");
        }

        // Custom validation for Base64-encoded strings
        private bool BeValidBase64(string data)
        {
            if (string.IsNullOrEmpty(data))
                return false;

            try
            {
                Convert.FromBase64String(data);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }

}
