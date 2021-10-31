using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using SecretSanta.Domain;

var app = new CommandLineApplication
          {
                  AllowArgumentSeparator = true,
                  UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect,
          };

app.HelpOption();
var santasPath = app.Option ("-s|--santas <SUBJECT>", "Path to file containing santas", CommandOptionType.SingleValue)
        .IsRequired();

app.OnExecute (
        () =>
        {
            var secretSantaService = new SecretSantaService();
            var santas = ReadSantas (santasPath).ToArray();
            var assignments = secretSantaService.AssignSantas(santas);
        });

return app.Execute (args);

IEnumerable<Santa> ReadSantas (CommandOption commandOption)
{
    var santaLines = File.ReadLines (commandOption.Value()!);
    foreach (var santaLine in santaLines)
    {
        var santa = santaLine.Split (",");
        yield return new Santa { Name = santa[0], Email = santa[1] };
    }
}