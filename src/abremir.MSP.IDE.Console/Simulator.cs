using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using abremir.MSP.IDE.Console.Views;
using abremir.MSP.VirtualMachine.Models;
using Terminal.Gui;

namespace abremir.MSP.IDE.Console
{
    internal class Simulator : Toplevel
    {
        public static event EventHandler<EventArgs>? UiInitialized;

        private const string SimulatorWindowTitle = "MSP Simulator";

        private readonly Window _simulatorWindow;
        private readonly SimulatorManager _simulatorManager;
        private readonly EditorFrame _editorFrame;

        private string? _filePath = null;

        public Simulator()
        {
            _simulatorManager = new SimulatorManager();

            ColorScheme = Colors.Base;

            var menu = new MenuBar(new MenuBarItem[]
            {
                new("_File", new MenuItem[]
                {
                    new("_New", "", () => New()),
                    new("_Open...", "", () => Open(), shortcut: Key.F3),
                    new("_Save", "", () => Save(), shortcut: Key.F2),
                    new("Save _as...", "", () => SaveAs()),
                    null!,
                    new("E_xit", "", () => Exit(), shortcut: Key.AltMask | Key.X),
                }),
                new("_Search", new MenuItem[]
                {
                    new("_Go to line number...", "", () => GoToLineNumber()),
                }),
                new("_Run", new MenuItem[]
                {
                    new("_Run", "", () => Run(), shortcut: Key.CtrlMask | Key.F9),
                    new("_Program reset", "", () => Reset(), () => _simulatorManager.VirtualMachine.PC != 0, shortcut: Key.CtrlMask | Key.F2)
                }),
                new("_Compile", new MenuItem[]
                {
                    new("_Compile", "", () => Compile(), shortcut: Key.AltMask | Key.F9)
                }),
                new("_Help", new MenuItem[]
                {
                    new("_MSP Help", "", () => MspHelp(), shortcut: Key.F1),
                    null!,
                    new("_About...", "", () => About())
                })
            });

            Add(menu);

            _simulatorWindow = new Window(SimulatorWindowTitle)
            {
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ColorScheme = ColorScheme
            };

            var siCursorPosition = new StatusItem(Key.Null, "", null);

            _editorFrame = new EditorFrame(_simulatorManager, siCursorPosition)
            {
                Width = Dim.Percent(31),
                Height = Dim.Fill()
            };

            _simulatorWindow.Add(_editorFrame);

            var virtualMachineFrame = new VirtualMachineFrame(_simulatorManager)
            {
                X = Pos.Right(_editorFrame),
                Width = Dim.Fill(),
                Height = Dim.Percent(75)
            };

            _simulatorWindow.Add(virtualMachineFrame);

            var outputFrame = new OutputFrame(_simulatorManager)
            {
                X = Pos.Right(_editorFrame),
                Y = Pos.Bottom(virtualMachineFrame),
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            _simulatorWindow.Add(outputFrame);

            Add(_simulatorWindow);

            var statusBar = new StatusBar(new StatusItem[] {
                siCursorPosition,
                new(Key.F3, "~F3~ Open", () => Open()),
                new(Key.F2, "~F2~ Save", () => Save()),
                new(Key.AltMask | Key.F9, "~Alt+F9~ Compile", () => Compile())
            });

            Add(statusBar);

            _simulatorManager.VirtualMachine.InputRequested += (object? _, InputRequestedEventArgs args) =>
            {
                var inputRequestDialog = new InputRequestDialog(args);

                Application.Run(inputRequestDialog);

                if (!inputRequestDialog.Cancelled)
                {
                    var value = inputRequestDialog.Value!;

                    _simulatorManager.Output($"{value}{Environment.NewLine}");

                    if (args.IsCharacter)
                    {
                        _simulatorManager.VirtualMachine.InputCharacter((byte)value[0]);
                    }
                    else
                    {
                        _simulatorManager.VirtualMachine.Input(sbyte.Parse(value));
                    }
                }
            };

            OnInitialized(this);
        }

        private static void OnInitialized(Simulator simulator)
        {
            UiInitialized?.Invoke(simulator, EventArgs.Empty);
        }

        private static int UnsavedChangesQuery(string title) => MessageBox.ErrorQuery(
            title,
            $"There are unsaved changes in the editor.{Environment.NewLine}Do you want to continue without saving?",
            "no",
            "yes");
        private static string GetWindowTitle(string subtitle) => $"{SimulatorWindowTitle} - {subtitle}";

        public void Initialize(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                New();
            }
            else
            {
                Open(filePath);
            }
        }

        private void New(bool skipCheckForDirty = false)
        {
            if (skipCheckForDirty
                || !_editorFrame.IsDirty
                || UnsavedChangesQuery("New file") == 1)
            {
                _editorFrame.New();
                _simulatorManager.New();
                _simulatorWindow.Title = GetWindowTitle("Untitled");
            }
        }

