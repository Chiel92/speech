module Interpreter
open System
open Language
open System.Collections.Generic

type Stack = int list
type Scope = IReadOnlyDictionary<string,Definition>
type State() = 
    let mutable _previousStack = Stack.Empty
    let mutable _stack = Stack.Empty
    let _scope = Dictionary<string,Definition>()

    member this.PreviousStack = _previousStack
    member this.Stack = _stack
    member this.SetStack(stack:Stack) =
        _previousStack <- _stack
        _stack <- stack

    member this.Scope : Scope = upcast _scope
    member this.SetDefinition(name, definition) =
        _scope.[name] <- definition

let rec processOperation (stack:Stack) (scope:Scope) (operation:Operation) =
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
        | LessThan -> (if stack.[0] < stack.[1] then 1 else 0) :: List.skip 2 stack
        | Maybe op ->
            if stack.[0] <> 0
            then (processOperation (List.skip 1 stack) scope op)
            else stack
        | Call d -> consumeOperations stack scope (snd scope.[d])
    with
    | e ->
        printfn "Exception: %s" e.Message
        stack
and consumeOperations (stack:Stack) (scope:Scope) (operations:Operation list) =
    match operations with
    | op::ops -> consumeOperations (processOperation stack scope op) scope ops
    | [] -> stack

let processDefinition (state:State) (definition:Definition) =
    state.SetDefinition(fst definition, definition)
    printfn "Saved definition %A" definition

let processCommand (state:State) (command:Command) =
    match command with
    | Undo -> state.SetStack(state.PreviousStack)
    | Op o -> state.SetStack(processOperation state.Stack state.Scope o)
    | Def d -> processDefinition state d
