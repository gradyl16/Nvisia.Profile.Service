using System.Text;
using Serilog.Events;
using Serilog.Formatting;

namespace Nvisia.Profile.Service.Api.Formatters;

public class ParsedMessageFormatter : ITextFormatter
{
    private readonly ITextFormatter _textFormatter;

    public ParsedMessageFormatter(ITextFormatter textFormatter)
    {
        _textFormatter = textFormatter;
    }

    public void Format(LogEvent? logEvent, TextWriter output)
    {
        if (logEvent == null) return;
        var buffer = new StringWriter(new StringBuilder());
        _textFormatter.Format(logEvent, buffer);
        var parsed = buffer.ToString().Replace(Environment.NewLine, " ");
        output.WriteLine(parsed);
        output.Flush();
    }
}