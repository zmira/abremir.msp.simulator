using abremir.MSP.Compiler.Models;

namespace abremir.MSP.Compiler
{
    public interface ICompiler
    {
        CompilerResult Compile(string? source);
    }
}
