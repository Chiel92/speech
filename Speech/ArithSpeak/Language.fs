module Language

type Operation =
    | Push of int
    | Add
    | Subtract
    | Multiply
    | Swap
    | Rotate
    | Duplicate
    | Scratch
    | Call of string

type Definition = string * Operation list

type Command = 
    | Op of Operation
    | Def of Definition
    | Undo
