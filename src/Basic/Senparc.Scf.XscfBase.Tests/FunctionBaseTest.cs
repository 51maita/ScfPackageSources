using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Senparc.Scf.XscfBase.Tests
{
    public class FunctionBaseTest_Function : FunctionBase
    {
        public FunctionBaseTest_Function(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override string Name => "���Է���";

        public override string Description => "���Է���˵��";

        public override Type FunctionParameterType => typeof(FunctionBaseTest_FunctionParameter);

        public override string Run(IFunctionParameter param)
        {
            Console.WriteLine("Run");
            return "OK";
        }
    }

    public class FunctionBaseTest_FunctionParameter : IFunctionParameter
    {
        [Required]
        [MaxLength(300)]
        [System.ComponentModel.Description("·��||��������·�����磺E:\\Senparc\\Scf\\")]
        public string Path { get; set; }

        [MaxLength(100)]
        [System.ComponentModel.Description("�������ռ�||�����ռ����������.��β�������滻[Senparc.Scf.]")]
        public string NewNamespace { get; set; }
    }

    [TestClass]
    public class FunctionBaseTest
    {
        [TestMethod]
        public void GetFunctionParammeterInfo()
        {
            FunctionBaseTest_Function function = new FunctionBaseTest_Function(null);
            var paraInfo = function.GetFunctionParammeterInfo().ToList();

            Assert.AreEqual(2, paraInfo.Count);

            Assert.AreEqual("Path", paraInfo[0].Name);
            Assert.AreEqual("·��", paraInfo[0].Title);
            Assert.AreEqual("��������·�����磺E:\\Senparc\\Scf\\", paraInfo[0].Description);
            Assert.AreEqual(true, paraInfo[0].IsRequired);
            Assert.AreEqual("String", paraInfo[0].SystemType);

            Assert.AreEqual("NewNamespace", paraInfo[1].Name);
            Assert.AreEqual("�������ռ�", paraInfo[1].Title);
            Assert.AreEqual("�����ռ����������.��β�������滻[Senparc.Scf.]", paraInfo[1].Description);
        }
    }
}
