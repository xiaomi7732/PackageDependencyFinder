using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PackageDependencyFinder
{
    public class PackageInfoComparerNoSource : IEqualityComparer<PackageInfo>
    {
        public bool Equals(PackageInfo x, PackageInfo y)
        {
            if (Object.ReferenceEquals(x, y))
                return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            if (string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(x.VersionExpression, y.VersionExpression, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public int GetHashCode([DisallowNull] PackageInfo obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return 0;

            int hashName = obj.Name is null ? 0 : obj.Name.GetHashCode();
            int hashVersion = obj.VersionExpression is null ? 0 : obj.VersionExpression.GetHashCode();

            return hashName ^ hashVersion;
        }
    }
}