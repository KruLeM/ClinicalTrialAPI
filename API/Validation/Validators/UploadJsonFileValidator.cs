﻿using FluentValidation;
using API.Validation.RequestModels;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Text;

namespace API.Validation.Validators
{
    public class UploadJsonFileValidator : AbstractValidator<UploadJsonFileRequestModel>
    {
        private readonly JSchema _jsonSchema;

        public UploadJsonFileValidator()
        {
            string schemaPath = Path.Combine(AppContext.BaseDirectory, "Resources", "clinical-trial-json-shema.json");
            if (!File.Exists(schemaPath))
            {
                throw new FileNotFoundException($"JSON Schema file not found at: {schemaPath}");
            }

            string schemaContent = File.ReadAllText(schemaPath);
            _jsonSchema = JSchema.Parse(schemaContent);

            RuleFor(x => x.File)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("File is required.")
                .Must(file => Path.GetExtension(file.FileName).ToLower() == ".json").WithMessage("Only .json files are allowed.")
                .Must(file => file.Length > 0).WithMessage("File is empty.")
                .Must(file => file.Length <= ValidationConstants.MaxFileSizeInBytes).WithMessage("File exceeds the maximum allowed size.")
                .Must(ValidateJsonSchema).WithMessage("Invalid JSON format according to schema.");
        }
        private bool ValidateJsonSchema(IFormFile file)
        {
            try
            {
                using var stream = new MemoryStream();
                file.CopyToAsync(stream);
                stream.Position = 0;

                using var reader = new StreamReader(stream, Encoding.UTF8);
                string fileContent = reader.ReadToEnd();

                var jsonObject = JObject.Parse(fileContent);
                return jsonObject.IsValid(_jsonSchema, out IList<string> _);
            }
            catch
            {
                return false;
            }
        }
    }
}
