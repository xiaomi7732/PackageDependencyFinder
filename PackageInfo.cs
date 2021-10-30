namespace PackageDependencyFinder
{
    public class PackageInfo
    {
        public string Name { get; set; }
        public string VersionExpression { get; set; }

        public string Referenced { get; set; }
    }
}