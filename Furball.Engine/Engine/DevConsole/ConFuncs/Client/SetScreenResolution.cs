namespace Furball.Engine.Engine.DevConsole.ConFuncs.Client {
    /// <summary>
    /// `cl_set_screen_resolution`
    /// Changes the Screen Resolution
    /// Syntax: `cl_set_screen_resolution width height`
    /// </summary>
    public class SetScreenResolution : ConFunc {
        public SetScreenResolution() : base("cl_set_screen_resolution") {}

        public override ConsoleResult Run(string[] consoleInput) {
            string input = consoleInput[0];
            
            if (input.Trim().Length != 0)
                ConVars.ScreenResolution.Set(input.Trim());

            (int width, int height) = ConVars.ScreenResolution.Value;

            FurballGame.Instance.ChangeScreenSize(width, height);

            return new ConsoleResult(ExecutionResult.Success, $"Resolution has been set to {width}x{height}");
        }
    }
}
