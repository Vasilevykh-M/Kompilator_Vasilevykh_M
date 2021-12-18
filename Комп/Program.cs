using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;

namespace Комп
{
    class Program
    {
        static void Main(string[] args)
        {
            String  assemblyName = "Expression";
            String modName = "Program.exe";
            String typeName = "Expression";
            String methodName = "Main";

            AssemblyName name = new AssemblyName(assemblyName);


            AssemblyBuilder builder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndSave);

            ModuleBuilder module = builder.DefineDynamicModule(modName, true);

            TypeBuilder typeBuilder = module.DefineType(typeName, TypeAttributes.Public);
            Type[] parameterTypes = { typeof(int) };
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodName, MethodAttributes.Static
                | MethodAttributes.Public, null, new Type[] { });

            ILGenerator ilGenerator = methodBuilder.GetILGenerator();

            //var x = ilGenerator.DeclareLocal(typeof(double));
            //ilGenerator.Emit(OpCodes.Ldc_R8, (double)4.5);
            //ilGenerator.Emit(OpCodes.Ldc_R8, (double)4.5);
            ////ilGenerator.Emit(OpCodes.Add);
            //ilGenerator.Emit(OpCodes.Div);

            //ilGenerator.Emit(OpCodes.Stloc, x);

            //ilGenerator.EmitWriteLine(x);

            //ilGenerator.Emit(OpCodes.Ret);

            //builder.SetEntryPoint(methodBuilder);

            //Type myClass = typeBuilder.CreateType();
            //builder.Save(modName);
            CEXPR a = new CEXPR() { ilGenerator = ilGenerator };
            a.C_Programm();

            ilGenerator.Emit(OpCodes.Ret);

            builder.SetEntryPoint(methodBuilder);

            Type myClass = typeBuilder.CreateType();
            if (a.leks.C_ERR.Count() == 0)
                builder.Save(modName);

        }
    }
}
