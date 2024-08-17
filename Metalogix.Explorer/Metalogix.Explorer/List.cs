using System;

namespace Metalogix.Explorer
{
    public interface List : Folder
    {
        bool EnableVersioning { get; }

        FieldCollection Fields { get; }
    }
}