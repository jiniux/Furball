using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Furball.Engine.Engine.Graphics.Drawables;
using Furball.Engine.Engine.Platform;
using Furball.Vixie.Graphics;
using ImGuiNET;
using Silk.NET.Input;
using TextCopy;

namespace Furball.Engine.Engine.DevConsole {
    public static class ImGuiConsole {
        private static string              _consoleBuffer = "";
        private static List<ConsoleResult> _devConsoleCache = new();

        private static BlankDrawable _consoleInputCoverDrawable;
        public static  bool          Visible = false;

        private static bool _scrollDown     = false;
        public static  bool AutoScroll      = true;

        private static int _cursorPosition = 0;
        private static int _selectionStart = 0;
        private static int _selectionEnd   = 0;

        public static string Title;

        public static void UpdateTitle() {
            //TODO: Figure out how to make this change without breaking the window
            Title = $"Console (Press CTRL to toggle auto-scroll!)";
        }
        
        public static void Initialize() {
            UpdateTitle();

            RefreshCache();

            FurballGame.InputManager.OnKeyDown += (_, key) => {
                if (key == Key.F12 && !Visible)
                    Visible = true;

                if (key == Key.ControlLeft) {
                    AutoScroll = !AutoScroll;
                    UpdateTitle();
                }
                if (Visible) {
                    if (key == Key.V && FurballGame.InputManager.ControlHeld && RuntimeInfo.CurrentPlatform() == OSPlatform.Linux) {
                        _consoleBuffer = _consoleBuffer.Insert(_cursorPosition, ClipboardService.GetText() ?? string.Empty);
                    }

                    if (key == Key.C && FurballGame.InputManager.ControlHeld && RuntimeInfo.CurrentPlatform() == OSPlatform.Linux) {
                        ClipboardService.SetText(_consoleBuffer.Substring(_selectionStart, _selectionEnd));
                    }
                }
            };

            DevConsole.ConsoleLog.CollectionChanged += (sender, args) => {
                RefreshCache();
                if(AutoScroll)
                    _scrollDown = true;
            };

            var style = ImGui.GetStyle();

            _consoleInputCoverDrawable = new BlankDrawable {
                CoverClicks = true,
                Clickable   = true,
                Visible     = true,
                CoverHovers = true,
                Hoverable   = true,
                Depth       = 0
            };

            FurballGame.DrawableManager.Add(_consoleInputCoverDrawable);

            style.ScrollbarRounding = 0;
            style.ChildRounding     = 0;
            style.FrameRounding     = 0;
            style.PopupRounding     = 0;
            style.WindowRounding    = 0;

            style.Colors[(int) ImGuiCol.WindowBg]         = new Vector4((40.0f / 255.0f), (5.0f / 255.0f),  (50.0f / 255.0f), 0.8f);
            style.Colors[(int) ImGuiCol.FrameBg]          = new Vector4((50.0f / 255.0f), (10.0f / 255.0f), (90.0f / 255.0f), 0.8f);
            style.Colors[(int) ImGuiCol.ChildBg]          = new Vector4((50.0f / 255.0f), (10.0f / 255.0f), (90.0f / 255.0f), 0.8f);
            style.Colors[(int) ImGuiCol.TitleBgActive]    = new Vector4((70.0f / 255.0f), (10.0f / 255.0f), (90.0f / 255.0f), 0.8f);
            style.Colors[(int) ImGuiCol.TitleBgCollapsed] = new Vector4((70.0f / 255.0f), (10.0f / 255.0f), (90.0f / 255.0f), 0.8f);

            style.WindowPadding = new Vector2(8, 8);
            style.ItemSpacing   = new Vector2(8, 8);
            style.FramePadding  = new Vector2(8, 4);
        }

        public static unsafe void Draw() {
            if (Visible) {
                ImGui.SetNextWindowSize(new Vector2(520, 600), ImGuiCond.FirstUseEver);
                ImGui.Begin(Title, ref Visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoCollapse);

                _consoleInputCoverDrawable.Position     = ImGui.GetWindowPos() / FurballGame.VerticalRatio;
                _consoleInputCoverDrawable.OverrideSize = ImGui.GetWindowSize() / FurballGame.VerticalRatio;

                float heightReserved = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

                ImGui.BeginChild("ScrollingRegion", new Vector2(0,           -heightReserved), true, ImGuiWindowFlags.HorizontalScrollbar);
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(4, 1));

                lock (_devConsoleCache) {
                    for (int i = 0; i != _devConsoleCache.Count; i++) {
                        ConsoleResult result = _devConsoleCache[i];

                        switch (result.Result) {
                            case ExecutionResult.Success:
                            case ExecutionResult.Log:
                            case ExecutionResult.Message:
                                ImGui.PushStyleColor(ImGuiCol.Text, Color.White.ToVector4F());
                                break;
                            case ExecutionResult.Warning:
                                ImGui.PushStyleColor(ImGuiCol.Text, Color.Yellow.ToVector4F());
                                break;
                            case ExecutionResult.Error:
                                ImGui.PushStyleColor(ImGuiCol.Text, Color.Red.ToVector4F());
                                break;
                        }

                        ImGui.TextWrapped(result.Message);
                        ImGui.PopStyleColor();
                    }
                }
                
                if (_scrollDown) {
                    ImGui.SetScrollY(ImGui.GetScrollMaxY() + 100);
                    _scrollDown = false;
                }

                ImGui.PopStyleVar();
                ImGui.EndChild();
                ImGui.Separator();

                ImGuiInputTextFlags textFlags = ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.CallbackCompletion | ImGuiInputTextFlags.CallbackHistory;

                ImGui.PushItemWidth(-1);

                //if (ImGui.InputText("", _consoleBuffer, (uint) _consoleBuffer.Length, textFlags, OnTextEdit)) {
                if (ImGui.InputText("", ref _consoleBuffer, 4096, textFlags, OnTextEdit)) {
                    DevConsole.Run(_consoleBuffer);

                    _consoleBuffer = string.Empty;

                    ImGui.SetKeyboardFocusHere(-1);
                    _scrollDown = true;
                }

                ImGui.PopItemWidth();
                ImGui.SetItemDefaultFocus();
                ImGui.End();
            }
        }

        private static unsafe int OnTextEdit(ImGuiInputTextCallbackData* data) {
            //Potentially maybe add some sort of autocomplete? i wont add it right now because volpe is gonna make us redo this entire thing anyway, but something for later
            //if (data->EventKey == ImGuiKey.Tab) {
            //
            //}

            _cursorPosition = data->CursorPos;
            _selectionStart = data->SelectionStart;
            _selectionEnd   = data->SelectionEnd;

            return 0;
        }

        private static void RefreshCache() {
            lock (_devConsoleCache) {
                _devConsoleCache.Clear();

                for (int i = 0; i != DevConsole.ConsoleLog.Count; i++) {
                    var current = DevConsole.ConsoleLog[i];

                    if (current.input != "") {
                        _devConsoleCache.Add(new ConsoleResult(ExecutionResult.Message, $"] {current.input}"));
                        _devConsoleCache.Add(current.Item2);
                    } else {
                        _devConsoleCache.Add(current.Item2);
                    }
                }
            }
        }
    }
}
