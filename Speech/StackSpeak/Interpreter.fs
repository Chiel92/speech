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

let rec processOperation (state:State) (scope:Scope) (operation:Operation) frameCount =
    let stack = state.Stack
    let newstack =
        match operation with
        | Push x -> Some <| x :: stack
        | Pull x -> Some <| stack.[x] :: (stack.[..(x-1)] @ stack.[(x+1)..])
        | Add -> Some <| stack.[0] + stack.[1] :: List.skip 2 stack
        | Subtract -> Some <| stack.[0] - stack.[1] :: List.skip 2 stack
        | Multiply -> Some <| stack.[0] * stack.[1] :: List.skip 2 stack
        | Duplicate -> Some <| stack.[0] :: stack
        | Scratch -> Some <| List.skip 1 stack
        | LessThan -> Some <| (if stack.[0] < stack.[1] then 1 else 0) :: List.skip 2 stack
        | Maybe op ->
            if stack.[0] <> 0
            then
                state.SetStack(List.skip 1 stack)
                (processOperation state scope op frameCount)
                None
            else Some <| List.skip 1 stack
        | Call d ->
            consumeOperations state scope (snd scope.[d]) (frameCount+1)
            None
    match newstack with
    | Some s -> state.SetStack(s)
    | None -> ()
and consumeOperations state (scope:Scope) (operations:Operation list) frameCount =
    if frameCount > 1000 then invalidOp "Recursion too deep"
    match operations with
    | [] -> ()
    | op::ops -> 
        processOperation state scope op frameCount
        consumeOperations state scope ops frameCount

let processDefinition (state:State) (definition:Definition) =
    state.SetDefinition(fst definition, definition)
    printfn "Saved definition %A" definition

let processCommand (state:State) (command:Command) =
    try
        match command with
        | Nop -> ()
        | Undo -> state.SetStack(state.PreviousStack)
        | Op o -> processOperation state state.Scope o 0
        | Def d -> processDefinition state d
    with
    | e ->
        printfn "Exception: %s" e.Message
