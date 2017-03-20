using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ExpressiveAnnotations.Attributes;
using ExpressiveAnnotations.MvcUnobtrusive.Validators;
using Orchard.Environment.Extensions;

namespace dcp.Routing.Models
{
    [OrchardFeature("dcp.Routing.Redirects")]
    public class RedirectRule
    {
        public RedirectRule()
        {
            CreatedDateTime = DateTime.Now;
        }

        public virtual int Id { get; set; }

        public DateTime CreatedDateTime { get; set; }

        [Required]
        [RegularExpression(ValidRelativeUrlPattern, ErrorMessage = "Do not start with '~/'")]
        [ClientValidation]
        [Display(Name = "Source URL")]
        public string SourceUrl { get; set; }

        [Required]
        [RegularExpression(ValidRelativeUrlPattern, ErrorMessage = "Do not start with '~/'")]
        [AssertThat("DestinationUrl != SourceUrl")]
        [ClientValidation]
        [Display(Name = "Destination URL")]
        public string DestinationUrl { get; set; }

        public bool IsPermanent { get; set; }

        public const string ValidRelativeUrlPattern = @"^[^\~\/\\].*";
    }

    public class ClientValidationAttribute : ValidationAttribute, IClientValidatable 
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rules = new List<ModelClientValidationRule>();
            var property = metadata.ContainerType.GetProperty(metadata.PropertyName);

            var assertThatAttributeType = typeof(AssertThatAttribute);
            if (IsDefined(property, assertThatAttributeType))
            {
                var provider = new AssertThatValidator(metadata, context, (AssertThatAttribute)GetCustomAttribute(property, assertThatAttributeType));
                rules.AddRange(provider.GetClientValidationRules());
            }

            var requiredIfAttributeType = typeof(RequiredIfAttribute);
            if (IsDefined(property, requiredIfAttributeType))
            {
                var provider = new RequiredIfValidator(metadata, context, (RequiredIfAttribute)GetCustomAttribute(property, requiredIfAttributeType));
                rules.AddRange(provider.GetClientValidationRules());
            }

            return rules;
        }
    }
}