module Interpreter

open System.Diagnostics
open Language

type State() =
    class
    end

let Bash (cmd: string): string =
    let escapedArgs = cmd.Replace("\"", "\\\"")

    let proc =
        new Process(StartInfo =
            new ProcessStartInfo(FileName = "cmd.exe",
                                 Arguments = sprintf "/C \"%s\"" escapedArgs,
                                 RedirectStandardOutput = true,
                                 UseShellExecute = false,
                                 CreateNoWindow = true))

    proc.Start()
    let result = proc.StandardOutput.ReadToEnd()
    proc.WaitForExit()
    result


let processCommand (state: State) (command: Command) =
    try
        match command with
        | GitStatus -> Bash "git status" |> printf "%s"
    with e -> printfn "Exception: %s" e.Message
