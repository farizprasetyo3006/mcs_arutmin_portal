using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Threading.Tasks;

namespace MCSScript
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        private static async Task<object> Exec(string code, object globals = null,
            string[] references = null, string imports = null)
        {
            try
            {
                object result = null;
                ScriptOptions options = null;
                if (references != null)
                {
                    options =
                        ScriptOptions.Default.WithReferences(references);
                }
                if (imports != null)
                {
                    if (options == null)
                        options = ScriptOptions.Default.WithImports(imports);
                    else options = options.WithImports(imports);
                }
                result = await CSharpScript.EvaluateAsync(code,
                    options, globals: globals);
                //Evaluation result
                return result;
            }
            catch (CompilationErrorException e)
            {
                //Returns full compilation error 
                return string.Join(Environment.NewLine, e.Diagnostics);
            }
        }
    }
}
