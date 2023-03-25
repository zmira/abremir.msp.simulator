using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Terminal.Gui;

namespace abremir.MSP.IDE.Console.Views
{
    internal class HelpDialog : Dialog
    {
        public HelpDialog()
        {
            var mspHelp = new Help(LoadMspHelp());

            Title = "MSP Help";
            Border.Effect3D = false;
            ButtonAlignment = ButtonAlignments.Center;
            Width = Dim.Percent(90);
            Height = Dim.Percent(80);

            var mspHelpTreeFrame = new FrameView
            {
                Width = Dim.Percent(40),
                Height = Dim.Fill() - 1
            };

            var mspHelpItemFrame = new FrameView
            {
                X = Pos.Right(mspHelpTreeFrame),
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1,
            };

            var mspHelpTree = new TreeView()
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                DesiredCursorVisibility = CursorVisibility.Invisible,
                AllowLetterBasedNavigation = false
            };

            mspHelpTree.Style.LeaveLastRow = true;
            mspHelpTree.Style.ExpandableSymbol = '►';
            mspHelpTree.Style.CollapseableSymbol = '▼';

            mspHelpTree.AddObject(mspHelp);

            mspHelpTree.SelectionChanged += (sender, e) => DisplayHelpDetails(mspHelpItemFrame, e.NewValue as HelpItem);

            mspHelpTreeFrame.Add(mspHelpTree);

            Add(mspHelpTreeFrame);
            Add(mspHelpItemFrame);

            mspHelpTree.GoToFirst();
            mspHelpTree.Expand();

            var mspHelperTreeScrollBar = new ScrollBarView(mspHelpTree, true);

            mspHelperTreeScrollBar.ChangedPosition += () =>
            {
                mspHelpTree.ScrollOffsetVertical = mspHelperTreeScrollBar.Position;
                if (mspHelpTree.ScrollOffsetVertical != mspHelperTreeScrollBar.Position)
                {
                    mspHelperTreeScrollBar.Position = mspHelpTree.ScrollOffsetVertical;
                }
                mspHelpTree.SetNeedsDisplay();
            };

            mspHelperTreeScrollBar.OtherScrollBarView.ChangedPosition += () =>
            {
                mspHelpTree.ScrollOffsetHorizontal = mspHelperTreeScrollBar.OtherScrollBarView.Position;
                if (mspHelpTree.ScrollOffsetHorizontal != mspHelperTreeScrollBar.OtherScrollBarView.Position)
                {
                    mspHelperTreeScrollBar.OtherScrollBarView.Position = mspHelpTree.ScrollOffsetHorizontal;
                }
                mspHelpTree.SetNeedsDisplay();
            };

            mspHelpTree.DrawContent += (e) =>
            {
                mspHelperTreeScrollBar.Size = mspHelpTree.ContentHeight;
                mspHelperTreeScrollBar.Position = mspHelpTree.ScrollOffsetVertical;
                mspHelperTreeScrollBar.OtherScrollBarView.Size = mspHelpTree.GetContentWidth(true);
                mspHelperTreeScrollBar.OtherScrollBarView.Position = mspHelpTree.ScrollOffsetHorizontal;
                mspHelperTreeScrollBar.Refresh();
            };

            mspHelpTree.SetFocus();

            var closeButton = new Button("close", true);

            closeButton.Clicked += () => Application.RequestStop(this);

            AddButton(closeButton);
        }

        private IReadOnlyCollection<HelpItem> LoadMspHelp()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var mspHelpResourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("msp-help.json"));

            using var stream = assembly.GetManifestResourceStream(mspHelpResourceName);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Deserialize<IReadOnlyCollection<HelpItem>>(stream!, options)!;
        }

        private void DisplayHelpDetails(FrameView mspHelpItemView, HelpItem? helpItem)
        {
            mspHelpItemView.RemoveAll();

            if (helpItem is null)
            {
                return;
            }

            var mspHelpTitleLabel = new Label($"{(helpItem.Code is null ? string.Empty : $"{helpItem.Code} - ")}{helpItem.Title}")
            {
                X = 1,
                Height = 2,
                Width = Dim.Fill(),
                AutoSize = false
            };

            mspHelpItemView.Add(mspHelpTitleLabel);

            var mspHelloItemTextView = helpItem.Syntax is not null || helpItem.Description is not null
                ? new TextView
                {
                    X = 1,
                    Y = Pos.Bottom(mspHelpTitleLabel),
                    Width = Dim.Fill(1),
                    Height = Dim.Fill() - (helpItem.Url is null ? 0 : 2),
                    ReadOnly = true,
                    WordWrap = true
                }
                : null;

            if (mspHelloItemTextView is not null)
            {
                mspHelloItemTextView.Text = $"{helpItem.Description ?? string.Empty}{(helpItem.Description is not null ? $"{Environment.NewLine}{Environment.NewLine}" : string.Empty)}{helpItem.Syntax ?? string.Empty}";

                mspHelpItemView.Add(mspHelloItemTextView);

                var mspHelpItemTextViewScrollBar = new ScrollBarView(mspHelloItemTextView, true);

                mspHelpItemTextViewScrollBar.ChangedPosition += () =>
                {
                    mspHelloItemTextView.TopRow = mspHelpItemTextViewScrollBar.Position;
                    if (mspHelloItemTextView.TopRow != mspHelpItemTextViewScrollBar.Position)
                    {
                        mspHelpItemTextViewScrollBar.Position = mspHelloItemTextView.TopRow;
                    }
                    mspHelloItemTextView.SetNeedsDisplay();
                };

                mspHelpItemTextViewScrollBar.VisibleChanged += () =>
                {
                    if (mspHelpItemTextViewScrollBar.Visible && mspHelloItemTextView.RightOffset == 0)
                    {
                        mspHelloItemTextView.RightOffset = 1;
                    }
                    else if (!mspHelpItemTextViewScrollBar.Visible && mspHelloItemTextView.RightOffset == 1)
                    {
                        mspHelloItemTextView.RightOffset = 0;
                    }
                };

                mspHelloItemTextView.DrawContent += (_) =>
                {
                    mspHelpItemTextViewScrollBar.Size = mspHelloItemTextView.Lines;
                    mspHelpItemTextViewScrollBar.Position = mspHelloItemTextView.TopRow;
                    if (mspHelpItemTextViewScrollBar.OtherScrollBarView != null)
                    {
                        mspHelpItemTextViewScrollBar.OtherScrollBarView.Size = mspHelloItemTextView.Maxlength;
                        mspHelpItemTextViewScrollBar.OtherScrollBarView.Position = mspHelloItemTextView.LeftColumn;
                    }
                    mspHelpItemTextViewScrollBar.LayoutSubviews();
                    mspHelpItemTextViewScrollBar.Refresh();
                };
            }

            var documentationButton = helpItem.Url is not null
                ? new Label($"{SimulatorConstants.Documentation}{helpItem.Url}")
                {
                    X = 1,
                    Y = mspHelloItemTextView is not null ? Pos.Bottom(mspHelloItemTextView) : Pos.Bottom(mspHelpTitleLabel),
                    Height = 2,
                    Width = Dim.Fill(),
                    AutoSize = false
                }
                : null;

            if (documentationButton is not null)
            {
                mspHelpItemView.Add(documentationButton);
            }
        }
    }
}
