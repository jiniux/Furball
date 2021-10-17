namespace Furball.Engine.Engine.DevConsole.ConFuncs.Client {
    public class ClearLog : ConFunc {
        public ClearLog() : base("cl_clear_log") {}

        public override ConsoleResult Run(string consoleInput) {
            if (DevConsole.ConsoleLog.Count == 0)
                return new ConsoleResult(ExecutionResult.Warning, "Console log is already empty!");

            DevConsole.ConsoleLog.Clear();

            return new ConsoleResult(ExecutionResult.Success, "Console log cleared");
        }
    }
}
