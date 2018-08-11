using System;
using System.Reflection;
using System.Reflection.Emit;

namespace EmitIntroduction
{
  class Program
  {
    static void Main(string[] args)
    {
      // specify a new assembly name
      var assemblyName = new AssemblyName("Kitty");

      // create assembly builder
      var assemblyBuilder = AppDomain.CurrentDomain
        .DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);

      // create module builder
      var moduleBuilder = assemblyBuilder.DefineDynamicModule("KittyModule", "Kitty.exe");

      // create type builder for a class
      var typeBuilder = moduleBuilder.DefineType("HelloKittyClass", TypeAttributes.Public);

      // create method builder
      var methodBuilder = typeBuilder.DefineMethod(
        "SayHelloMethod",
        MethodAttributes.Public | MethodAttributes.Static,
        null,
        null);

      // then get the method il generator
      var il = methodBuilder.GetILGenerator();

      // then create the method function
      il.Emit(OpCodes.Ldstr, "Hello, Kitty!");
      il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
      il.Emit(OpCodes.Call, typeof(Console).GetMethod("ReadLine"));
      il.Emit(OpCodes.Pop); // we just read something here, throw it.
      il.Emit(OpCodes.Ret);

      // then create the whole class type
      var helloKittyClassType = typeBuilder.CreateType();

      // set entry point for this assembly
      assemblyBuilder.SetEntryPoint(helloKittyClassType.GetMethod("SayHelloMethod"));

      // save assembly
      assemblyBuilder.Save("Kitty.exe");

      Console.WriteLine("Hi, Dennis, a Kitty assembly has been generated for you.");
      Console.ReadLine();
    }
  }
}
