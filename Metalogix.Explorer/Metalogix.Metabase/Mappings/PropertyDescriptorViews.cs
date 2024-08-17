using Metalogix.Data.Mapping;
using System;
using System.ComponentModel;
using System.Reflection;

namespace Metalogix.Metabase.Mappings
{
    public class PropertyDescriptorViews : ListView<PropertyDescriptor>
    {
        public override string Name
        {
            get { return "Internal Name"; }
        }

        public PropertyDescriptorViews()
        {
        }

        public override string Render(PropertyDescriptor item)
        {
            return item.Name;
        }

        public override string RenderType(PropertyDescriptor item)
        {
            return item.PropertyType.Name;
        }
    }
}