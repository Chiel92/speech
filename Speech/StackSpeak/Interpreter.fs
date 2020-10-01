module Interpreter

open System
open Language
open System.Collections.Generic

type Stack = int list
type Scope = IReadOnlyDictionary<string, Definition>

type State() =
    let mutable _previousStack = Stack.Empty
    let mutable _stack = Stack.Empty
    let _scope = Dictionary<string, Definition>()

    member this.PreviousStack = _previousStack
    member this.Stack = _stack

    member this.SetStack(stack: Stack) =
        _previousStack <- _stack
        _stack <- stack

    member this.Scope: Scope = upcast _scope
    member this.SetDefinition(name, definition) = _scope.[name] <- definition

let rec processOperation (state: State) frameCount operations =
    if frameCount > 1000000 then invalidOp "Recursion too deep"
    let stack = state.Stack

    match operations with
    | [] -> ()
    | op :: ops ->
        match op with
        | Push x -> x :: stack |> state.SetStack
        | Pull x ->
            stack.[x]
            :: (stack.[..(x - 1)] @ stack.[(x + 1)..])
            |> state.SetStack
        | Add ->
            stack.[0] + stack.[1] :: List.skip 2 stack
            |> state.SetStack
        | Subtract ->
            stack.[0] - stack.[1] :: List.skip 2 stack
            |> state.SetStack
        | Multiply ->
            stack.[0] * stack.[1] :: List.skip 2 stack
            |> state.SetStack
        | Duplicate -> stack.[0] :: stack |> state.SetStack
        | Scratch -> List.skip 1 stack |> state.SetStack
        | LessThan ->
            (if stack.[0] < stack.[1] then 1 else 0)
            :: List.skip 2 stack
            |> state.SetStack
        | _ -> ()

        match op with
        | Maybe innerOp ->
            let isTrue = stack.[0] <> 0
            List.skip 1 stack |> state.SetStack

            if isTrue
            then processOperation state frameCount (innerOp :: ops)
        | Call d ->
            let innerOps = snd state.Scope.[d]
            processOperation state (frameCount + 1) (innerOps @ ops)
        | _ -> processOperation state frameCount ops

let processDefinition (state: State) (definition: Definition) =
    state.SetDefinition(fst definition, definition)
    printfn "Saved definition %A" definition

let processCommand (state: State) (command: Command) =
    try
        match command with
        | Nop -> ()
        | Undo -> state.SetStack(state.PreviousStack)
        | Op o -> processOperation state 0 [ o ]
        | Def d -> processDefinition state d
    with e -> printfn "Exception: %s" e.Message
