module Interpreter
open System
open Language

type Stack = int list
type State() = 
    let mutable _previousStack = Stack.Empty
    let mutable _stack = Stack.Empty

    member this.PreviousStack = _previousStack
    member this.Stack = _stack
    member this.SetStack(stack:Stack) =
        _previousStack <- _stack
        _stack <- stack

let processOperation (state:State) (operation:Operation) =
    let stack = state.Stack
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
        | Undo -> state.PreviousStack
    with
    | :? ArgumentException as e ->
        printfn "Exception %s" e.Message
        stack
