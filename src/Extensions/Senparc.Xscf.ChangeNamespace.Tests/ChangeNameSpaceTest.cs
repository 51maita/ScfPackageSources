using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Senparc.Xscf.ChangeNamespace.Functions;

namespace Senparc.Xscf.ChangeNamespace.Tests
{
    [TestClass]
    public class ChangeNameSpaceTest
    {
        [TestMethod]
        public void RunTest()
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();
            var function = new Functions.ChangeNamespace(serviceProvider);
            var path = @"E:\Senparc��Ŀ\SenparcCoreFramework\ScfPackageSources\src\Extensions\Senparc.Xscf.ChangeNamespace.Tests\App_Data\src";
            var newNameSpace = "This.Is.NewNamespace.";
            var result = function.Run(path, newNameSpace);

            System.Console.WriteLine(result);
        }
    }
}
