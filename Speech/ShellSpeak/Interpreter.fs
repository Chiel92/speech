module Interpreter

open System
open Language

type State() = class end

let processCommand (state:State) (command: Command) =
    try
        match command with
        | GitStatus -> ()
    with e -> printfn "Exception: %s" e.Message
