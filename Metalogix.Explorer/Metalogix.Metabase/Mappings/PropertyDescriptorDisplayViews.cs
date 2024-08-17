using Metalogix.Data.Mapping;
using System;
using System.ComponentModel;
using System.Reflection;

namespace Metalogix.Metabase.Mappings
{
    [Default]
    public class PropertyDescriptorDisplayViews : ListView<PropertyDescriptor>
    {
        public override string Name
        {
            get { return "Name"; }
        }

        public PropertyDescriptorDisplayViews()
        {
        }

        public override string Render(PropertyDescriptor item)
        {
            return item.DisplayName;
        }

        public override string RenderType(PropertyDescriptor item)
        {
            return item.PropertyType.Name;
        }
    }
}