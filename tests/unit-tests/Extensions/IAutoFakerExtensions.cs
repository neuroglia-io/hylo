using AutoBogus;
using System.Reflection;

namespace Hylo.UnitTests;

public static class IAutoFakerExtensions
{

    static readonly MethodInfo GenericGenerateMethod = typeof(IAutoFaker).GetMethods().Single(m => m.Name == nameof(IAutoFaker.Generate) && m.GetGenericArguments()?.Length == 1 && m.GetParameters().Length == 1);

    public static object Generate(this IAutoFaker autoFaker, Type type)
    {
        return GenericGenerateMethod.MakeGenericMethod(type).Invoke(autoFaker, new object[] { null! })!;
    }

}
