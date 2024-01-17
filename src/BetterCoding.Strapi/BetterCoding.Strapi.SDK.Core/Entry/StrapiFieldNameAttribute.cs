namespace BetterCoding.Strapi.SDK.Core.Entry
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class StrapiFieldNameAttribute : Attribute
    {
        public StrapiFieldNameAttribute(string fieldName) => FieldName = fieldName;

        /// <summary>
        /// Gets the name of the field represented by this property.
        /// </summary>
        public string FieldName { get; private set; }
    }
}
