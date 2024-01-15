using System.ComponentModel.DataAnnotations;

namespace Passwordless.AspNetIdentity.Example.Validation;

public class AlphanumericAttribute() : RegularExpressionAttribute("([a-zA-Z0-9]+)");