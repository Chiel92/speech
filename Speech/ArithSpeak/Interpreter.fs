module Interpreter
open System
open Language

type Stack = int list

let mutable State = Stack.Empty

let processOperation (stack:Stack) (operation:Operation) =
    try
        match operation with
        | Push x -> x :: stack
        | Add -> stack.[0] + stack.[1] :: List.skip 2 stack
        | Subtract -> stack.[0] - stack.[1] :: List.skip 2 stack
        | Multiply -> stack.[0] * stack.[1] :: List.skip 2 stack
        | Swap -> stack.[1] :: stack.[0] :: List.skip 2 stack
        | Rotate -> stack.[2] :: stack.[0] :: stack.[1] :: List.skip 3 stack
        | Duplicate -> stack.[0] :: stack
        | Scratch -> List.skip 1 stack
    with
    | :? ArgumentException as e ->
        printfn "Exception %s" e.Message
        stack
