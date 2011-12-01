using System.Collections.Generic;
using System.Configuration;

namespace NConfig
{
    /// <summary>
    /// Sections merger which performs configuration property-by-property merge.
    /// </summary>
    /// <typeparam name="TSection">Configuration section type.</typeparam>
    public class PropertyMerger<TSection> : NSectionMerger<TSection>
        where TSection : ConfigurationSection, new()
    {
        /// <summary>
        /// Merges the specified typed configuration sections.
        /// </summary>
        /// <param name="sections">The sections to merge in order from the most important to lower.</param>
        /// <returns>The merge resulting section.</returns>
        public override TSection Merge(IEnumerable<TSection> sections)
        {
            var result = new TSection();

            foreach (PropertyInformation resultPropery in result.ElementInformation.Properties)
            {
                foreach (TSection section in sections) // The order of sections should be from the most important to lower
                {
                    var sectionProperty = section.ElementInformation.Properties[resultPropery.Name];
                    if (sectionProperty == null)
                        break;

                    resultPropery.Value = sectionProperty.Value;
                    if (IsConfigurationPropertyDefined(sectionProperty))
                        // leave the value of first defined in config file property.
                        break;
                }
            }

            return result;
        }


        private bool IsConfigurationPropertyDefined(PropertyInformation property)
        {
            return property.ValueOrigin == PropertyValueOrigin.SetHere;
        }


    }
}
