module Interpreter

open System
open Language

let processCommand (command: Command) =
    try
        match command with
        | GitStatus -> ()
    with e -> printfn "Exception: %s" e.Message
