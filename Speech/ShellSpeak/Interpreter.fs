module Interpreter

open System.Diagnostics
open Language
open System

type State() =
    class
    end

let Bash (cmd: string): string =
    let escapedArgs = cmd.Replace("\"", "\\\"")

    let previousColor = Console.ForegroundColor
    Console.ForegroundColor <- ConsoleColor.DarkYellow
    printfn "Executing %s" cmd
    Console.ForegroundColor <- previousColor

    let proc =
        new Process(StartInfo =
            new ProcessStartInfo(FileName = "cmd.exe",
                                 Arguments = sprintf "/C \"%s\"" escapedArgs,
                                 RedirectStandardOutput = true,
                                 RedirectStandardError = true,
                                 UseShellExecute = false,
                                 CreateNoWindow = true))

    proc.Start() |> ignore
    let result = proc.StandardOutput.ReadToEnd()
    proc.WaitForExit()
    result


let processCommand (state: State) (command: Command) =
    try
        match command with
        | GitStatus -> Bash "git status" |> printf "%s"
    with e -> printfn "Exception: %s" e.Message
