using System;

namespace Metalogix.Explorer
{
    public interface IManagableField : Field
    {
        bool DefaultVisible { get; }

        bool Visible { get; set; }
    }
}