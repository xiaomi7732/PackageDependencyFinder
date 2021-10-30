using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PackageDependencyFinder
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Specify path to start project.");
                return;
            }
            string startPoint = args[0];

            if (!File.Exists(startPoint))
            {
                Console.WriteLine(@"File not found at: {0}", startPoint);
                return;
            }

            List<PackageInfo> packageList = new List<PackageInfo>();
            await foreach (PackageInfo packageInfo in GetPackageInfoAsync(startPoint).ConfigureAwait(false))
            {
                packageList.Add(packageInfo);
            }

            Console.WriteLine("Brief");
            foreach (PackageInfo packageInfo in packageList.OrderBy(item => item.Name).Distinct(new PackageInfoComparerNoSource()))
            {
                Console.WriteLine("{0,-60}\t{1,20}", packageInfo.Name, packageInfo.VersionExpression);
            }
            Console.WriteLine("About to output detailed reference info. Press any key to continue. Ctrl + C to break");
            Console.ReadKey(intercept: true);
            Console.WriteLine("Details:");
            foreach (PackageInfo packageInfo in packageList.OrderBy(item => item.Name))
            {
                Console.WriteLine("{0,-60}\t\t{1,-16}{2}", packageInfo.Name, packageInfo.VersionExpression, packageInfo.Referenced);
            }
        }

        private static async IAsyncEnumerable<PackageInfo> GetPackageInfoAsync(string projectFile)
        {
            string baseDirectory = Path.GetDirectoryName(projectFile);
            Queue<string> workingFiles = new Queue<string>();
            HashSet<string> workedFiles = new HashSet<string>();
            // Dictionary<string, string> packageList = new Dictionary<string, string>();
            workingFiles.Enqueue(projectFile);

            while (workingFiles.TryDequeue(out string workingFile))
            {
                baseDirectory = Path.GetDirectoryName(workingFile);
                using (Stream xmlFileStream = File.OpenRead(workingFile))
                {
                    XElement root = await XElement.LoadAsync(xmlFileStream, LoadOptions.None, default).ConfigureAwait(false);
                    foreach (XElement itemGroupElement in root.Elements("ItemGroup"))
                    {
                        foreach (XElement packageReferenceElement in itemGroupElement.Elements("PackageReference"))
                        {
                            string packageName = packageReferenceElement.Attribute("Include").Value.ToLowerInvariant();
                            yield return new PackageInfo()
                            {
                                Name = packageName,
                                VersionExpression = packageReferenceElement.Attribute("Version").Value,
                                Referenced = workingFile,
                            };
                        }
                        foreach (XElement projectReferenceElement in itemGroupElement.Elements("ProjectReference"))
                        {
                            string absolutePath = Path.GetFullPath(Path.Combine(baseDirectory, projectReferenceElement.Attribute("Include").Value));
                            if (!workedFiles.Contains(absolutePath))
                            {
                                workingFiles.Enqueue(absolutePath);
                            }
                        }
                    }
                }
                workedFiles.Add(workingFile);
            }
        }
    }
}
