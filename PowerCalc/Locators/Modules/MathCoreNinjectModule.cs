using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TAlex.MathCore.ExpressionEvaluation.Trees.Builders;
using TAlex.MathCore.ExpressionEvaluation.Trees.Metadata;


namespace TAlex.PowerCalc.Locators.Modules
{
    public class MathCoreNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ConstantFlyweightFactory<Object>>()
                .ToSelf()
                .InSingletonScope()
                .OnActivation((c, i) => { i.AddFromAssemblies(GetAssembliesFromPath(Properties.Settings.Default.ExtensionsPath)); });
            Bind<IConstantFactory<Object>>().ToMethod(ctx => ctx.Kernel.Get<ConstantFlyweightFactory<Object>>());
            Bind<IConstantsMetadataProvider>().ToMethod(ctx => ctx.Kernel.Get<ConstantFlyweightFactory<Object>>());

            Bind<FunctionFactory<Object>>()
                .ToSelf()
                .InSingletonScope()
                .OnActivation((c, i) => { i.AddFromAssemblies(GetAssembliesFromPath(Properties.Settings.Default.ExtensionsPath)); });
            Bind<IFunctionFactory<Object>>().ToMethod(ctx => ctx.Kernel.Get<FunctionFactory<Object>>());
            Bind<IFunctionsMetadataProvider>().ToMethod(ctx => ctx.Kernel.Get<FunctionFactory<Object>>());

            Bind<IExpressionTreeBuilder<Object>>()
                .To<ComplexExpressionTreeBuilder>()
                .InSingletonScope()
                .OnActivation((c, i) =>
                {
                    i.ConstantFactory = c.Kernel.Get<IConstantFactory<Object>>();
                    i.FunctionFactory = c.Kernel.Get<IFunctionFactory<Object>>();
                });
        }


        private static IEnumerable<Assembly> GetAssembliesFromPath(string path)
        {
            string[] files = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path), "*.dll", SearchOption.TopDirectoryOnly);

            foreach (string filePath in files)
            {
                yield return Assembly.LoadFile(filePath);
            }
        }
    }
}
