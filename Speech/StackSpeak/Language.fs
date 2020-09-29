module Language

type Operation =
    | Push of int
    | Pull of int
    | Add
    | Subtract
    | Multiply
    | Duplicate
    | Scratch
    | LessThan
    | Maybe of Operation
    | Call of string

type Definition = string * Operation list

type Command =
    | Op of Operation
    | Def of Definition
    | Undo
    | Nop
