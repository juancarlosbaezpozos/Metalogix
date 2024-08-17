using Metalogix.Actions;
using System;

namespace Metalogix.DataStructures
{
    public interface IComparable
    {
        bool IsEqual(Metalogix.DataStructures.IComparable targetComparable, DifferenceLog differencesOutput,
            ComparisonOptions options);
    }
}