// https://github.com/data-bomb/FixNullableAttribute/blob/main/FixNullableAttribute.cs

// ReSharper disable once CheckNamespace
// namespace System.Runtime.CompilerServices
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event
        | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter,
        Inherited = false
    )]
    public sealed class NullableAttribute : Attribute
    {
        public readonly byte[] NullableFlags;

        public NullableAttribute(byte value) => NullableFlags = [value];
        public NullableAttribute(byte[] value) => NullableFlags = value;
    }
}