        private void Open(string? filePath = null)
        {
            if (!_editorFrame.IsDirty || UnsavedChangesQuery("Open a file") == 1)
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    var fileSelection = new OpenDialog("Open a file", "Load existing MSP file to the editor", new List<string> { "msp", "asm", "txt" })
                    {
                        DirectoryPath = Path.GetDirectoryName(_filePath ?? Assembly.GetEntryAssembly()!.Location)
                    };

                    Application.Run(fileSelection);

                    if (!fileSelection.Canceled && !string.IsNullOrWhiteSpace(fileSelection.FilePath?.ToString()))
                    {
                        filePath = fileSelection.FilePath.ToString();
                    }
                }

                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    if (!File.Exists(filePath))
                    {
                        MessageBox.ErrorQuery("Open a file", $"File not found!{Environment.NewLine}{GetShortenedFilePath(filePath)}", "ok");
                    }
                    else
                    {
                        _filePath = filePath;
                        string contents = File.ReadAllText(_filePath);

                        _simulatorWindow.Title = GetWindowTitle(GetShortenedFilePath(_filePath));
                        _editorFrame.New(contents);
                        _simulatorManager.New();
                    }
                }
            }
        }

        private void Save()
        {
            if (!_editorFrame.IsDirty)
            {
                return;
            }

            if (_filePath is null)
            {
                SaveAs();
            }
            else
            {
                SaveToFile(_filePath, _editorFrame.SourceCode!);
            }
        }

        private void SaveAs()
        {
            var fileSelection = new SaveDialog("Save file as...", "Save content of the editor to a new file", new List<string> { "msp", "asm", "txt", "*" });

            Application.Run(fileSelection);

            if (!fileSelection.Canceled && !string.IsNullOrWhiteSpace(fileSelection.FilePath?.ToString()))
            {
                var filePath = fileSelection.FilePath.ToString();

                bool overwrite = true;

                if (File.Exists(filePath))
                {
                    var result = MessageBox.ErrorQuery(
                        "Save as...",
                        $"File already exists. Do you want to overwrite it?{Environment.NewLine}{GetShortenedFilePath(filePath)}",
                        "no",
                        "yes");

                    overwrite = result == 1;
                }

                if (overwrite)
                {
                    _filePath = filePath!;
                    _simulatorWindow.Title = GetWindowTitle(GetShortenedFilePath(_filePath));

                    SaveToFile(_filePath, _editorFrame.SourceCode!);
                }
            }
        }

        private void Exit()
        {
            if (!_editorFrame.IsDirty || UnsavedChangesQuery("Exit simulator") == 1)
            {
                Application.RequestStop();
            }
        }

        private void Compile()
        {
            _simulatorManager.Compile(_editorFrame.SourceCode);
        }

        private void Run()
        {
            _simulatorManager.Run(_editorFrame.SourceCode);
        }

        private void Reset()
        {
            _simulatorManager.Reset();
        }

        private void SaveToFile(string filePath, string contents)
        {
            File.WriteAllText(filePath, contents);

            _editorFrame.SourceCodeSaved();
        }

        private void GoToLineNumber()
        {
            var goToLineNumberDialog = new GoToLineNumberDialog(_editorFrame.Lines);

            Application.Run(goToLineNumberDialog);

            if (!goToLineNumberDialog.Cancelled)
            {
                _editorFrame.GoToLine(goToLineNumberDialog.Value!.Value);
            }
        }

        private static void About()
        {
            var aboutMessage = new StringBuilder();
            aboutMessage.AppendLine("MSP (Mais Simples Possível) Simulator");
            aboutMessage.AppendLine();
            aboutMessage.AppendLine("An IDE and emulator for the MSP assembly programming language.");
            aboutMessage.AppendLine();
            aboutMessage.AppendLine("Copyright (c) 2023 José Mira");
            aboutMessage.AppendLine();
            aboutMessage.AppendLine(SimulatorConstants.RepositoryRoot);

            var border = new Border()
            {
                Effect3D = false,
                BorderStyle = BorderStyle.Single
            };

            MessageBox.Query("About MSP Simulator", aboutMessage.ToString(), 0, border, "close");
        }

        private static void MspHelp()
        {
            var helpDialog = new HelpDialog();

            Application.Run(helpDialog);
        }

        private static string GetShortenedFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || filePath.Length <= 50)
            {
                return filePath;
            }

            var filePathSegments = filePath.Split(Path.DirectorySeparatorChar);

            if (filePathSegments.Length < 4)
            {
                return filePath;
            }

            var fileName = filePathSegments[^1];
            var startSegments = new List<string> { filePathSegments[0] };
            var endSegments = new List<string>();
            var middleSegment = new List<string> { "..." };

            var startIndex = 1;
            var endIndex = filePathSegments.Length - 2;

            // unix/linux paths may start with /, which means the first segment will be empty!
            if (string.IsNullOrEmpty(filePathSegments[0]))
            {
                startSegments.Add(filePathSegments[1]);
                startIndex++;
            }

            string? shortenedFilePath;
            bool toggle = true;
            do
            {
                if (toggle)
                {
                    startSegments.Add(filePathSegments[startIndex]);
                    startIndex++;
                }

                if (!toggle)
                {
                    endSegments.Insert(0, filePathSegments[endIndex]);
                    endIndex--;
                }

                toggle = !toggle;

                var shortenedFilePathSegments = new List<string>(startSegments);
                shortenedFilePathSegments.AddRange(middleSegment);
                shortenedFilePathSegments.AddRange(endSegments);
                shortenedFilePathSegments.Add(fileName);

                shortenedFilePath = string.Join(Path.DirectorySeparatorChar, shortenedFilePathSegments);
            } while (shortenedFilePath.Length < 50);

            return shortenedFilePath;
        }
    }
}
