using System;
using System.CommandLine;
using abremir.MSP.IDE.Console;
using Terminal.Gui;

var pathOption = new Option<string>(new[] { "-f", "--file" }, description: "File to load");

var rootCommand = new RootCommand
{
    pathOption
};

rootCommand.Description = "MSP Simulator";

rootCommand.SetHandler((string filePath) =>
{
    Simulator.UiInitialized += (object? sender, EventArgs _) => (sender as Simulator)?.Initialize(filePath);

    Application.Run<Simulator>();
}, pathOption);

return rootCommand.Invoke(args);