namespace System.Runtime.CompilerServices;

#if !NET5_0_OR_GREATER
// https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.isexternalinit
internal static class IsExternalInit;
#endif

#if !NET7_0_OR_GREATER
// https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.requiredmemberattribute
[AttributeUsage(
    AttributeTargets.Class
    | AttributeTargets.Field
    | AttributeTargets.Property
    | AttributeTargets.Struct,
    Inherited = false
)]
internal class RequiredMemberAttribute : Attribute;

// https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.compilerfeaturerequiredattribute
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
internal partial class CompilerFeatureRequiredAttribute : Attribute
{
    public string FeatureName { get; }

    public bool IsOptional { get; init; }

    public CompilerFeatureRequiredAttribute(string featureName) => FeatureName = featureName;
}

internal partial class CompilerFeatureRequiredAttribute
{
    public const string RefStructs = nameof(RefStructs);

    public const string RequiredMembers = nameof(RequiredMembers);
}
#endif