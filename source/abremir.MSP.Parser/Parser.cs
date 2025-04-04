using abremir.MSP.Parser.Models;
using abremir.MSP.Parser.Parsers;
using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using Superpower;

namespace abremir.MSP.Parser
{
    public class Parser(
        ITokenizer tokenizer,
        ITokenListParser tokenListParser) : IParser
    {
        private readonly ITokenizer _tokenizer = tokenizer;
        private readonly ITokenListParser _tokenListParser = tokenListParser;

        public ParserResult Parse(string source)
        {
            var parserResult = new ParserResult();

            try
            {
                var tokenListParserResult = _tokenListParser.Parse(_tokenizer.Tokenize(source));

                if (tokenListParserResult.Expectations is not null)
                {
                    foreach (var expectation in tokenListParserResult.Expectations)
                    {
                        if (Enum.TryParse<Error>(expectation, true, out var mspError))
                        {
                            parserResult.AddError(new(mspError, tokenListParserResult.ErrorPosition.Line, tokenListParserResult.ErrorPosition.Column));
                        }
                        else
                        {
                            parserResult.AddError(new(Error.UnknownToken, tokenListParserResult.ErrorPosition.Line, tokenListParserResult.ErrorPosition.Column));
                        }
                    }
                }

                if (parserResult.Errors.Count is not 0 || !tokenListParserResult.HasValue)
                {
                    return parserResult;
                }

                parserResult.SetData(tokenListParserResult.Value.Data?.OrderBy(data => data.Address).ToList());
                parserResult.SetInstructions(tokenListParserResult.Value.Instructions?.OrderBy(instruction => instruction.LineNumber).ToList());

                return parserResult;
            }
            catch (ParseException parseException)
            {
                var codeLines = source.Split(Environment.NewLine);
                var exceptionLine = parseException.ErrorPosition.HasValue && parseException.ErrorPosition.Line <= codeLines.Length
                    ? codeLines[parseException.ErrorPosition.Line - 1]
                    : string.Empty;
                var error = Error.SyntaxError;

                if (exceptionLine.Contains($"{Operation.Jump.GetDescription()} ", StringComparison.OrdinalIgnoreCase)
                    || exceptionLine.Contains($"{Operation.JumpIfFalse.GetDescription()} ", StringComparison.OrdinalIgnoreCase)
                    || exceptionLine.Contains($"{Operation.Call.GetDescription()} ", StringComparison.OrdinalIgnoreCase))
                {
                    error = Error.CodeBranchInvalidArgument;
                }
                else if (exceptionLine.Contains($"{Operation.PushAddress.GetDescription()} ", StringComparison.OrdinalIgnoreCase))
                {
                    error = Error.CodePshaInvalidArgument;
                }
                else if (exceptionLine.Contains($"{Operation.PushValue.GetDescription()} ", StringComparison.OrdinalIgnoreCase))
                {
                    error = Error.CodePushArgumentOutsideAllowedRange;
                }
                else if (exceptionLine.Contains("TAM", StringComparison.OrdinalIgnoreCase) && exceptionLine.Contains("VAL", StringComparison.OrdinalIgnoreCase))
                {
                    error = Error.DataUnexpectedInitializationValues;
                }

                parserResult.AddError(new(error, parseException.ErrorPosition.Line, parseException.ErrorPosition.Column, parseException.Message));

                return parserResult;
            }
            catch (Exception exception)
            {
                parserResult.AddError(new(Error.Exception, ErrorMessage: exception.Message));

                return parserResult;
            }
        }
    }
}
