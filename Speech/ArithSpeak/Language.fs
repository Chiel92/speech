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
    | Call of Definition
    | Undo
and Definition = Define of (string * Operation list) 
