module Interpreter
open System
open Language

type Stack = int list

let mutable State = Stack.Empty

let processOperation (stack:Stack) (operation:Operation) =
    match operation with
    | Push x -> x :: stack
    | Add -> stack.[0] + stack.[1] :: List.skip 2 stack
    | Subtract -> stack.[0] - stack.[1] :: List.skip 2 stack
    | Multiply -> stack.[0] * stack.[1] :: List.skip 2 stack
